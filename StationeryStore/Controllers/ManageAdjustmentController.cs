using StationeryStore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    [StoreFilter]
    public class ManageAdjustmentVoucherController : Controller
    {
        // GET: ManageAdjustment
        public ActionResult Index()
        {
            return View();
        }
    }
}