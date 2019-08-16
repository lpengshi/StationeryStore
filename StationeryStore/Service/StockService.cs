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

        public void LogTransactionsForDeliveryOrder(int purchaseOrderId)
        {
            List<PurchaseOrderDetailsEF> poDetails = purchaseEFF.FindPurchaseOrderDetailsByOrderId(purchaseOrderId);

            foreach(PurchaseOrderDetailsEF d in poDetails)
            {
                StockTransactionDetailsEF transaction = new StockTransactionDetailsEF()
                {
                    ItemCode = d.ItemCode,
                    Date = Timestamp.unixTimestamp(),
                    Quantity = d.QuantityOrdered,
                    Type = "Supplier "+ d.PurchaseOrder.SupplierCode,
                    Balance = stockEFF.FindStockByItemCode(d.ItemCode).QuantityOnHand + d.QuantityOrdered,
                };
                stockEFF.AddToStockTransaction(transaction);

                StockEF stock = stockEFF.FindStockByItemCode(d.ItemCode);
                stock.QuantityOnHand = stock.QuantityOnHand + d.QuantityOrdered;
                stockEFF.SaveStock(stock);
            }
        }

        public void LogTransactionsForAdjustmentVoucher(string adjustmentVoucherId)
        {
            List<AdjustmentVoucherDetailsEF> adjVoucherDetails = stockEFF.FindAdjustmentVoucherDetailsById(adjustmentVoucherId);

            foreach (AdjustmentVoucherDetailsEF d in adjVoucherDetails)
            {
                StockTransactionDetailsEF transaction = new StockTransactionDetailsEF()
                {
                    ItemCode = d.ItemCode,
                    Date = Timestamp.unixTimestamp(),
                    Quantity = d.Quantity,
                    Type = "Stock Adjustment " + d.VoucherId,
                    Balance = stockEFF.FindStockByItemCode(d.ItemCode).QuantityOnHand + d.Quantity,
                };
                stockEFF.AddToStockTransaction(transaction);

                StockEF stock = stockEFF.FindStockByItemCode(d.ItemCode);
                stock.QuantityOnHand = stock.QuantityOnHand + d.Quantity;
                stockEFF.SaveStock(stock);
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

        //AdjustmentVoucher

        public List<AdjustmentVoucherEF> FindAllAdjustmentVouchers()
        {
            return stockEFF.FindAllAdjustmentVouchers();
        }

        public AdjustmentVoucherEF FindAdjustmentVoucherById(string voucherId)
        {
            return stockEFF.FindAdjustmentVoucherById(voucherId);
        }

        public List<AdjustmentVoucherDetailsEF> FindAdjustmentDetailsById(string voucherId)
        {
            return stockEFF.FindAdjustmentVoucherDetailsById(voucherId);
        }

        public bool UpdateAdjustmentVoucherStatus(StaffEF decisionMaker, string voucherId, string status)
        {
            AdjustmentVoucherEF voucher = FindAdjustmentVoucherById(voucherId);
            if(status == "Approve")
            {
                voucher.Status = "Approved";
                voucher.ApproverId = decisionMaker.StaffId;
                //change status to Approved, update the stock card
                stockEFF.UpdateAdjustmentVoucher(voucher);
                LogTransactionsForAdjustmentVoucher(voucherId);
                return true;
            }
            if(status == "Reject")
            {
                voucher.Status = "Rejected";
                voucher.ApproverId = decisionMaker.StaffId;
                stockEFF.UpdateAdjustmentVoucher(voucher);
            }
            return false;
        }

        public string SaveAdjustmentVoucherAndDetails(StaffEF requester, AdjustmentVoucherEF voucher, List<AdjustmentVoucherDetailsDTO> detailsList, string approvalLevel)
        {           
            var existing = stockEFF.FindAdjustmentVoucherById(voucher.VoucherId);
            if (existing == null)
            {
                int year = DateTime.Now.Year;
                string prefix = "VO/" + year + "/";
                int num = stockEFF.FindLastAdjustmentVoucher(prefix) + 1;
                voucher.VoucherId = prefix + num;

                long unixTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                voucher.DateIssued = unixTimestamp;
                voucher.RequesterId = requester.StaffId;

                if(approvalLevel == "Manager")
                {
                    voucher.Status = "Pending Manager Approval";

                }
                else if(approvalLevel == "Supervisor")
                {
                    voucher.Status = "Pending Approval";
                }
                stockEFF.AddNewAdjustmentVoucherAndDetails(voucher, detailsList);
                //return stockEFF.AddNewAdjustmentVoucherAndDetails(voucher, detailsList);
            }
            else
            {
                stockEFF.FindAndReplaceAdjustmentVoucherDetails(voucher.VoucherId, detailsList);
            }
            //return false;
            return voucher.VoucherId;
        }

        public bool VoucherExceedsSetValue(List<AdjustmentVoucherDetailsDTO> detailsList)
        {
            double minValPerLineItem = 250;
            bool valueExceeded = false;
            foreach(var d in detailsList)
            {
                //Derived the cost flag by averaging stock cost across suppliers.
                List<SupplierDetailsEF> items = purchaseEFF.FindSupplierDetailsByItemCode(d.ItemCode);
                double unitAvgCost = items.Average(x => x.UnitPrice);
                double sumPrice = unitAvgCost * d.Quantity;
                if(sumPrice >= minValPerLineItem || sumPrice <= (-1 * minValPerLineItem))
                {
                    valueExceeded = true;
                }
            }
            return valueExceeded;
        }

        public bool VoucherExceedsSetValue(List<AdjustmentVoucherDetailsEF> detailsList)
        {
            double minValPerLineItem = 250;
            bool valueExceeded = false;
            foreach (var d in detailsList)
            {
                //Derived the cost flag by averaging stock cost across suppliers.
                List<SupplierDetailsEF> items = purchaseEFF.FindSupplierDetailsByItemCode(d.ItemCode);
                double unitAvgCost = items.Average(x => x.UnitPrice);
                double sumPrice = unitAvgCost * d.Quantity;
                if (sumPrice >= minValPerLineItem || sumPrice <= (-1 * minValPerLineItem))
                {
                    valueExceeded = true;
                }
            }
            return valueExceeded;
        }
       
        public List<AdjustmentVoucherDetailsDTO> GetPricesForVoucherDetails(List<AdjustmentVoucherDetailsDTO> detailsList)
        {
            foreach (var d in detailsList)
            {
                //Averaging stock cost across suppliers.
                List<SupplierDetailsEF> items = purchaseEFF.FindSupplierDetailsByItemCode(d.ItemCode);
                d.Price = items.Average(x => x.UnitPrice);           
            }
            return detailsList;
        }

        public List<AdjustmentVoucherDetailsDTO> ConvertAdjVoucherDetailsToDTO(List<AdjustmentVoucherDetailsEF> detailsList)
        {
            List<AdjustmentVoucherDetailsDTO> dtoList = new List<AdjustmentVoucherDetailsDTO>();

            foreach(var d in detailsList)
            {
                AdjustmentVoucherDetailsDTO newItem = new AdjustmentVoucherDetailsDTO();
                List<SupplierDetailsEF> items = purchaseEFF.FindSupplierDetailsByItemCode(d.ItemCode);
                newItem.Price = items.Average(x => x.UnitPrice);
                newItem.ItemCode = d.ItemCode;
                newItem.Quantity = d.Quantity;
                newItem.Reason = d.Reason;
                newItem.Description = d.Stock.Description;

                dtoList.Add(newItem);
            }
            return dtoList;
        }
    }
}