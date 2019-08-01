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
    public class ViewCatalogueController : Controller
    {
        StaffService staffService = new StaffService();
        StockService stockService = new StockService();

        public ActionResult Index(string id)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            
            int pageNum; int startFrom; 
            List<CatalogueItemEF> catalogueList = stockService.ListCatalogueItems();
            List<CatalogueItemEF> newList = new List<CatalogueItemEF>();

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
                newList.Add(catalogueList[i]);

                if (i == catalogueList.Count - 1)
                {
                    break;
                }
            }

            ViewBag.pageNum = catalogueList.Count / 10;
            ViewBag.catalogueList = newList;

            return View();
        }
    }
}