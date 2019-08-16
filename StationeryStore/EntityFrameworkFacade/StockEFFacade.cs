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
                context.Entry(existingStock).CurrentValues.SetValues(stock);
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
            var stockItem = context.Stocks.SingleOrDefault(a => a.Description == description);

            return stockItem;
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
                context.Entry(existingItem).CurrentValues.SetValues(item);
                context.SaveChanges();
            }
        }

        public void RemoveFromCatalogue(CatalogueItemEF item)
        {
            context.CatalogueItems.Remove(item);
            context.SaveChanges();
        }

        public CatalogueItemEF FindCatalogueItemById(int catalogueId)
        {
            return context.CatalogueItems.Find(catalogueId);
        }

        //ADJUSTMENT VOUCHER

        public List<AdjustmentVoucherEF> FindAllAdjustmentVouchers()
        {
            List<AdjustmentVoucherEF> finalList = new List<AdjustmentVoucherEF>();
            foreach (var v in context.AdjustmentVouchers.Where(x => x.Status == "Pending Approval").OrderByDescending(x => x.VoucherId).ToList())
            {
                finalList.Add(v);
            }
            foreach (var v in context.AdjustmentVouchers.Where(x => x.Status != "Pending Approval").OrderByDescending(x => x.VoucherId).ToList())
            {
                finalList.Add(v);
            }

            return finalList;
        }

        public AdjustmentVoucherEF FindAdjustmentVoucherById(string voucherId)
        {
            return context.AdjustmentVouchers.Where(x => x.VoucherId == voucherId).SingleOrDefault();
        }

        public List<AdjustmentVoucherDetailsEF> FindAdjustmentVoucherDetailsById(string voucherId)
        {
            return context.AdjustmentVouchersDetails.Where(x => x.VoucherId == voucherId).ToList();
        }

        public bool UpdateAdjustmentVoucher(AdjustmentVoucherEF voucher)
        {
            var existingRecord = context.AdjustmentVouchers.Find(voucher.VoucherId);
            if (existingRecord != null)
            {
                context.Entry(existingRecord).CurrentValues.SetValues(voucher);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool AddNewAdjustmentVoucherAndDetails(AdjustmentVoucherEF voucher, List<AdjustmentVoucherDetailsDTO> voucherDetailsList)
        {
            var existingRecord = context.AdjustmentVouchers.Find(voucher.VoucherId);
            if (existingRecord == null)
            {
                context.AdjustmentVouchers.Add(voucher);
                foreach (AdjustmentVoucherDetailsDTO newItem in voucherDetailsList)
                {
                    AdjustmentVoucherDetailsEF d = new AdjustmentVoucherDetailsEF();
                    d.ItemCode = newItem.ItemCode;
                    d.Quantity = newItem.Quantity;
                    d.Reason = newItem.Reason;
                    d.VoucherId = voucher.VoucherId;
                    context.AdjustmentVouchersDetails.Add(d);
                }
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public int FindLastAdjustmentVoucher(string prefix)
        {
            return context.AdjustmentVouchers.Count(x => x.VoucherId.StartsWith(prefix));
        }

        public bool FindAndReplaceAdjustmentVoucherDetails(string voucherId, List<AdjustmentVoucherDetailsDTO> detailsList)
        {
            //delete existing records
            context.AdjustmentVouchersDetails.RemoveRange(context.AdjustmentVouchersDetails.Where(x => x.VoucherId == voucherId));

            //replace records
            foreach (AdjustmentVoucherDetailsDTO newItem in detailsList)
            {
                AdjustmentVoucherDetailsEF d = new AdjustmentVoucherDetailsEF();
                d.ItemCode = newItem.ItemCode;
                d.Quantity = newItem.Quantity;
                d.Reason = newItem.Reason;
                d.VoucherId = voucherId;
                context.AdjustmentVouchersDetails.Add(d);
            }
            context.SaveChanges();

            return false;
        }
    }
}