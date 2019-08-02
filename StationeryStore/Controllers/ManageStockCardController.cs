using StationeryStore.Models;
using StationeryStore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StationeryStore.Controllers
{
    public class ManageStockCardController : Controller
    {
        StockService stockService = new StockService();
        PurchaseService purchaseService = new PurchaseService();

        // GET: ManageStockCard
        public ActionResult ViewAllStocks(int page, string search)
        {
            int pageSize = 8;
            List<StockEF> fullStocks = stockService.FindAllStocks();
            if (!String.IsNullOrEmpty(search))
            {
                fullStocks = fullStocks.Where(s => s.Description.ToLower().Contains(search.ToLower()) ||
                            s.ItemCode.ToLower().Contains(search.ToLower())).ToList<StockEF>();
            }
            List<StockEF> stocks = fullStocks.Skip((page - 1) * pageSize).Take(pageSize).ToList<StockEF>();
            int noOfPages = (int)Math.Ceiling((double)fullStocks.Count() / pageSize);
            ViewData["page"] = page;
            ViewData["stocks"] = stocks;
            ViewData["noOfPages"] = noOfPages;
            ViewData["search"] = search;
            return View();
        }

        public ActionResult ViewStockCard(string itemCode)
        {
            StockEF stock = stockService.FindStockByItemCode(itemCode);
            ViewData["stock"] = stock;

            List<SupplierEF> suppliers = purchaseService.FindSupplierByItemCodeOrderByRank(itemCode);
            ViewData["suppliers"] = suppliers;

            List<StockTransactionDetailsEF> transactions = stockService.FindTransactionsByItemCode(itemCode);
            ViewData["transactions"] = transactions;
            return View();
        }

        [HttpGet]
        public ActionResult UpdateStockCard(string itemCode)
        {
            StockEF s = stockService.FindStockByItemCode(itemCode);
            StockCardDTO stock = new StockCardDTO()
            {
                ItemCode = s.ItemCode,
                Category = s.Category,
                Description = s.Description,
                Uom = s.Uom,
                Bin = s.Bin,
                QuantityOnHand = s.QuantityOnHand
            };
            return View(stock);
        }

        [HttpPost]
        public ActionResult UpdateStockCard(StockCardDTO stock, string decision)
        {
            if (decision == "Cancel")
            {
                return RedirectToAction("ViewStockCard", new { itemCode = stock.ItemCode });
            }
            //check that item description does not exist
            StockEF existingStock = stockService.FindStockByDescription(stock.Description);
            if(existingStock != null && existingStock.ItemCode != stock.ItemCode)
            {
                ModelState.AddModelError("Description", "Description already exists");
                return View(stock);
            }
            //save the stock
            StockEF s = new StockEF()
            {
                ItemCode = stock.ItemCode,
                Category = stock.Category,
                Description = stock.Description,
                Uom = stock.Uom,
                Bin = stock.Bin,
                QuantityOnHand = stock.QuantityOnHand
            };
            stockService.UpdateStock(s);

            return RedirectToAction("ViewStockCard", new { itemCode = stock.ItemCode });
        }

        [HttpGet]
        public ActionResult CreateStockCard()
        {
            StockCardDTO stock = new StockCardDTO();
            return View(stock);
        }

        [HttpPost]
        public ActionResult CreateStockCard(StockCardDTO stock, string decision)
        {
            if(decision == "Cancel")
            {
                return RedirectToAction("ViewAllStocks");
            }
            //check that item code and description does not exist
            if(stockService.FindStockByItemCode(stock.ItemCode) != null)
            {
                ModelState.AddModelError("ItemCode", "Item Code already exists");
                if(stockService.FindStockByDescription(stock.Description) != null)
                {
                    ModelState.AddModelError("Description", "Description already exists");
                }
                return View(stock);
            }
            //save the stock
            StockEF s = new StockEF()
            {
                ItemCode = stock.ItemCode,
                Category = stock.Category,
                Description = stock.Description,
                Uom = stock.Uom,
                Bin = stock.Bin,
                QuantityOnHand = 0
            };
            stockService.CreateStock(s);

            return RedirectToAction("ViewStockCard", new { itemCode = stock.ItemCode });
        }
    }
}