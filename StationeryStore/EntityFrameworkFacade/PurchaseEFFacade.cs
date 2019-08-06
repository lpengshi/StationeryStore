using StationeryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.EntityFrameworkFacade
{
    public class PurchaseEFFacade
    {
        StoreContext context = new StoreContext();

        public void AddToPurchaseOrder(PurchaseOrderEF order)
        {
            context.PurchaseOrders.Add(order);
            context.SaveChanges();
        }

        public void SavePurchaseOrder(PurchaseOrderEF po)
        {
            var existingRecord = context.PurchaseOrders.Find(po.OrderId);
            if (existingRecord != null)
            {
                context.Entry(existingRecord).CurrentValues.SetValues(po);
                context.SaveChanges();
            }
        }
        public List<PurchaseOrderEF> FindAllPurchaseOrders()
        {
            return context.PurchaseOrders.ToList();
        }

        public PurchaseOrderEF FindPurchaseOrderByOrderId(int orderId)
        {
            return context.PurchaseOrders.Find(orderId);
        }

        public void AddToPurchaseOrderDetails(PurchaseOrderDetailsEF orderDetails)
        {
            context.PurchaseOrderDetails.Add(orderDetails);
            context.SaveChanges();
        }

        public List<PurchaseOrderDetailsEF> FindAllPurchaseOrderDetails()
        {
            return context.PurchaseOrderDetails.ToList();
        }

        public List<PurchaseOrderDetailsEF> FindPurchaseOrderDetailsByOrderId(int orderId)
        {
            return context.PurchaseOrderDetails.Where(a => a.OrderId == orderId).ToList<PurchaseOrderDetailsEF>();
        }

        public List<PurchaseOrderDetailsEF> FindPurchaseOrderDetailsByItemCode(string itemCode)
        {
            return context.PurchaseOrderDetails.Where(a => a.ItemCode == itemCode).ToList<PurchaseOrderDetailsEF>();
        }

        public int FindLastPOId()
        {
            return context.PurchaseOrders.Count();
        }

        public int FindLastPODetId()
        {
            return context.PurchaseOrderDetails.Count();
        }

        public bool AddToSupplier(SupplierEF supplier)
        {
            var existingRecord = context.Suppliers.Find(supplier.SupplierCode);
            if (existingRecord == null)
            {
                context.Suppliers.Add(supplier);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public void UpdateSupplier(SupplierEF supplier)
        {
            var existingRecord = context.Suppliers.Find(supplier.SupplierCode);
            if (existingRecord != null)
            {
                context.Entry(existingRecord).CurrentValues.SetValues(supplier);
                context.SaveChanges();
            }
        }

        public void RemoveFromSupplier(SupplierEF supplier)
        {
            context.Suppliers.Remove(supplier);
            context.SaveChanges();
        }

        public List<SupplierEF> FindAllSuppliers()
        {
            return context.Suppliers.ToList();
        }

        public SupplierEF FindSupplierBySupplierCode(string supplierCode)
        {
            return context.Suppliers.Find(supplierCode);
        }

        public void AddToSupplierDetails(SupplierDetailsEF supplierDetails)
        {
            context.SupplierDetails.Add(supplierDetails);
            context.SaveChanges();
        }

        public void RemoveFromSupplierDetails(SupplierDetailsEF supplierDetails)
        {
            context.SupplierDetails.Remove(supplierDetails);
            context.SaveChanges();
        }

        public List<SupplierDetailsEF> FindAllSupplierDetailsBySupplierCode(string supplierCode)
        {
            return context.SupplierDetails.Where(a => a.SupplierCode == supplierCode).OrderBy(a => a.ItemCode).ToList<SupplierDetailsEF>();
        }

        public List<SupplierDetailsEF> FindSupplierDetailsByItemCode(string itemCode)
        {
            return context.SupplierDetails.Where(a => a.ItemCode == itemCode).ToList();
        }
    }
}