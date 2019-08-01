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
    public class ManageCollectionController : Controller
    {
        StaffService staffService = new StaffService();
        DepartmentService deptService = new DepartmentService();

        public ActionResult Index()
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            DepartmentEF dept = staff.Department;
            ViewBag.deptStaff = staffService.FindAllStaffByDepartmentCode(dept.DepartmentCode);

            return View(dept);
        }
    }
}