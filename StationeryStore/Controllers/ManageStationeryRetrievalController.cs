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
    public class ManageStationeryRetrievalController : Controller
    {
        // GET: ManageStationeryRetrieval

        RequestAndDisburseService rndService = new RequestAndDisburseService();
        
        
        // Displays the retrieval list history
        public ActionResult ViewRetrievalHistory(int page)
        {
            List<StationeryRetrievalEF> allRetrievals = rndService.FindAllRetrieval();

            int pageSize = 8;
            List<StationeryRetrievalEF> retrievals = allRetrievals
                .OrderByDescending(x => x.DateRetrieved)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList<StationeryRetrievalEF>();

            int noOfPages = (int)Math.Ceiling((double)allRetrievals.Count() / pageSize);
            ViewData["page"] = page;
            ViewData["retrievals"] = retrievals;
            ViewData["noOfPages"] = noOfPages;
            return View();
        }

        // View and amend retrieval
        public ActionResult ViewRetrieval(int retrievalId)
        {
            StationeryRetrievalEF retrieval = rndService.FindRetrievalById(retrievalId);
            ViewData["retrieval"] = retrieval;
            List<RetrievalItemDTO> details = rndService.ViewRetrievalListById(retrievalId).OrderBy(x => x.Bin).ToList();
            return View(details);
        }

        // Store clerk inserts actual retrieved quantities and saves it to the DB
        [HttpPost]
        public ActionResult SaveRetrieval(List<RetrievalItemDTO> items, int retrievalId, string decision, 
            DateTimeOffset disbursementDate)
        {
            if (decision == "Cancel")
            {
                return RedirectToAction("ViewRetrievalHistory", new { page = 1 });
            }

            // update individual request's fulfilled quantity and retrieved quantity in current retrieval
            rndService.UpdateRequestAfterRetrieval(items, retrievalId);

            // update retrieval status from processing to retrieved
            StationeryRetrievalEF retrieval = rndService.FindRetrievalById(retrievalId);
            retrieval.Status = "Retrieved";
            rndService.SaveRetrieval(retrieval);
            
            //get disbursements and save the dates
            rndService.UpdateDisbursementDate(retrievalId, disbursementDate);

            return RedirectToAction("GenerateDisbursementList", "GenerateDisbursementList", new { retrievalId });
        }
    }
}