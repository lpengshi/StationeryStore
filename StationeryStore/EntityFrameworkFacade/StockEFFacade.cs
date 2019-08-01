using StationeryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.EntityFrameworkFacade
{
    public class StockEFFacade
    {
        StoreContext context = new StoreContext();

        public void AddToStock(StockEF stock)
        {
            context.Stocks.Add(stock);
            context.SaveChanges();
        }
        public void SaveStock(StockEF stock)
        {
            var existingStock = context.Stocks.Find(stock.ItemCode);
            if (existingStock != null)
            {
                existingStock.ItemCode = stock.ItemCode;
                existingStock.Category = stock.Category;
                existingStock.Description = stock.Description;
                existingStock.Uom = stock.Uom;
                existingStock.Bin = stock.Bin;
                existingStock.QuantityOnHand = stock.QuantityOnHand;
                context.SaveChanges();
            }
        }

        public List<StockEF> FindAllStock()
        {
            var stockList = context.Stocks.ToList();

            return stockList;
        }

        public StockEF FindStockByItemCode(string itemcode)
        {
            var stockItem = context.Stocks.Where(a => a.ItemCode == itemcode).SingleOrDefault();

            return stockItem;
        }

        public StockEF FindStockByDescription(string description)
        {
            var stockItem = context.Stocks.Single(a => a.Description == description);

            return stockItem;
        }


        public List<StockEF> FindAllStockByCategory(string category)
        {
            var stockList = context.Stocks.Where(a => a.Category == category).ToList();
            return stockList;
        }
            
        public List<CatalogueItemEF> FindAllCatalogueItems()
        {
            var catalogueList = context.CatalogueItems.ToList();
            foreach (var item in catalogueList)
            {
                item.Stock = context.Stocks.Where(a => a.ItemCode == item.ItemCode).SingleOrDefault();
            }
            return catalogueList;
        }

        public List<LowStockDTO> FindLowStock()
        {
            var stocks = from stock in context.Stocks
                         join cat in context.CatalogueItems
                         on stock equals cat.Stock
                         where stock.QuantityOnHand < cat.ReorderLevel
                         select new
                         {
                             stock.ItemCode,
                             stock.Description,
                             stock.QuantityOnHand,
                             cat.ReorderLevel,
                             cat.ReorderQty
                         };

            List<LowStockDTO> lowStockList = new List<LowStockDTO>();
            foreach (var s in stocks)
            {
                LowStockDTO lowStock = new LowStockDTO()
                {
                    ItemCode = s.ItemCode,
                    Description = s.Description,
                    QuantityOnHand = s.QuantityOnHand,
                    ReorderLevel = s.ReorderLevel,
                    ReorderQuantity = s.ReorderQty
                };
                lowStockList.Add(lowStock);
            }
            return lowStockList;
        }

        public void AddToStockTransaction(StockTransactionDetailsEF trans)
        {
            context.StockTransactionDetails.Add(trans);
            context.SaveChanges();
        }

        public List<StockTransactionDetailsEF> FindTransactionsbyItemCode(string itemCode)
        {
            return context.StockTransactionDetails.Where(a => a.ItemCode == itemCode).ToList();
        }


        public void AddToCatalogueItem(CatalogueItemEF item)
        {
            context.CatalogueItems.Add(item);
            context.SaveChanges();
        }

        public void SaveCatalogueItem(CatalogueItemEF item)
        {
            var existingItem = context.CatalogueItems.Find(item.CatalogueId);
            if (existingItem != null)
            {
                existingItem.ReorderLevel = item.ReorderLevel;
                existingItem.ReorderQty = item.ReorderQty;
                context.SaveChanges();
            }
        }

        public void RemoveFromCatalogue(CatalogueItemEF item)
        {
            context.CatalogueItems.Remove(item);
            context.SaveChanges();
        }

        public List<CatalogueItemEF> FindAllCatalogueItem()
        {
            return context.CatalogueItems.ToList();
        }

        public CatalogueItemEF FindCatalogueItemById(int catalogueId)
        {
            return context.CatalogueItems.Find(catalogueId);
        }

    }
}