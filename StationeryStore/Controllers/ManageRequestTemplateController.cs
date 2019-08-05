using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Filters;
using StationeryStore.Models;
using StationeryStore.Service;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    public class ManageRequestTemplateController : Controller
    {
        StaffService staffService = new StaffService();
        StockService stockService = new StockService();
        RequestAndDisburseService rndService = new RequestAndDisburseService();

        public ActionResult ViewRequestTemplate()
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            RequestTemplateEF requestTemplate = rndService.FindRequestTemplateByStaffId(staff.StaffId);

            RequestListDTO requestListDTO = null;

            if (TempData["requestListDTO"] != null)
            {
                requestListDTO = (RequestListDTO)TempData["requestListDTO"];
                ViewBag.amend = true;
            }

            return View(requestListDTO);
        }


        [HttpGet]
        public ActionResult CreateRequestTemplate()
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            List<CatalogueItemEF> catalogueList = stockService.ListCatalogueItems();
            ViewBag.catalogueList = catalogueList;
            RequestListDTO requestListDTO = null;

            if (TempData["requestListDTO"] != null)
            {
                requestListDTO = (RequestListDTO)TempData["requestListDTO"];
                ViewBag.amend = true;
            }

            return View(requestListDTO);
        }

        [HttpPost]
        public ActionResult CreateRequest(RequestListDTO requestListDTO, string items, string decision)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            List<CatalogueItemEF> catalogueList = stockService.ListCatalogueItems();
            ViewBag.catalogueList = catalogueList;

            if (requestListDTO.RequestId != null)
            {
                ViewBag.amend = true;
            }

            if (decision == "Delete")
            {
                rndService.DeleteRequest(requestListDTO.RequestId);

                return RedirectToAction("Index");
            }

            if (decision == null)
            {
                ViewBag.note = "Invalid quantity detected. Please amend.";
            }

            if (items != null && decision == "Add Item")
            {
                CatalogueItemEF newItem = null;
                bool checkValid = false;
                foreach (var item in catalogueList)
                {
                    if (item.Stock.Description == items)
                    {
                        checkValid = true;
                        newItem = item;
                        break;
                    }
                }

                if (!checkValid)
                {
                    ViewBag.note = "The item is not recognised. Please select another item from the list";
                }

                if (requestListDTO.ItemDescription.Count > 0 && checkValid == true)
                {
                    foreach (var description in requestListDTO.ItemDescription)
                    {
                        if (description == items)
                        {
                            checkValid = false;
                            ViewBag.note = "The item has already been added to the request list";
                            break;
                        }
                    }
                }
                if (checkValid)
                {
                    requestListDTO = rndService.AddToRequestListDTO(requestListDTO, newItem.Stock.Description, newItem.Stock.Uom);
                }
            }

            if (decision == "Remove Selected Items")
            {
                int before = requestListDTO.ItemDescription.Count;
                requestListDTO = rndService.RemoveFromRequestListDTO(requestListDTO);
                int after = requestListDTO.ItemDescription.Count;

                if (before != after)
                {
                    ModelState.Clear();
                }
                else
                {
                    ViewBag.note = "Please check the remove box to remove selected row";
                }

            }

            if (decision == "Submit" || decision == "Update")
            {
                if (requestListDTO.ItemDescription.Count == 0)
                {
                    ViewBag.note = "You will need to have at least one item in the request";
                }
                else
                {
                    //rndService.CreateRequest(staff, requestListDTO);
                    return RedirectToAction("Index");
                }
            }
            return View(requestListDTO);
        }
    }
}