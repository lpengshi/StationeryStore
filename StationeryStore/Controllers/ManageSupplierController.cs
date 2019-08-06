using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Models;
using StationeryStore.Service;
using StationeryStore.Filters;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    public class ManageSupplierController : Controller
    {
        PurchaseService purchaseService = new PurchaseService();
        StaffService staffService = new StaffService();

        //Supplier List
        public ActionResult Index()
        {
            List<SupplierEF> suppliers = purchaseService.FindAllSuppliers();
            ViewBag.suppliers = suppliers;
            return View();
        }

        public ActionResult ViewSupplier(string supplierCode, string decision)
        {
            SupplierEF supplier = purchaseService.FindSupplierBySupplierCode(supplierCode);
            ViewBag.supplier = supplier;

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

        public ActionResult AssignItemRank(List<SupplierDetailsEF> editedItems, string choice)
        {

            return View();
        }
    }
}