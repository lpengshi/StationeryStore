using StationeryStore.Models;
using StationeryStore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StationeryStore.Controllers
{
    public class GenerateReportController : Controller
    {
        StockService stockService = new StockService();
        
        public ActionResult GenerateReorderReport(int page)
        {
            int pageSize = 8;
            List<ReorderReportDTO> reorderReport = stockService.GenerateReorderReport()
                .OrderBy(x => x.LowStockDTO.ItemCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList(); 

            int noOfPages = (int)Math.Ceiling((double)reorderReport.Count() / pageSize);
            ViewData["page"] = page;
            ViewData["reorderReport"] = reorderReport;
            ViewData["noOfPages"] = noOfPages;
            return View();
        }

        public ActionResult DownloadReorderReport()
        {

            return RedirectToAction("GenerateReorderReport");
        }

        public ActionResult GenerateInventoryStatusReport(int page)
        {
            List<InventoryStatusReportDTO> fullInventoryStatusReport = stockService.GenerateInventoryStatusReport();

            int pageSize = 8;
            List<InventoryStatusReportDTO> inventoryStatusReport = stockService.GenerateInventoryStatusReport()
                .OrderBy(x => x.Stock.Category)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int noOfPages = (int)Math.Ceiling((double)fullInventoryStatusReport.Count() / pageSize);
            ViewData["page"] = page;
            ViewData["inventoryStatusReport"] = inventoryStatusReport;
            ViewData["noOfPages"] = noOfPages;
            return View();
        }

        public ActionResult GenerateOrderTrendAnalysis()
        {
            return View();
        }

        public ActionResult GenerateRequestTrendAnalysis()
        {
            return View();
        }
    }
}