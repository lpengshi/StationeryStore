using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Service;

namespace StationeryStore.Controllers
{
    public class ManageAdjustmentVoucherController : Controller
    {
        StockService stockService = new StockService();

        // GET: ManageAdjustment
        public ActionResult Index()
        {
            List<AdjustmentVoucher> adjVouchers = stockService.FindAllAdjustmentVouchers();
            ViewData["voucherList"] = adjVouchers;
            return View();
        }

        public ActionResult CreateAdjustmentVoucher()
        {
            return View();
        }
    }
}