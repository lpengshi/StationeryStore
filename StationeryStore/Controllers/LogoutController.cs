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
    public class LogoutController : Controller
    {
        StaffService staffService = new StaffService();

        public ActionResult Index()
        {
            StaffEF staff = staffService.GetStaff();
            staffService.Logout(staff);
            Session["sessionId"] = null;

            return RedirectToAction("Index", "Login");
        }
    }
}