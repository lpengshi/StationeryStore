﻿using System;
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
    [DepartmentHeadFilter]
    public class ManageDelegationController : Controller
    {
        DepartmentService deptService = new DepartmentService();
        StaffService staffService = new StaffService();

        [HttpGet]
        public ActionResult Index()
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            List<StaffEF> deptStaff = staffService.FindAllEmployeeByDepartmentCode(staff.DepartmentCode);
            ViewBag.deptStaff = deptStaff;
            ViewBag.department = staff.Department;
            //retrieve current delegation
            ManageDelegationDTO manageDelegationDTO = new ManageDelegationDTO();
            manageDelegationDTO.DelegationStartDate = DateTime.UtcNow;
            manageDelegationDTO.DepartmentCode = staff.DepartmentCode;

            return View(manageDelegationDTO);
        }

        [HttpPost]
        public ActionResult Index(ManageDelegationDTO manageDelegationDTO, string decision)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;

            List<StaffEF> deptStaff = staffService.FindAllEmployeeByDepartmentCode(staff.DepartmentCode);
            ViewBag.deptStaff = deptStaff;
            ViewBag.department = staff.Department;
            manageDelegationDTO.DelegationStartDate = DateTime.UtcNow;

            //update delegation and validate delegation start and end date
            if (decision == "Add Delegation") {
                if (staff.Department.AuthorityId != staff.StaffId)
                {
                    ViewBag.note = "Please remove existing delegation before adding a new one";

                    return View(manageDelegationDTO);
                }

                if (manageDelegationDTO.DelegationEndDate.Date.CompareTo(manageDelegationDTO.DelegationStartDate.Date) < 0)
                {
                    ViewBag.note = "Delegation End Date cannot be earlier than Delegation Start Date";

                    return View(manageDelegationDTO);
                }

                deptService.DelegateStaff(manageDelegationDTO);
            }
            else if (decision == "Remove Delegation")
            {
                deptService.RemoveStaffDelegation(staff);
            }
          
            return RedirectToAction("Index");
        }
    }
}