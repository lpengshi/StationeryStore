using StationeryStore.Filters;
using StationeryStore.Models;
using StationeryStore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    [StoreFilter]
    public class ViewOutstandingRequestsController : Controller
    {

        RequestAndDisburseService rndService = new RequestAndDisburseService();
        

        // GET: ViewOutstandingRequests
        public ActionResult ViewOutstandingRequests(int page)
        {
            List<StationeryRequestDetailsEF> requests = rndService.FindOutstandingRequestDetails().OrderByDescending(x => x.FulfilmentStatus).ToList();
            
            // disable retrieval button if there is an ongoing retrieval/disbursement
            bool ongoingRetrieval = rndService.FindOngoingRetrieval();
            bool ongoingDisbursement = rndService.FindOngoingDisbursement();
            if (ongoingRetrieval || ongoingDisbursement)
            {
                ViewData["error"] = "error";
            }

            int pageSize = 8;
            List<StationeryRequestDetailsEF> outstandingRequests = requests
                .OrderBy(x => x.RequestDetailsId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList<StationeryRequestDetailsEF>();

            int noOfPages = (int)Math.Ceiling((double)requests.Count() / pageSize);
            ViewData["page"] = page;
            ViewData["outstandingRequests"] = outstandingRequests;
            ViewData["noOfPages"] = noOfPages;
            return View();
        }
    }
}