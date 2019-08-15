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
    [CurrentAuthorityFilter]
    public class ManageCollectionController : Controller
    {
        StaffService staffService = new StaffService();
        DepartmentService deptService = new DepartmentService();
        
        [HttpGet]
        public ActionResult Index(string update)
        {
            if (update == "success")
            {
                ViewBag.note = "Department collection details has been updated";
            } else if (update == "unchanged")
            {
                ViewBag.note = "No changes were made to the collection details";
            }

            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            ViewBag.department = staff.Department;
            List<StaffEF> deptStaff = staffService.FindAllEmployeeByDepartmentCode(staff.DepartmentCode);
            List<CollectionPointEF> collectionPoints = deptService.FindAllCollectionPoints();

            ManageCollectionDTO collectDTO = new ManageCollectionDTO();

            collectDTO.Department = staff.Department.DepartmentCode;
            collectDTO.CollectionPoints = collectionPoints;
            ViewBag.deptStaff = deptStaff;

            return View(collectDTO);
        }

        [HttpPost]
        public ActionResult Index(ManageCollectionDTO manageCollectionDTO)
        {
            StaffEF staff = staffService.GetStaff();
            DepartmentEF department = staff.Department;

            if (manageCollectionDTO.DepartmentRepId == department.DepartmentRepresentativeId && manageCollectionDTO.CollectionPointId == department.CollectionPointId)
            {
                return RedirectToAction("Index", new { update = "unchanged" });
            } else if (manageCollectionDTO.CollectionPointId == department.CollectionPointId && manageCollectionDTO.DepartmentRepId == null)
            {
                return RedirectToAction("Index", new { update = "unchanged" });
            }

            deptService.UpdateDepartmentCollection(manageCollectionDTO);

            return RedirectToAction("Index", new { update = "success"});
        }
    }
}