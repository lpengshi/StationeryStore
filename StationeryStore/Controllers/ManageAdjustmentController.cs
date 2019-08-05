using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Service;
using StationeryStore.Models;

namespace StationeryStore.Controllers
{
    public class ManageAdjustmentVoucherController : Controller
    {
        StockService stockService = new StockService();
        StaffService staffService = new StaffService();

        // GET: ManageAdjustment
        public ActionResult Index(string search)
        {
            List<AdjustmentVoucherEF> adjVouchers = stockService.FindAllAdjustmentVouchers();
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
                    return RedirectToAction("CreateAdjustmentVoucher", "ManageAdjustment", new { voucher = voucher, detailsList= voucherDetailsList});
                }
                if (choice == "Approve" || choice == "Reject" && isManager)
                {
                    stockService.UpdateAdjustmentVoucherStatus(staff, voucherId, choice);
                }
            }
            return View();
        }

        public ActionResult CreateAdjustmentVoucher(string itemToAdd, string removalItem, string choice, AdjustmentVoucherEF voucher, List<AdjustmentVoucherDetailsDTO> detailsList)
        {
            List<StockEF> itemList = stockService.FindAllStocks().OrderBy(x=> x.ItemCode).ToList();
            ViewData["itemList"] = itemList;

            if (voucher == null || voucher.VoucherId == null)
            {
                voucher = new AdjustmentVoucherEF();
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
                //check if in the existing voucher details list
                //same item can exist but with different reasons.
                //foreach (var v in detailsList)
                //{
                //    if(v.ItemCode == itemToAdd)
                //    {
                //        isValid = false;
                //    }
                //}                
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
            if(choice == "Remove")
            {
                foreach (var v in detailsList)
                {
                    if(v.Remove == true)
                    {
                        detailsList.Remove(v);
                    }
                }
            }
            if(choice == "Submit")
            {
                StaffEF requester = staffService.GetStaff();
                stockService.SaveAdjustmentVoucherAndDetails(requester, voucher, detailsList);
                if (stockService.VoucherExceedsSetValue(detailsList))
                {
                    SendEmailToAuthority(voucher.VoucherId, "Manager");
                }
                else
                {
                    SendEmailToAuthority(voucher.VoucherId, "Supervisor");
                }
            }

            ViewData["detailsList"] = detailsList;
            return View();
        }

        public void SendEmailToAuthority(string voucherId, string authorityLevel)
        {

        }
    }
}