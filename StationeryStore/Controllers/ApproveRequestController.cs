using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Filters;
using StationeryStore.Models;
using StationeryStore.Service;
using StationeryStore.Util;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    public class ApproveRequestController : Controller
    {
        StaffService staffService = new StaffService();
        RequestAndDisburseService rndService = new RequestAndDisburseService();

        public ActionResult Index()
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            List<StationeryRequestEF> pendingList = rndService.FindRequestByDepartmentAndStatus(staff.Department, "Submitted");
            List<string> requestDate = rndService.ConvertToDate(pendingList);

            ViewBag.pendingList = pendingList;
            ViewBag.requestDate = requestDate;

            return View();
        }

        [HttpGet]
        public ActionResult ViewRequest(string requestId)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            StationeryRequestEF request = rndService.FindRequestById(requestId);
            List<StationeryRequestDetailsEF> requestDetails = rndService.FindRequestDetailsByRequestId(requestId);

            ViewBag.requestDetails = requestDetails;
            ViewBag.request = request;
            ViewBag.date = Timestamp.dateFromTimestamp(request.RequestDate);

            return View();
        }

        [HttpPost]
        public ActionResult ApproveRequest(string requestId, string comment, string decision)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            StationeryRequestEF request = rndService.FindRequestById(requestId);
            List<StationeryRequestDetailsEF> requestDetails = rndService.FindRequestDetailsByRequestId(requestId);

            rndService.UpdateRequestDecision(staff, request, comment, decision);

            return RedirectToAction("Index");
        }
    }
}