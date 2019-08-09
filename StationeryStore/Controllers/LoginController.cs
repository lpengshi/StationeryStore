﻿using System.Web.Mvc;
using StationeryStore.Models;
using StationeryStore.Service;
using StationeryStore.Util;

namespace StationeryStore.Controllers
{
    public class LoginController : Controller
    {
        StaffService staffService = new StaffService();
        DepartmentService deptService = new DepartmentService();

        [HttpGet]
        public ActionResult Index()
        {
            ViewData["note"] = "Please enter your Username and Password";



            return View();
        }

        [HttpPost]
        public ActionResult Index (LoginDTO loginDTO)
        {

            StaffEF staff = staffService.FindStaffByUsername(loginDTO.Username);
            
            if (staff == null || staff.Password != loginDTO.Password)
            {

                ViewData["note1"] = "Username or Password is incorrect. Please try again.";
                return View();
            }
            else {
                string sessionId = staffService.CreateSession(staff);
                Session["sessionId"] = sessionId;
                Session["staff"] = staff;
                if (staff.Department != null)
                {
                    deptService.CheckDelegation(staff.Department);
                }

                if (staff.Role.Description == "Store Clerk" || staff.Role.Description == "Store Supervisor")
                {
                    return RedirectToAction("ViewLowStock", "ViewLowStock", new { page = 1 });
                }
                if (staff.Role.Description == "Store Manager")
                {
                    return RedirectToAction("ViewAllStocks", "ManageStockCard", new { page = 1 });
                }
                if (staff.Role.Description == "Department Head" || staff.Department.AuthorityId == staff.StaffId)
                {
                    return RedirectToAction("Index", "ApproveRequest");
                }
                if (staff.Role.Description == "Employee")
                {
                    return RedirectToAction("Index", "ManageRequest");
                }
            }
            return View();
        }
    }
}