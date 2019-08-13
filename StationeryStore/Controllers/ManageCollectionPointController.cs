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
            CollectionPointEF collectionPoint = departmentService.FindCollectionPointById(collectionPointId);
            return View(collectionPoint);
        }

        [HttpPost]
        public ActionResult UpdateCollectionPoint(CollectionPointEF collectionPoint, string decision)
        {
            int collectionPointId = collectionPoint.CollectionPointId;
            if (decision == "Cancel")
            {
                return RedirectToAction("ViewAllCollectionPoints");
            }
            if (decision == "Save")
            {
                //check that all fields are not null
                if(collectionPoint.Location == null || collectionPoint.CollectionTime == null)
                {
                    if(collectionPoint.Location == null)
                    {
                        ModelState.AddModelError("Location", "Location is required");
                    }
                    if (collectionPoint.CollectionTime == null)
                    {
                        ModelState.AddModelError("CollectionTime", "Collection time is required");
                    }
                    return View(collectionPoint);
                }

                //check that collection point location does not exist
                CollectionPointEF existingPoint = departmentService.FindAllCollectionPoints()
                    .SingleOrDefault(x => x.Location == collectionPoint.Location);

                if (existingPoint != null && existingPoint.CollectionPointId != collectionPoint.CollectionPointId)
                {
                    ModelState.AddModelError("Location", "Location already exists");
                    return View(collectionPoint);
                }
                departmentService.UpdateCollectionPoint(collectionPoint);
            }
            return RedirectToAction("ViewAllCollectionPoints");
        }
    }
}