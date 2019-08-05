using StationeryStore.Models;
using StationeryStore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Util;

namespace StationeryStore.Controllers
{
    public class MobileController : Controller
    {
        RequestAndDisburseService rndService = new RequestAndDisburseService();
        StockService stockService = new StockService();

        // Send retrieval details to android
        public JsonResult GetRetrieval()
        {
            // find retrieval where status = Processed
            StationeryRetrievalEF retrieval = rndService.FindRetrievalByStatus("Processing");
            // get the retrieval details
            List<RetrievalItemDTO> details = rndService.ViewRetrievalListById(retrieval.RetrievalId);
            // send the retrieval list over to android app
            MobileRetrievalItemDTO mRetrieval = new MobileRetrievalItemDTO()
            {
                RetrievalId = retrieval.RetrievalId,
                DateDisbursed = DateTimeOffset.UtcNow.AddDays(3),
                RetrievalItems = details
            };
            return Json(retrieval, JsonRequestBehavior.AllowGet);
        }

        // Receive retrieval details from android
        public JsonResult SetRetrieval(MobileRetrievalItemDTO mRetrieval)
        {
            if (mRetrieval != null)
            {
                // save it to the database
                // update individual request's fulfilled quantity and retrieved quantity in current retrieval
                rndService.UpdateRequestAfterRetrieval(mRetrieval.RetrievalItems, mRetrieval.RetrievalId);

                // update retrieval status from processing to retrieved
                StationeryRetrievalEF retrieval = rndService.FindRetrievalById(mRetrieval.RetrievalId);
                retrieval.Status = "Retrieved";
                rndService.SaveRetrieval(retrieval);

                // get disbursements and save the dates
                rndService.UpdateDisbursementDate(mRetrieval.RetrievalId, mRetrieval.DateDisbursed);

                // update disbursement list
                rndService.UpdateRetrievedQuantities(mRetrieval.RetrievalId);

                // and log stock transaction (deduction for department) (StockService)
                stockService.LogTransactionsForRetrieval(mRetrieval.RetrievalId);
                return Json(new { status = "ok" });
            }
            return Json(new { status = "fail" });
        }

        // Send disbursement details to android
        //public JsonResult GetDisbursement()
        //{
        //    // Create a DTO for disbursement which includes storeClerkId/Name and EmployeeId/Name
        //    // send a list of disbursements 


        //}

        //    //Get disbursement details from android
        //public JsonResult SetDisbursement()
        //{
        //    // update disbursement details' Disbursed Quantity and disbursement status to disbursed
        //    // rndService.UpdateDisbursedQuantities(details, disbursementId, collectionRepId, storeClerkId);

        //    // update request details
        //    // rndService.UpdateRequestAfterDisbursement(details, disbursementId);

        //    // log any stock transaction (damaged goods) - compare retrievedQty with disbursedqty
        //    // stockService.LogTransactionsForActualDisbursement(disbursementId);

        //    // android side to send a notification to department rep

        //}

        //public JsonResult AcknowledgeDisbursement()
        //{
        //    // department rep to acknowledge 
        //    // system to update status of disbursement
        //}

    }
}