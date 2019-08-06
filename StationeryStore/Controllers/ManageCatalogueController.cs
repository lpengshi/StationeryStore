using StationeryStore.Filters;
using StationeryStore.Models;
using StationeryStore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StationeryStore.Controllers
{
    public class ManageCatalogueController : Controller
    {
        StockService stockService = new StockService();
        StaffService staffService = new StaffService();

        // GET: ManageCatalogue

        public ActionResult ViewCatalogue(int page, string search)
        {
            int pageSize = 8;
            List<CatalogueItemEF> fullCatalogue = stockService.ListCatalogueItems();
            if (!String.IsNullOrEmpty(search))
            {
                fullCatalogue = fullCatalogue.Where(s => s.Stock.Description.ToLower().Contains(search.ToLower()) || 
                            s.ItemCode.ToLower().Contains(search.ToLower())).ToList<CatalogueItemEF>();
            }
            List<CatalogueItemEF> catalogue = fullCatalogue.OrderBy(x => x.CatalogueId).Skip((page-1)*pageSize).Take(pageSize).ToList<CatalogueItemEF>();
            int noOfPages = (int)Math.Ceiling((double)fullCatalogue.Count() / pageSize);
            ViewData["page"] = page;
            ViewData["catalogue"] = catalogue;
            ViewData["noOfPages"] = noOfPages;
            ViewData["search"] = search;
            return View();
        }

        public ActionResult ViewCatalogueItem(int catalogueId)
        {
            CatalogueItemEF catItem = stockService.FindCatalogueItemById(catalogueId);
            if (catItem == null)
            {
                return View("Error");
            }
            ViewData["catItem"] = catItem;

            StaffEF staff = staffService.GetStaff();
            ViewData["staff"] = staff;
            return View();
        }

        [StoreManagerFilter]
        [HttpGet]
        public ActionResult CreateCatalogueItem()
        {
            //get a list of itemCodes from stock that do not have a catalogue
            List<StockEF> itemsNotInCat = stockService.FindItemsNotInCatalogue();
            IEnumerable<SelectListItem> items = itemsNotInCat
                                           .Select(i => new SelectListItem()
                                           {
                                               Text = i.ItemCode + ": " + i.Description,
                                               Value = i.ItemCode
                                           });
            ViewData["ItemCode"] = items;

            CatalogueItemDTO item = new CatalogueItemDTO();

            return View(item);
        }

        [StoreManagerFilter]
        [HttpPost]
        public ActionResult CreateCatalogueItem(CatalogueItemDTO item)
        {
            //create catalogue item
            CatalogueItemEF catItem = new CatalogueItemEF()
            {
                ItemCode = item.ItemCode,
                ReorderLevel = item.ReorderLevel,
                ReorderQty = item.ReorderQty
            };
            stockService.CreateCatalogueItem(catItem);
            return RedirectToAction("ViewCatalogueItem", new { catalogueId = catItem.CatalogueId });
        }

        [StoreManagerFilter]
        [HttpGet]
        public ActionResult UpdateCatalogueItem(int catalogueId)
        {
            CatalogueItemEF catItem = stockService.FindCatalogueItemById(catalogueId);
            if (catItem == null)
            {
                return View("Error");
            }
            CatalogueItemDTO item = new CatalogueItemDTO()
            {
                CatalogueId = catItem.CatalogueId,
                ItemCode = catItem.ItemCode,
                Description = catItem.Stock.Description,
                ReorderLevel = catItem.ReorderLevel,
                ReorderQty = catItem.ReorderQty
            };
            return View(item);
        }

        [StoreManagerFilter]
        [HttpPost]
        public ActionResult UpdateCatalogueItem(CatalogueItemDTO item, string decision)
        {
            if (decision == "Cancel")
            {
                return RedirectToAction("ViewCatalogueItem", new { catalogueId = item.CatalogueId });
            }

            if (decision == "Predict Reorder Level")
            {
                // get prediction from service
                int prediction = 1;
                // set it and throw it back to the update catalogue view
                ModelState.Remove("ReorderLevel");
                item.ReorderLevel = prediction;
                return View(item);
            }

            if (decision == "Predict Reorder Quantity")
            {
                // get prediction from service
                int prediction = 1;
                // set it and throw it back to the update catalogue view
                ModelState.Remove("ReorderQty");
                item.ReorderQty = prediction;
                return View(item);
            }

            // decision == Save
            CatalogueItemEF catItem = new CatalogueItemEF()
            {
                CatalogueId = item.CatalogueId,
                ItemCode = item.ItemCode,
                ReorderLevel = item.ReorderLevel,
                ReorderQty = item.ReorderQty
            };
            stockService.UpdateCatalogueItem(catItem);
            return RedirectToAction("ViewCatalogueItem", new { catalogueId = catItem.CatalogueId });
        }

        [StoreManagerFilter]
        public ActionResult DeleteCatalogueItem(int catalogueId)
        {
            CatalogueItemEF item = stockService.FindCatalogueItemById(catalogueId);
            stockService.DeleteCatalogueItem(item);
            return RedirectToAction("ViewCatalogue", new { page = 1 });
        }
    }
}