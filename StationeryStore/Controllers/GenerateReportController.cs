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
    [AuthorizeFilter]
    [StoreFilter]
    public class GenerateReportController : Controller
    {
        StockService stockService = new StockService();
        
        public ActionResult GenerateReorderReport()
        {
            return View();
        }

        public ActionResult GenerateInventoryStatusReport()
        {
            return View();
        }

        [StoreSupervisorFilter]
        public ActionResult GenerateOrderTrendAnalysis()
        {
            return View();
        }

        [StoreSupervisorFilter]
        public ActionResult GenerateRequestTrendAnalysis()
        {
            return View();
        }
    }
}