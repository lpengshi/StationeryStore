using StationeryStore.Models;
using StationeryStore.Service;
using StationeryStore.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StationeryStore.Controllers
{
    public class GenerateRetrievalListController : Controller
    {
        // GET: GenerateRetrievalList
        RequestAndDisburseService rndService = new RequestAndDisburseService();

        // Consolidates all outstanding requests by stationery item
        // Saves the disbursement list to the DB
        public ActionResult GenerateRetrievalList()
        {
            // Create StationeryRetrieval
            StationeryRetrievalEF retrieval = new StationeryRetrievalEF()
            {
                DateRetrieved = Timestamp.unixTimestamp(),
                Status = "Processing"
            };
            rndService.CreateRetrieval(retrieval);

            //Insert each dept's request quantity per item into disbursement and disbursement details
            List<StationeryRequestDetailsEF> outstandingRequests = rndService.FindOutstandingRequestDetails();
            rndService.GenerateDisbursementList(outstandingRequests, retrieval.RetrievalId);

            // set all the request details status to processed
            foreach (StationeryRequestDetailsEF detail in outstandingRequests)
            {
                detail.FulfilmentStatus = "Processed";
                rndService.UpdateRequestDetails(detail);
            }

            return RedirectToAction("ViewRetrieval", "ManageStationeryRetrieval", new { retrievalId = retrieval.RetrievalId });
        }
    }
}