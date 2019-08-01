using StationeryStore.EntityFrameworkFacade;
using StationeryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StationeryStore.Util;

namespace StationeryStore.Service
{
    public class StockService
    {
        StockEFFacade stockEFF = new StockEFFacade();
        RequestAndDisburseEFFacade rndEFF = new RequestAndDisburseEFFacade();

        public void CreateStock(StockEF stock)
        {
            stockEFF.AddToStock(stock);
        }

        public void UpdateStock(StockEF stock)
        {
            stockEFF.SaveStock(stock);
        }

        public List<StockEF> FindAllStocks()
        {
            return stockEFF.FindAllStock();
        }

        public StockEF FindStockByItemCode(string itemCode)
        {
            return stockEFF.FindStockByItemCode(itemCode);
        }

        public StockEF FindStockByDescription(string description)
        {
            return stockEFF.FindStockByDescription(description);
        }

        public List<LowStockDTO> FindLowStock()
        {
            return stockEFF.FindLowStock();
        }

        public List<StockTransactionDetailsEF> FindTransactionsByItemCode(string itemCode)
        {
            return stockEFF.FindTransactionsbyItemCode(itemCode);
        }

        public void LogTransactionsForRetrieval(int retrievalId)
        {
            List<StationeryDisbursementDetailsEF> details = rndEFF.FindDisbursementDetailsByRetrievalId(retrievalId);
            foreach(StationeryDisbursementDetailsEF d in details)
            {
                StockTransactionDetailsEF transaction = new StockTransactionDetailsEF()
                {
                    ItemCode = d.ItemCode,
                    Date = Timestamp.unixTimestamp(),
                    Quantity = -d.RetrievedQuantity,
                    Type = d.StationeryDisbursement.Department.DepartmentName,
                    Balance = stockEFF.FindStockByItemCode(d.ItemCode).QuantityOnHand - d.RetrievedQuantity
                };
                stockEFF.AddToStockTransaction(transaction);

                StockEF stock = stockEFF.FindStockByItemCode(d.ItemCode);
                stock.QuantityOnHand = transaction.Balance;
                stockEFF.SaveStock(stock);
            }
        }

        public void LogTransactionsForActualDisbursement(int disbursementId)
        {
            List<StationeryDisbursementDetailsEF> details = rndEFF.FindDisbursementDetailsByDisbursementId(disbursementId);
            foreach(StationeryDisbursementDetailsEF d in details)
            {
                if(d.DisbursedQuantity != d.RetrievedQuantity)
                {
                    StockTransactionDetailsEF transaction = new StockTransactionDetailsEF()
                    {
                        ItemCode = d.ItemCode,
                        Date = Timestamp.unixTimestamp(),
                        Quantity = d.RetrievedQuantity - d.DisbursedQuantity,
                        Type = d.StationeryDisbursement.Department.DepartmentName,
                        Balance = stockEFF.FindStockByItemCode(d.ItemCode).QuantityOnHand + (d.RetrievedQuantity - d.DisbursedQuantity)
                    };
                    stockEFF.AddToStockTransaction(transaction);

                    StockEF stock = stockEFF.FindStockByItemCode(d.ItemCode);
                    stock.QuantityOnHand = transaction.Balance;
                    stockEFF.SaveStock(stock);
                }
            }
        }


        //Catalogue
        public List<CatalogueItemEF> ListCatalogueItems()
        {
            var catalogueList = stockEFF.FindAllCatalogueItems();

            return catalogueList;
        }

        public List<StockEF> FindItemsNotInCatalogue()
        {
            List<CatalogueItemEF> catItems = stockEFF.FindAllCatalogueItems();
            List<StockEF> stock = stockEFF.FindAllStock();
            List<StockEF> itemsNotInCat = stock.Where(c => !catItems.Any(s => s.ItemCode == c.ItemCode)).ToList<StockEF>();
            return itemsNotInCat;
        }

        public void CreateCatalogueItem(CatalogueItemEF item)
        {
            stockEFF.AddToCatalogueItem(item);
        }

        public void DeleteCatalogueItem(CatalogueItemEF item)
        {
            if (item != null)
            {
                stockEFF.RemoveFromCatalogue(item);
            }
        }

        public void UpdateCatalogueItem(CatalogueItemEF item)
        {
            if (item != null)
            {
                stockEFF.SaveCatalogueItem(item);
            }
        }

        public CatalogueItemEF FindCatalogueItemById(int catalogueId)
        {
            return stockEFF.FindCatalogueItemById(catalogueId);
        }
    }
}