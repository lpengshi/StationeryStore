using System;
using System.Collections.Generic;
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
        // GET: ManageSupplier
        //Supplier List
        public ActionResult Index()
        {
            List<SupplierEF> suppliers = purchaseService.FindAllSuppliers();
            ViewData["suppliers"] = suppliers;

            StaffEF staff = staffService.GetStaff();
            ViewData["staff"] = staff;
            return View();
        }

        public ActionResult ViewSupplier(string supplierCode, string choice)
        {
            SupplierEF supplier = purchaseService.FindSupplierBySupplierCode(supplierCode);
            ViewData["supplier"] = supplier;
            //Staff staff = staffsvc.GetStaff();
            if (choice == "edit" /* && staff.role.rolename? == "manager" */)
            {
                return RedirectToAction("EditSupplier", "ManageSupplier", new { supplierCode = supplier.SupplierCode });
            }
            return View();
        }

        public ActionResult CreateSupplier(SupplierDTO supplierForm, string choice)
        {

            if(choice == "Create Supplier")
            {
                purchaseService.AddNewSupplier(supplierForm);
                return RedirectToAction("ViewSupplier", "ManageSupplier", new { supplierCode = supplierForm.SupplierCode });
            }
            return View();
        }      

        
        public ActionResult EditSupplier(string supplierCode, SupplierDTO supplierForm, string choice)
        {
            //cannot amend supplier code/supplier name

            SupplierEF baseSupplier = purchaseService.FindSupplierBySupplierCode(supplierCode);
            ViewData["baseSupplier"] = baseSupplier;

            supplierForm.SupplierCode = baseSupplier.SupplierCode;
            supplierForm.SupplierName = baseSupplier.SupplierName;

            if(choice == "Edit Supplier")
            {
                purchaseService.EditSupplier(supplierForm);
                return RedirectToAction("ViewSupplier", "ManageSupplier", new { supplierCode = baseSupplier.SupplierCode });
            }

            return View();
        }
    }
}