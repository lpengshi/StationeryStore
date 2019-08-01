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
            
            return RedirectToAction("ViewDisbursementHistory", "ManageStationeryDisbursement", new { page = 1 });
        }
    }
}