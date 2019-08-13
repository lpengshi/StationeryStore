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
    [DepartmentFilter]
    public class ViewCatalogueController : Controller
    {
        StaffService staffService = new StaffService();
        StockService stockService = new StockService();

        [HttpGet]
        public ActionResult Index(string id, string description, string decision)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            //retrieve all catalogue items and do pagination
            int pageNum; int startFrom; 
            List<CatalogueItemEF> catalogueList = stockService.ListCatalogueItems();
            List<CatalogueItemEF> newList = new List<CatalogueItemEF>();

            if (description != null && decision != "Reset")
            {
                List<CatalogueItemEF> searchList = new List<CatalogueItemEF>();
                foreach (CatalogueItemEF item in catalogueList)
                {
                    if (item.Stock.Description.ToLower().Contains(description.ToLower()))
                    {
                        searchList.Add(item);
                    }
                }

                catalogueList = searchList;
            } else if (decision == "Reset")
            {
                description = "";
            }

            if (id != null)
            {
                pageNum = int.Parse(id);

            } else
            {
                pageNum = 1;
            }

            startFrom = (pageNum - 1) * 10;
            for (int i = startFrom; i < (startFrom + 10); i++ )
            {
                if (catalogueList.Count != 0)
                {
                    newList.Add(catalogueList[i]);

                    if (i == catalogueList.Count - 1)
                    {
                        break;
                    }
                } else
                {
                    break;
                }
            }

            ViewBag.description = description;
            ViewBag.pageNum = catalogueList.Count / 10;
            ViewBag.catalogueList = newList;

            return View();
        }
    }
}