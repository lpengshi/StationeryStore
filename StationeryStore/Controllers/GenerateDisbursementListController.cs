using StationeryStore.Models;
using StationeryStore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Util;
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
            List<StationeryDisbursementEF> disbursements = rndService.FindDisbursementsByRetrievalId(retrievalId);
            foreach(StationeryDisbursementEF d in disbursements)
            {
                if(d.Status == "Retrieved")
                {
                    string collectionRepEmail = "janel.leejq@gmail.com"; //d.Staff.Email;
                    string subject = "Disbursement #" + d.DisbursementId + " : Ready for Collection";
                    string body = "Disbursement #" + d.DisbursementId + " is ready for collection on " + Timestamp.dateFromTimestamp((long)d.DateDisbursed)
                        + " " + d.Staff.Department.CollectionPoint.CollectionTime + ", at " + d.Staff.Department.CollectionPoint.Location + ".";
                    Email.SendEmail(collectionRepEmail, subject, body);
                }
            }
            
            return RedirectToAction("ViewDisbursementHistory", "ManageStationeryDisbursement", new { page = 1 });
        }
    }
}