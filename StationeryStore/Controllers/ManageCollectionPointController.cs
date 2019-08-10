using StationeryStore.Filters;
using StationeryStore.Models;
using StationeryStore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    [StoreFilter]
    public class ManageCollectionPointController : Controller
    {
        DepartmentService departmentService = new DepartmentService();
        
        public ActionResult ViewAllCollectionPoints()
        {
            List<CollectionPointEF> collectionPoints = departmentService.FindAllCollectionPoints();
            ViewData["collectionPoints"] = collectionPoints;
            return View();
        }

        [HttpGet]
        public ActionResult UpdateCollectionPoint(int collectionPointId)
        {
            //retrieve collection point details
            CollectionPointEF collectionPoint = departmentService.FindCollectionPointById(collectionPointId);
            return View(collectionPoint);
        }

        [HttpPost]
        public ActionResult UpdateCollectionPoint(CollectionPointEF collectionPoint, string decision)
        {
            int collectionPointId = collectionPoint.CollectionPointId;
            if (decision == "Save")
            {
                //update collection point details
                departmentService.UpdateCollectionPoint(collectionPoint);
            }
            return RedirectToAction("ViewAllCollectionPoints", new { collectionPointId });
        }
    }
}