using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Filters;
using StationeryStore.Models;
using StationeryStore.Service;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    [DepartmentFilter]
    public class ViewDisbursementController : Controller
    {
        StaffService staffService = new StaffService();
        RequestAndDisburseService rndService = new RequestAndDisburseService();

        public ActionResult Index()
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            List<StationeryDisbursementEF> disbursementList = rndService.FindDisbursementByDepartmentCode(staff.DepartmentCode);
            List<string> disbursementDate = new List<string>();

            foreach (StationeryDisbursementEF disbursement in disbursementList)
            {
                string date = Util.Timestamp.dateFromTimestamp(disbursement.DateDisbursed.Value);
                disbursementDate.Add(date);
            }

            ViewBag.disbursementList = disbursementList;
            ViewBag.disbursementDate = disbursementDate;

            return View();
        }

        [HttpGet]
        public ActionResult ViewDisbursement(int disbursementId)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            StationeryDisbursementEF disbursement = rndService.FindDisbursementById(disbursementId);
            List<StationeryDisbursementDetailsEF> disbursementDetails = rndService.FindDisbursementDetailsByDisbursementId(disbursementId);

            ViewBag.disbursement = disbursement;
            ViewBag.disbursementDetails = disbursementDetails;
            return View();
        }

        [HttpPost]
        public ActionResult ViewDisbursement(int disbursementId, string decision)
        {
            if(decision == "Back")
            {
                return RedirectToAction("Index");
            }
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            StationeryDisbursementEF disbursement = rndService.FindDisbursementById(disbursementId);
            rndService.UpdateDisbursementStatus(disbursement);

            return RedirectToAction("ViewDisbursement", new { disbursementId });
        }

    }
}