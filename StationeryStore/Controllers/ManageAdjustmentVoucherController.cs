using StationeryStore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Service;
using StationeryStore.Models;
using StationeryStore.Util;
using System.Net.Mail;
using System.Diagnostics;

namespace StationeryStore.Controllers
{
    [AuthorizeFilter]
    [StoreFilter]
    public class ManageAdjustmentVoucherController : Controller
    {
        StockService stockService = new StockService();
        StaffService staffService = new StaffService();

        // GET: ManageAdjustment
        public ActionResult Index(string search, int page)
        {
            List<AdjustmentVoucherEF> adjVouchers = new List<AdjustmentVoucherEF>();
            List<string> status = new List<string>();
            status.Add("All");
            foreach (var v in stockService.FindAllAdjustmentVouchers().Select(x => x.Status).Distinct().ToList())
            {
                status.Add(v);
            }

            if (search != null && search != "All")
            {
                adjVouchers = stockService.FindAllAdjustmentVouchers();
                adjVouchers = adjVouchers.Where(x => x.Status != null && x.Status.Contains(search)).ToList();
            }
            else if ((search != null && search == "All") || search == null)
            {
                adjVouchers = stockService.FindAllAdjustmentVouchers();
            }


            int pageSize = 8;
            List<AdjustmentVoucherEF> details = adjVouchers               
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList<AdjustmentVoucherEF>();

            int noOfPages = (int)Math.Ceiling((double)adjVouchers.Count() / pageSize);

            ViewData["statusList"] = status;
            ViewData["search"] = search;
            ViewData["page"] = page;
            ViewData["noOfPages"] = noOfPages;
            ViewData["voucherList"] = details;

            return View();
        }

        public ActionResult ViewAdjustmentDetails(string voucherId, string choice)
        {
            AdjustmentVoucherEF voucher = stockService.FindAdjustmentVoucherById(voucherId);
            List<AdjustmentVoucherDetailsEF> voucherDetailsList = stockService.FindAdjustmentDetailsById(voucherId);
            List<AdjustmentVoucherDetailsDTO> convertedList = stockService.ConvertAdjVoucherDetailsToDTO(voucherDetailsList);

            StaffEF staff = staffService.GetStaff();

            bool needsManagerAuthority = stockService.VoucherExceedsSetValue(voucherDetailsList);

            ViewData["needsManagerAuthority"] = needsManagerAuthority;
            ViewData["adjustmentVoucher"] = voucher;
            ViewData["voucherDetailsList"] = convertedList;
            ViewData["staffRole"] = staff.Role.Description;
            ViewData["staffId"] = staff.StaffId;

            if (choice != null && (voucher.Status == "Pending Approval" || voucher.Status == "Pending Manager Approval"))
            {

                if (choice == "Edit")
                {
                    return RedirectToAction("CreateAdjustmentVoucher", "ManageAdjustmentVoucher", new { voucherId = voucherId });
                }
                if (choice == "Approve" || choice == "Reject")
                {
                    stockService.UpdateAdjustmentVoucherStatus(staff, voucherId, choice);
                    SendEmailToStaffOnDecision(voucher.Requester, voucher);
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

                detailsList = stockService.GetPricesForVoucherDetails(detailsList);
                ViewData["voucher"] = voucher;
            }
            List<StockEF> itemList = stockService.FindAllStocks().OrderBy(x => x.ItemCode).ToList();
            ViewData["itemList"] = itemList;

            return View(detailsList);
        }

        [HttpPost]
        public ActionResult CreateAdjustmentVoucher(string itemToAdd, string removalItem, string choice, string voucherId, List<AdjustmentVoucherDetailsDTO> detailsList)
        {
            List<StockEF> itemList = stockService.FindAllStocks().OrderBy(x => x.ItemCode).ToList();
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
            if (choice == "Add Item")
            {
                StockEF item = new StockEF();
                bool isValid = false;
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
            if (choice == "Remove" && detailsList.Count > 0)
            {
                for (int i = 0; i < detailsList.Count; i++)
                {
                    if (detailsList[i].Remove == true)
                    {
                        detailsList.RemoveAt(i);
                        i--;
                    }
                }
            }
            if (choice == "Submit")
            {
                StaffEF requester = staffService.GetStaff();
                string newId = "";
                if (stockService.VoucherExceedsSetValue(detailsList))
                {
                    newId = stockService.SaveAdjustmentVoucherAndDetails(requester, voucher, detailsList, "Manager");
                    Debug.Print("Pending appr sent to manager email");
                    SendEmailToAuthorityOnRequest(voucher.VoucherId, "Manager");
                }
                else if(!stockService.VoucherExceedsSetValue(detailsList))
                {
                    newId = stockService.SaveAdjustmentVoucherAndDetails(requester, voucher, detailsList, "Supervisor");
                    Debug.Print("Pending appr sent to supervisor email");
                    SendEmailToAuthorityOnRequest(voucher.VoucherId, "Supervisor");
                }
                return RedirectToAction("ViewAdjustmentDetails", "ManageAdjustmentVoucher", new { voucherId = newId });
            }
            if (choice == "Cancel")
            {
                return RedirectToAction("Index", "ManageAdjustmentVoucher", new { page = 1 });
            }

            //get prices for each item
            detailsList = stockService.GetPricesForVoucherDetails(detailsList);

            ModelState.Clear();
            return View(detailsList);
        }

        private void SendEmailToAuthorityOnRequest(string voucherId, string authorityLevel)
        {
            List<string> recepientEmailAdd = new List<string>();

            if (authorityLevel == "Manager")
            {
                List<StaffEF> managers = staffService.FindStaffByRole(5);
                foreach (var manager in managers)
                {
                    recepientEmailAdd.Add(manager.Email);
                }

            }
            else if (authorityLevel == "Supervisor")
            {
                List<StaffEF> sups = staffService.FindStaffByRole(4);
                foreach (var s in sups)
                {
                    recepientEmailAdd.Add(s.Email);
                }
            }

            foreach (string e in recepientEmailAdd)
            {
                Email.SendEmail(e,
                    "Adjustment Voucher #" + voucherId + " : Pending Review",
                    "Adjustment Voucher #" + voucherId + " requires review.");
            }
        }

        private void SendEmailToStaffOnDecision(StaffEF staff, AdjustmentVoucherEF voucher)
        {
            string email = staff.Email;
            Email.SendEmail(email,
                    "Adjustment Voucher #" + voucher.VoucherId + " : has been " + voucher.Status.ToLower() + ".",
                    "Adjustment Voucher #" + voucher.VoucherId + " has been " + voucher.Status.ToLower() + " by " + voucher.Approver.Name + ".");
        }
    }
}