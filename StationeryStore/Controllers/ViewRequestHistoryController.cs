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
    public class ViewRequestHistoryController : Controller
    {
        StaffService staffService = new StaffService();
        RequestAndDisburseService rndService = new RequestAndDisburseService(); 

        public ActionResult Index()
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            List<RequestDTO> requestDTOList = rndService.FindRequestsByStaff(staff.StaffId);
            ViewBag.requestDTOList = requestDTOList;

            return View();
        }
    }
}