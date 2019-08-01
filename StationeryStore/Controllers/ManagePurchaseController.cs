using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Filters;
using StationeryStore.Models;
using StationeryStore.Service;
using StationeryStore.Util;

namespace StationeryStore.Controllers
{
    public class ManagePurchaseController : Controller
    {
        PurchaseService purchaseService = new PurchaseService();
        StaffService staffService = new StaffService();

        //Purchase Order
        public ActionResult PurchaseOrderHistory(string search, string startDate, string endDate)
        {
            //PO status : PENDING DELIVERY, DELIVERED, DELETED
            List<PurchaseOrderEF> poList = purchaseService.getAllPurchaseOrders();

            bool dateOkay = false;
            long sDate = 0;
            long eDate = 0;
            if (startDate != null && endDate != null
                && DateTime.TryParse(startDate, out DateTime res)
                && DateTime.TryParse(endDate, out DateTime result))
            {
                DateTime startDate1 = res;
                DateTime endDate1 = result;
                if (startDate1 <= endDate1)
                {
                    dateOkay = true;
                    sDate = (long)(startDate1.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                    eDate = (long)(endDate1.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                }
            }
            if (!(search == null || search.Trim() == "") && dateOkay)
            {
                List<PurchaseOrderEF> list = purchaseService.SearchPurchaseOrder(search, poList);
                var finalList = list.Where(p => p.OrderDate >= sDate && p.OrderDate <= eDate).ToList();

                ViewData["purchaseOrders"] = (List<PurchaseOrderEF>)finalList;
                ViewData["search"] = search;
            }
            else if (!(search == null || search.Trim() == "") && !dateOkay)
            {
                List<PurchaseOrderEF> finalList = purchaseService.SearchPurchaseOrder(search, poList);
                ViewData["purchaseOrders"] = finalList;
                ViewData["search"] = search;
            }
            else if ((search == null || search.Trim() == "") && dateOkay)
            {
                var finalList = poList.Where(p => p.OrderDate >= sDate && p.OrderDate <= eDate).ToList();

                ViewData["purchaseOrders"] = (List<PurchaseOrderEF>)finalList;
                ViewData["search"] = search;
            }
            else
            {
                ViewData["purchaseOrders"] = poList;
            }
            return View();
        }

        [HttpGet]
        public ActionResult CreatePurchaseOrder(PurchaseOrderFormDTO po)
        {
            List<SupplierDetailsEF> supplierItems = new List<SupplierDetailsEF>();
            List<SupplierEF> supplierList = purchaseService.FindAllSuppliers();
            ViewData["supplierList"] = supplierList;
            ViewData["supplierItems"] = supplierItems;

            return View(po);
        }

        [HttpPost]
        public ActionResult CreatePurchaseOrder(string supplier, string choice, string items,
            PurchaseOrderFormDTO purOrder)
        {
            List<SupplierDetailsEF> supplierItems = new List<SupplierDetailsEF>();
            List<SupplierEF> supplierList = purchaseService.FindAllSuppliers();
            ViewData["supplierList"] = supplierList;

            //Staff createdBy = staffService.GetStaff();
            StaffEF createdBy = staffService.FindStaffByUsername("scienceemp1");
            if (supplier == null)
                supplierItems = null;
            else
            {
                supplierItems = purchaseService.FindSupplierItems(supplier);
            }
            ViewData["supplier"] = supplier;
            ViewData["supplierItems"] = supplierItems;


            if (choice == "setSupplier")
            {
                if (supplier == supplierList[0].SupplierCode)
                {
                    //do nothing when supplier is the same
                }
                else
                {
                    //clear form.
                    purOrder = new PurchaseOrderFormDTO();
                }
            }

            if (choice == "AddItem")
            {
                SupplierDetailsEF newSD = null;
                bool isValid = false;

                foreach (var s in supplierItems)
                {
                    if (items == s.SupplierDetailsId.ToString())
                    {
                        newSD = s;
                        isValid = true;
                    }
                }
                if (purOrder.SupplierDetailIds.Count() > 0 && isValid == true)
                {
                    foreach (var thing in purOrder.SupplierDetailIds)
                    {
                        if (thing == items)
                        {
                            isValid = false;
                        }
                    }
                }
                if (isValid == true)
                {
                    purOrder.Icodes.Add(newSD.Stock.ItemCode);
                    purOrder.Descs.Add(newSD.Stock.Description);
                    purOrder.Uoms.Add(newSD.Stock.Uom);
                    purOrder.Prices.Add(newSD.UnitPrice);
                    purOrder.SupplierDetailIds.Add(newSD.SupplierDetailsId.ToString());
                    purOrder.Quantities.Add(1);
                    purOrder.Remove.Add(false);
                }
            }

            if (choice == "CreatePO")
            {
                purOrder.SupplierId = supplierItems[0].SupplierCode;
                int newPOId = purchaseService.CreatePO(createdBy, purOrder);

                return RedirectToAction("ViewPurchaseOrder", "ManagePurchase", new {purchaseOrderId = newPOId.ToString() });

            }

            if (choice == "Remove")
            {
                for (int i = 0; i < purOrder.Remove.Count; i++)
                {
                    if (purOrder.Remove[i] == true)
                    {
                        purOrder.Icodes.RemoveAt(i);
                        purOrder.Descs.RemoveAt(i);
                        purOrder.Uoms.RemoveAt(i);
                        purOrder.Prices.RemoveAt(i);
                        purOrder.SupplierDetailIds.RemoveAt(i);
                        purOrder.Quantities.RemoveAt(i);
                        purOrder.Remove.RemoveAt(i);
                        i--;
                    }
                }
            }
            return View(purOrder);
        }

        public ActionResult ViewPurchaseOrder(string purchaseOrderId, string choice)
        {
            bool validId = Int32.TryParse(purchaseOrderId, out int id);
            PurchaseOrderEF po = purchaseService.FindPOById(id);
            List<PurchaseOrderDetailsEF> pod = purchaseService.FindPODetailsByOrderId(id);
            //Staff receivedBy = staffService.GetStaff();
            StaffEF receivedBy = staffService.FindStaffByUsername("scienceemp1");
            ViewData["purchaseOrder"] = po;
            ViewData["purchaseOrderDetails"] = pod;

            if(choice == "Confirm Delivery" && po.Status == "Pending Delivery")
            {
                purchaseService.UpdatePurchaseOrderToDelivered(receivedBy, po);
                return RedirectToAction("PurchaseOrderHistory", "ManagePurchase");
            }

            return View();
        }

    }
}
