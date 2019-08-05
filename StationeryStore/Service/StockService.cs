﻿using StationeryStore.EntityFrameworkFacade;
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
        PurchaseEFFacade purchaseEFF = new PurchaseEFFacade();

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


        //ReorderReport
        public List<ReorderReportDTO> GenerateReorderReport()
        {
            List<LowStockDTO> lowStocks = stockEFF.FindLowStock();
            // for each item code in the low stock, find a corresponding PO and delivery date
            List<ReorderReportDTO> reorderList = new List<ReorderReportDTO>();
            foreach(LowStockDTO s in lowStocks)
            {
                // find the order details for each item
                List<PurchaseOrderDetailsEF> details = purchaseEFF.FindPurchaseOrderDetailsByItemCode(s.ItemCode)
                    .Where(o => o.PurchaseOrder.Status == "Pending Delivery").ToList();
                List<PurchaseOrderEF> orders = new List<PurchaseOrderEF>();
                foreach(PurchaseOrderDetailsEF d in details)
                {
                    orders.Add(d.PurchaseOrder);
                }

                ReorderReportDTO item = new ReorderReportDTO()
                {
                    LowStockDTO = s,
                    Order = orders
                };
                reorderList.Add(item);
            }
            return reorderList;
        }

        public List<InventoryStatusReportDTO> GenerateInventoryStatusReport()
        {
            List<StockEF> stocks = stockEFF.FindAllStock();
            List<InventoryStatusReportDTO> inventoryStatusList = new List<InventoryStatusReportDTO>();
            foreach(StockEF s in stocks)
            {
                InventoryStatusReportDTO item = new InventoryStatusReportDTO()
                {
                    Stock = s,
                    CatalogueItem = stockEFF.FindCatalogueItemByItemCode(s.ItemCode)
                };
                inventoryStatusList.Add(item);
            }
            return inventoryStatusList;
        }
    }
}