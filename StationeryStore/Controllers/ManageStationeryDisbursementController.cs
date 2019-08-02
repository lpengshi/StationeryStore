using StationeryStore.Models;
using StationeryStore.Service;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace StationeryStore.Controllers
{
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
            List<StaffEF> deptStaff = staffService.FindAllStaffByDepartmentCode(disbursement.DepartmentCode);
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

            return RedirectToAction("SendEmailForAcknowledgement", new { disbursementId });
        }

        public ActionResult SendEmailForAcknowledgement(int disbursementId)
        {
            ////get the collection rep for this disbursement
            //StationeryDisbursementEF disbursement = rndService.FindDisbursementById(disbursementId);
            //string collectionRepEmail = disbursement.Staff.Username + "@LogicUniversity";
            //System.Net.NetworkCredential credentials =
            //    new System.Net.NetworkCredential("sa48team5@gmail.com", "passTeam5word");
            //SmtpClient client = new SmtpClient()
            //{
            //    Host = "smtp.gmail.com",
            //    Port = 587,
            //    EnableSsl = true,
            //    UseDefaultCredentials = false,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    Credentials = credentials
            //};

            //MailMessage mm = new MailMessage("sa48team5@gmail.com", collectionRepEmail)
            //{
            //    // change the link once published
            //    Subject = "Disbursement #" + disbursementId + " : Request for Acknowledgement",
            //    Body = "Disbursement #" + disbursementId + " has been disbursed. Please click " +
            //    "<a href='localhost:56413/ManageStationeryDisbursement/ViewDisbursement/?disbursementId=" + disbursementId + "'>" +
            //    "here</a> to view the details of the disbursement and acknowledge receipt of stationery item(s)."
            //};
            //mm.IsBodyHtml = true;
            //try
            //{
            //    client.Send(mm);
            //}
            //catch (Exception e)
            //{
            //    Debug.Print(e.Message);
            //}

            return RedirectToAction("ViewDisbursement", new { disbursementId });
        }
    }
}