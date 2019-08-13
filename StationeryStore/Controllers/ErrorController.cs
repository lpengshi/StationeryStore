using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StationeryStore.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StoreRoleError()
        {
            return View();
        }

        public ActionResult DepartmentRoleError()
        {
            return View();
        }
    }
}