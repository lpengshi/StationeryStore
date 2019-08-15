using StationeryStore.Filters;
using StationeryStore.Models;
using StationeryStore.Service;
using StationeryStore.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    [StoreFilter]
    public class ManageStationeryDisbursementController : Controller
    {
        RequestAndDisburseService rndService = new RequestAndDisburseService();
        StaffService staffService = new StaffService();
        StockService stockService = new StockService();

        public ActionResult ViewDisbursementHistory(int page)
        {
            List<StationeryDisbursementEF> fullDisbursements = rndService.FindAllDisbursements();
            
            int pageSize = 8;
            List<StationeryDisbursementEF> disbursements = fullDisbursements
                .OrderByDescending(x => x.DateDisbursed)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList<StationeryDisbursementEF>();

            int noOfPages = (int)Math.Ceiling((double)fullDisbursements.Count() / pageSize);
            ViewData["page"] = page;
            ViewData["disbursements"] = disbursements;
            ViewData["noOfPages"] = noOfPages;
            return View();
        }

        public ActionResult ViewDisbursement(int disbursementId)
        {
            StationeryDisbursementEF disbursement = rndService.FindDisbursementById(disbursementId);
            ViewData["disbursement"] = disbursement;
            List<StationeryDisbursementDetailsEF> details = rndService.FindDisbursementDetailsByDisbursementId(disbursementId);
            // list of staff in that department
            List<StaffEF> deptStaff = staffService.FindAllEmployeeByDepartmentCode(disbursement.DepartmentCode);
            ViewData["deptStaff"] = deptStaff;
            StaffEF storeClerk = staffService.GetStaff();
            ViewData["storeClerk"] = storeClerk;
            return View(details);
        }

        [HttpPost]
        public ActionResult SaveDisbursement(List<StationeryDisbursementDetailsEF> details, int disbursementId,
            string decision, int collectionRepId, int storeClerkId)
        {
            if (decision == "Cancel")
            {
                return RedirectToAction("ViewDisbursementHistory", new { page = 1 });
            }
            // update disbursement details' Disbursed Quantity and disbursement status to disbursed
            rndService.UpdateDisbursedQuantities(details, disbursementId, collectionRepId, storeClerkId);

            // update request details
            rndService.UpdateRequestAfterDisbursement(details, disbursementId);

            // log any stock transaction (damaged goods) - compare retrievedQty with disbursedqty
            stockService.LogTransactionsForActualDisbursement(disbursementId);

            // email collection rep for acknowledgement of disbursement
            string collectionRepEmail = staffService.FindStaffById(collectionRepId).Email;
            string subject = "Disbursement #" + disbursementId + " : Request for Acknowledgement";
            string body = "Disbursement #" + disbursementId + " has been disbursed. Please click " +
                "<a href='http://localhost/StationeryStore/ViewDisbursement/ViewDisbursement/?disbursementId=" + disbursementId + "'>" +
                "here</a> to view the details of the disbursement and acknowledge receipt of stationery item(s).";
            Email.SendEmail(collectionRepEmail, subject, body);

            return RedirectToAction("ViewDisbursement", new { disbursementId });
        }
    }
}