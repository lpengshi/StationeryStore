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
    [DepartmentFilter]
    [DepartmentHeadFilter]
    public class ViewEmployeeRequestHistoryController : Controller
    {
        StaffService staffService = new StaffService();
        RequestAndDisburseService rndService = new RequestAndDisburseService();

        [HttpGet]
        public ActionResult Index()
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            List<StationeryRequestEF> requestList = rndService.FindRequestByDepartmentAndStatus(staff.Department, "all");
            List<string> requestDate = rndService.ConvertToDate(requestList);

            ViewBag.requestList = requestList;
            ViewBag.requestDate = requestDate;
            ViewBag.departmentStaff = staffService.FindAllEmployeeByDepartmentCode(staff.DepartmentCode);

            return View();
        }

        [HttpPost]
        public ActionResult Index(string staffId)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            List<StationeryRequestEF> requestList = rndService.FindRequestByDepartmentAndStatus(staff.Department, "all");
            List<string> requestDate = rndService.ConvertToDate(requestList);

            if (staffId == "All Staff")
            {
                return RedirectToAction("Index");
            } else
            {
                int selectedStaff = int.Parse(staffId);
                List<StationeryRequestEF> staffList = new List<StationeryRequestEF>();
                foreach (StationeryRequestEF request in requestList)
                {
                    if (request.Staff.StaffId == selectedStaff)
                    {
                        staffList.Add(request);
                    }
                }
                requestList = staffList;
                ViewBag.selectedStaff = selectedStaff;
            }

            ViewBag.requestList = requestList;
            ViewBag.requestDate = requestDate;
            ViewBag.departmentStaff = staffService.FindAllEmployeeByDepartmentCode(staff.DepartmentCode);

            return View();
        }

        public ActionResult ViewRequest(string requestId)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            StationeryRequestEF request = rndService.FindRequestById(requestId);
            List<StationeryRequestDetailsEF> requestDetails = rndService.FindRequestDetailsByRequestId(requestId);

            if (request.Staff.DepartmentCode != staff.DepartmentCode)
            {
                return RedirectToAction("Index");
            }

            if (request.Status == "Submitted")
            {
                return RedirectToAction("ViewRequest", "ApproveRequest", new { requestId });
            }

            ViewBag.request = request;
            ViewBag.requestDetails = requestDetails;

            ViewBag.requestdate = Timestamp.dateFromTimestamp(request.RequestDate);

            long decisionDate = request.DecisionDate.GetValueOrDefault();
            ViewBag.decisiondate = Timestamp.dateFromTimestamp(decisionDate);

            return View();
        }
    }
}