using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Service;
using StationeryStore.Models;
using StationeryStore.Filters;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    public class ManageAdjustmentVoucherController : Controller
    {
        StockService stockService = new StockService();
        StaffService staffService = new StaffService();

        // GET: ManageAdjustment
        public ActionResult Index(string search, int page)
        {
            List<AdjustmentVoucherEF> adjVouchers = stockService.FindAllAdjustmentVouchers();

            if (search != null) {
                adjVouchers = adjVouchers.Where(x => x.Status != null && x.Status.Contains(search)).ToList();
            }
            int pageSize = 8;
            List<AdjustmentVoucherEF> details = adjVouchers
                .OrderBy(x => x.DateIssued)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList<AdjustmentVoucherEF>();

            int noOfPages = (int)Math.Ceiling((double)adjVouchers.Count() / pageSize);

            ViewData["page"] = page;
            ViewData["noOfPages"] = noOfPages;
            ViewData["voucherList"] = adjVouchers;

            return View();
        }
        
        public ActionResult ViewAdjustmentDetails(string voucherId, string choice)
        {
            AdjustmentVoucherEF voucher = stockService.FindAdjustmentVoucherById(voucherId);
            List<AdjustmentVoucherDetailsEF> voucherDetailsList = stockService.FindAdjustmentDetailsById(voucherId);
            StaffEF staff = staffService.GetStaff();

            ViewData["adjustmentVoucher"] = voucher;
            ViewData["voucherDetailsList"] = voucherDetailsList;
            ViewData["staffRole"] = staff.Role.Description;
            ViewData["staffId"] = staff.StaffId;

            if (choice != null && voucher.Status == "Pending Approval")
            {
                bool isManager = false;

                if (choice == "Edit" && !isManager)
                {
                    return RedirectToAction("CreateAdjustmentVoucher", "ManageAdjustmentVoucher", new { voucherId = voucherId});
                }
                if (choice == "Approve" || choice == "Reject" && isManager)
                {
                    stockService.UpdateAdjustmentVoucherStatus(staff, voucherId, choice);
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult CreateAdjustmentVoucher(string voucherId)
        {
            List<AdjustmentVoucherDetailsDTO> detailsList = new List<AdjustmentVoucherDetailsDTO>();
            if (voucherId != null)
            {
                AdjustmentVoucherEF voucher = stockService.FindAdjustmentVoucherById(voucherId);
                var baseDetailsList = stockService.FindAdjustmentDetailsById(voucherId);
                foreach (var d in baseDetailsList)
                {
                    AdjustmentVoucherDetailsDTO toAdd = new AdjustmentVoucherDetailsDTO();
                    toAdd.ItemCode = d.ItemCode;
                    toAdd.Quantity = d.Quantity;
                    toAdd.Reason = d.Reason;
                    toAdd.Description = d.Stock.Description;
                    toAdd.Remove = false;

                    detailsList.Add(toAdd);
                }

                ViewData["voucher"] = voucher;
            }
            List<StockEF> itemList = stockService.FindAllStocks().OrderBy(x => x.ItemCode).ToList();
            ViewData["itemList"] = itemList;

            return View(detailsList);
        }

        [HttpPost]
        public ActionResult CreateAdjustmentVoucher(string itemToAdd, string removalItem, string choice, string voucherId, List<AdjustmentVoucherDetailsDTO> detailsList)
        {
            List<StockEF> itemList = stockService.FindAllStocks().OrderBy(x=> x.ItemCode).ToList();
            ViewData["itemList"] = itemList;

            AdjustmentVoucherEF voucher = new AdjustmentVoucherEF();
            if (voucherId == null)
            {
                voucher = new AdjustmentVoucherEF();
            }
            else
            {
                voucher = stockService.FindAdjustmentVoucherById(voucherId);

            }
            ViewData["voucher"] = voucher;

            if (detailsList == null)
            {
                detailsList = new List<AdjustmentVoucherDetailsDTO>();
            }
            if(choice == "Add Item")
            {
                StockEF item = new StockEF();
                bool isValid = true;
                string description = "";
                //check if in the existing stock
                foreach (var v in itemList)
                {
                    if (v.ItemCode == itemToAdd)
                    {
                        description = v.Description;
                        isValid = true;
                    }
                }                              
                if (isValid)
                {
                    AdjustmentVoucherDetailsDTO newVoucherDetails = new AdjustmentVoucherDetailsDTO();
                    newVoucherDetails.ItemCode = itemToAdd;
                    newVoucherDetails.Description = description;
                    newVoucherDetails.Quantity = 1;
                    newVoucherDetails.Remove = false;
                    detailsList.Add(newVoucherDetails);
                }
            }
            if(choice == "Remove" && detailsList.Count > 0)
            {
                detailsList = detailsList.Where(x => x.Remove != true).ToList();               
            }
            if(choice == "Submit")
            {
                StaffEF requester = staffService.GetStaff();
                string newId = stockService.SaveAdjustmentVoucherAndDetails(requester, voucher, detailsList);
                if (stockService.VoucherExceedsSetValue(detailsList))
                {
                    System.Diagnostics.Debug.Print("Pending appr sent to manager");
                    SendEmailToAuthority(voucher.VoucherId, "Manager");
                }
                else
                {
                    System.Diagnostics.Debug.Print("Pending appr sent to supervisor");
                    SendEmailToAuthority(voucher.VoucherId, "Supervisor");
                }
                return RedirectToAction("ViewAdjustmentDetails", "ManageAdjustmentVoucher", new { voucherId = newId});
            }

            return View(detailsList);
        }

        public void SendEmailToAuthority(string voucherId, string authorityLevel)
        {

        }
    }
}