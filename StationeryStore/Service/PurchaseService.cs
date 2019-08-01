using StationeryStore.EntityFrameworkFacade;
using StationeryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Service
{
    public class PurchaseService
    {
        PurchaseEFFacade purchaseEFF = new PurchaseEFFacade();

        public List<PurchaseOrderEF> getAllPurchaseOrders()
        {
           List<PurchaseOrderEF> poList = purchaseEFF.FindAllPurchaseOrders();

            var pendingDelivery = from p in poList
                                  where p.Status == "Pending Delivery"
                                  orderby p.OrderDate
                                  select p;

            var otherPO = from p in poList
                          where p.Status != "Pending Delivery"
                          orderby p.OrderDate
                          select p;
            List<PurchaseOrderEF> finalList = pendingDelivery.ToList();
            foreach (PurchaseOrderEF po in otherPO)
                finalList.Add(po);

            return finalList;
        }

        public PurchaseOrderEF FindPOById(int id)
        {
            return purchaseEFF.FindPurchaseOrderByOrderId(id);
        }

        public List<PurchaseOrderDetailsEF> FindPODetailsByOrderId(int id)
        {
            return purchaseEFF.FindPurchaseOrderDetailsByOrderId(id);
        }

        public List<SupplierDetailsEF> FindSupplierItems(string supplierCode)
        {
            return purchaseEFF.FindAllSupplierDetailsBySupplierCode(supplierCode);
        }

        public List<PurchaseOrderEF> SearchPurchaseOrder(string search, List<PurchaseOrderEF> poList)
        {
            List<PurchaseOrderEF> searchRes = new List<PurchaseOrderEF>();

            if(Int32.TryParse(search, out int result))
            {
                searchRes = poList.Where(p => p.OrderId == result).ToList<PurchaseOrderEF>();
            }else
            {
                string query = search.ToLower();
                foreach (PurchaseOrderEF p in poList)
                {
                    string tempStatus = p.Status.ToLower();
                    string tempDesc = p.Description.ToLower();
                    string tempSupplier = p.Supplier.SupplierCode.ToLower();
                    if (tempStatus.Contains(query) || tempDesc.Contains(query) || tempSupplier.Contains(query))
                    {
                        searchRes.Add(p);
                    }
                }
            }
            return searchRes;
        }

        public int CreatePO(StaffEF staff, PurchaseOrderFormDTO poForm)
        {
            PurchaseOrderEF po = new PurchaseOrderEF();

            po.CreatedById = staff.StaffId;
            po.OrderDate = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            po.DeliverByDate = (long)(poForm.SupplyItemBy.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            po.SupplierCode = poForm.SupplierId;
            po.DeliveryAddress = poForm.DeliveryAdd;
            po.Description = poForm.Description;
            po.Status = "Pending Delivery";

            po.OrderId = purchaseEFF.FindLastPOId();
            purchaseEFF.AddToPurchaseOrder(po);

            for(int i = 0; i<poForm.SupplierDetailIds.Count(); i++)
            {
                PurchaseOrderDetailsEF podet = new PurchaseOrderDetailsEF();                
                podet.OrderId = po.OrderId;
                podet.ItemCode = poForm.Icodes[i];
                podet.QuantityOrdered = poForm.Quantities[i];

                purchaseEFF.AddToPurchaseOrderDetails(podet);
            }

            return po.OrderId;
        }

        public void UpdatePurchaseOrderToDelivered(StaffEF staff, PurchaseOrderEF po)
        {
            po.DateDeliveredOn = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            po.ReceivedById = staff.StaffId;
            po.Status = "Delivered";

            purchaseEFF.SavePurchaseOrder(po);
        }

        public void UpdatePurchaseOrderToDeleted(PurchaseOrderEF po)
        {
            po.Status = "Deleted";
            purchaseEFF.SavePurchaseOrder(po);
        }


        //SUPPLIER

        public List<SupplierEF> FindAllSuppliers()
        {
            return purchaseEFF.FindAllSuppliers().OrderBy(s => s.SupplierCode).ToList();
        }
        public SupplierEF FindSupplierBySupplierCode(string code)
        {
            return purchaseEFF.FindSupplierBySupplierCode(code);
        }

        public List<SupplierEF> FindSupplierByItemCodeOrderByRank(string itemCode)
        {
            List<SupplierDetailsEF> sortedDetails = purchaseEFF.FindSupplierDetailsByItemCode(itemCode)
                .OrderBy(a => a.SupplierRank).ToList();
            List<SupplierEF> suppliers = new List<SupplierEF>();
            foreach (SupplierDetailsEF d in sortedDetails)
            {
                SupplierEF supplier = purchaseEFF.FindSupplierBySupplierCode(d.Supplier.SupplierCode);
                suppliers.Add(supplier);
            }
            return suppliers;
        }

        public bool AddNewSupplier(SupplierDTO supDto)
        {
            SupplierEF sup = new SupplierEF();
            sup.SupplierCode = supDto.SupplierCode;
            sup.SupplierName = supDto.SupplierName;
            sup.ContactName = supDto.Contact;
            sup.Address = supDto.Address;
            sup.GstRegistrationNo = supDto.GstNumber;
            sup.PhoneNo = supDto.PhoneNo;
            sup.FaxNo = supDto.FaxNo;

            return purchaseEFF.AddToSupplier(sup);
        }

        public void EditSupplier(SupplierDTO supDto)
        {
            SupplierEF sup = purchaseEFF.FindSupplierBySupplierCode(supDto.SupplierCode);

            sup.ContactName = supDto.Contact;
            sup.Address = supDto.Address;
            sup.GstRegistrationNo = supDto.GstNumber;
            sup.PhoneNo = supDto.PhoneNo;
            sup.FaxNo = supDto.FaxNo;

            purchaseEFF.UpdateSupplier(sup);

        }
    }
}