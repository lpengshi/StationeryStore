using StationeryStore.Models;
using StationeryStore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Util;
using System.Net.Mail;
using System.Diagnostics;
using StationeryStore.Filters;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    [StoreFilter]
    public class GenerateDisbursementListController : Controller
    {
        RequestAndDisburseService rndService = new RequestAndDisburseService();
        StockService stockService = new StockService();

        // GET: GenerateDisbursementList
        public ActionResult GenerateDisbursementList(int retrievalId)
        {
            //update disbursement list
            rndService.UpdateRetrievedQuantities(retrievalId);

            // and log stock transaction (deduction for department) (StockService)
            stockService.LogTransactionsForRetrieval(retrievalId);

            //send email to collection rep to notify of collection
            //get the collection rep for this disbursement
            List<StationeryDisbursementEF> disbursements = rndService.FindDisbursementsByRetrievalId(retrievalId);
            foreach(StationeryDisbursementEF d in disbursements)
            {
                string collectionRepEmail = d.Staff.Email;
                System.Net.NetworkCredential credentials =
                    new System.Net.NetworkCredential("sa48team5@gmail.com", "passTeam5word");
                SmtpClient client = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = credentials
                };

                MailMessage mm = new MailMessage("sa48team5@gmail.com", collectionRepEmail)
                {
                    Subject = "Disbursement #" + d.DisbursementId + " : Ready for Collection",
                    Body = "Disbursement #" + d.DisbursementId + " is ready for collection on " + Timestamp.dateFromTimestamp((long)d.DateDisbursed)
                        + " " + d.Staff.Department.CollectionPoint.CollectionTime + ", at " + d.Staff.Department.CollectionPoint.Location + "."
                };
                mm.IsBodyHtml = true;
                try
                {
                    client.Send(mm);
                }
                catch (Exception e)
                {
                    Debug.Print(e.Message);
                }
            }
            
            return RedirectToAction("ViewDisbursementHistory", "ManageStationeryDisbursement", new { page = 1 });
        }
    }
}