using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Filters;
using StationeryStore.Models;
using StationeryStore.Service;


namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    [StoreFilter]
    public class ManageSupplierController : Controller
    {
        PurchaseService purchaseService = new PurchaseService();
        StaffService staffService = new StaffService();

        //Supplier List
        public ActionResult Index()
        {
            StaffEF staff = staffService.GetStaff();
            string staffRole = staff.Role.Description;
            List<SupplierEF> suppliers = purchaseService.FindAllSuppliers();
            ViewBag.suppliers = suppliers;
            ViewBag.staffRole = staffRole;
            return View();
        }

        public ActionResult ViewSupplier(string supplierCode, string decision)
        {
            SupplierEF supplier = purchaseService.FindSupplierBySupplierCode(supplierCode);
            ViewBag.supplier = supplier;
            StaffEF staff = staffService.GetStaff();
            ViewBag.staffRole = staff.Role.Description;

            if (decision == "edit")
            {
                return RedirectToAction("EditSupplier", "ManageSupplier", new { supplierCode = supplier.SupplierCode });
            }
            return View();
        }

        public ActionResult ViewSupplierDetails(string supplierCode, int page)
        {
            SupplierEF supplier = purchaseService.FindSupplierBySupplierCode(supplierCode);
            List<SupplierDetailsEF> allDetailsList = purchaseService.FindSupplierItems(supplierCode);

            int pageSize = 8;
            List<SupplierDetailsEF> details = allDetailsList
                .OrderBy(x => x.ItemCode)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList<SupplierDetailsEF>();

            int noOfPages = (int)Math.Ceiling((double)allDetailsList.Count() / pageSize);

            ViewData["supplier"] = supplier;
            ViewData["page"] = page;
            ViewData["supplierDetails"] = details;
            ViewData["noOfPages"] = noOfPages;
            return View();
        }

        [StoreManagerFilter]
        public ActionResult CreateSupplier(SupplierDTO supplierForm, string decision)
        {

            if (decision == "Create Supplier")
            {
                supplierForm.SupplierCode = supplierForm.SupplierCode.ToUpper();
                bool success = purchaseService.AddNewSupplier(supplierForm);
                if (!success)
                {
                    ViewBag.SavingError = "Supplier code already exists!";
                    return View();
                }
                else if (success)
                {
                    return RedirectToAction("ViewSupplier", "ManageSupplier", new { supplierCode = supplierForm.SupplierCode });
                }
            }
            return View();
        }

        [StoreManagerFilter]
        public ActionResult EditSupplier(string supplierCode, SupplierDTO supplierForm, string decision)
        {
            SupplierEF baseSupplier = purchaseService.FindSupplierBySupplierCode(supplierCode);
            ViewBag.supplier = baseSupplier;

            supplierForm.SupplierCode = baseSupplier.SupplierCode;
            supplierForm.SupplierName = baseSupplier.SupplierName;

            if(decision == "Confirm Changes")
            {
                purchaseService.EditSupplier(supplierForm);
                return RedirectToAction("ViewSupplier", "ManageSupplier", new { supplierCode = supplierCode });
            }
            return View();
        }

        [StoreManagerFilter]
        [HttpGet]
        public ActionResult EditSupplierDetails(string supplierCode, List<SupplierDetailsEF> editedItems)
        {
            SupplierEF supplier = purchaseService.FindSupplierBySupplierCode(supplierCode);
            List<SupplierDetailsEF> supplierItems = purchaseService.FindSupplierItems(supplierCode);

            ViewData["supplier"] = supplier;            
            ViewData["supplierItems"] = supplierItems;            

            return View(editedItems);
        }

        [StoreManagerFilter]
        [HttpPost]
        public ActionResult EditSupplierDetails(List<SupplierDetailsEF> editedItems, string choice, string itemToAddCode, string supplierCode)
        {
            SupplierEF supplier = purchaseService.FindSupplierBySupplierCode(supplierCode);
            List<SupplierDetailsEF> supplierItems = purchaseService.FindSupplierItems(supplierCode);
            ViewData["supplier"] = supplier;
            ViewData["supplierItems"] = supplierItems;

            if(editedItems == null)
            {
                editedItems = new List<SupplierDetailsEF>();
            }

            if (choice == "Add Item")
            {
                bool isValid = false;
                SupplierDetailsEF newItem = new SupplierDetailsEF();

                //check if exists in the supplier list of items
                foreach(var item in supplierItems)
                {
                    if (itemToAddCode == item.ItemCode)
                    {
                        newItem = item;
                        isValid = true;
                    }
                }
                //check for duplicate entry
                foreach(var item in editedItems)
                {
                    if(itemToAddCode == item.ItemCode)
                    {
                        isValid = false;
                    }
                }
                if (isValid)
                {
                    editedItems.Add(newItem);
                }
            }
            if(choice == "Submit")
            {
                purchaseService.AmendSupplierDetails(editedItems);
                return RedirectToAction("ViewSupplierDetails", "ManageSupplier", new {page = 1, supplierCode= supplierCode });
            }

            ModelState.Clear();
            return View(editedItems);
        }       
    }
}