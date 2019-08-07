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
    public class ViewLowStockController : Controller
    {
        StockService stockService = new StockService();

        // GET: ViewLowStock
        public ActionResult ViewLowStock(int page)
        {
            List<LowStockDTO> stocks = stockService.FindLowStock();

            int pageSize = 8;
            List<LowStockDTO> lowStocks = stocks
                .OrderBy(x => x.ItemCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList<LowStockDTO>();

            int noOfPages = (int)Math.Ceiling((double)stocks.Count() / pageSize);
            ViewData["page"] = page;
            ViewData["lowStocks"] = lowStocks;
            ViewData["noOfPages"] = noOfPages;
            return View();
        }
    }
}