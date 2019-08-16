using StationeryStore.Models;
using StationeryStore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StationeryStore.Util;
using System.Diagnostics;

namespace StationeryStore.Controllers
{
    public class MobileController : Controller
    {
        RequestAndDisburseService rndService = new RequestAndDisburseService();
        StockService stockService = new StockService();
        StaffService staffService = new StaffService();

        // Validate user login
        public JsonResult GetLogin(LoginDTO loginDTO)
        {
            StaffEF staff = null;
            if (loginDTO != null)
            {
                staff = staffService.FindStaffByUsername(loginDTO.Username);
            }

            return Json(staff, JsonRequestBehavior.AllowGet);
        }


        // Send retrieval details to android
        public JsonResult GetRetrieval()
        {
            // find retrieval where status = Processed
            StationeryRetrievalEF retrieval = rndService.FindRetrievalByStatus("Processing");

            // get the retrieval details
            List<RetrievalItemDTO> details = rndService.ViewRetrievalListById(retrieval.RetrievalId);
            // send the retrieval list over to android app
            MobileRetrievalItemDTO mRetrieval = new MobileRetrievalItemDTO()
            {
                RetrievalId = retrieval.RetrievalId,
                DateDisbursed = DateTimeOffset.UtcNow.AddDays(3),
                RetrievalItems = details
            };

            return Json(mRetrieval, JsonRequestBehavior.AllowGet);
        }

        // Receive retrieval details from android
        public JsonResult SetRetrieval(MobileRetrievalItemDTO mRetrieval)
        {
            if (mRetrieval != null)
            {
                //Debug logger
                Debug.WriteLine(mRetrieval.RetrievalId.ToString() + mRetrieval.DateDisbursed.ToString());
                if (mRetrieval.RetrievalItems != null)
                {
                    foreach (var i in mRetrieval.RetrievalItems)
                    {
                        String x = i.ItemCode + " " + i.RetrievedQty + " " + i.ItemDescription + " " + i.Bin + " " + i.TotalOutstandingQty;
                        Debug.WriteLine(x);
                        if (i.RetrievedQty.ToString() == "")
                        {
                            return Json(new { status = "fail" });
                        }
                    }
                }
                //END of logger

                // save it to the database
                // update individual request's fulfilled quantity and retrieved quantity in current retrieval
                rndService.UpdateRequestAfterRetrieval(mRetrieval.RetrievalItems, mRetrieval.RetrievalId);

                // update retrieval status from processing to retrieved
                StationeryRetrievalEF retrieval = rndService.FindRetrievalById(mRetrieval.RetrievalId);
                retrieval.Status = "Retrieved";
                rndService.SaveRetrieval(retrieval);

                // get disbursements and save the dates
                rndService.UpdateDisbursementDate(mRetrieval.RetrievalId, mRetrieval.DateDisbursed);

                // update disbursement list
                rndService.UpdateRetrievedQuantities(mRetrieval.RetrievalId);

                // and log stock transaction (deduction for department) (StockService)
                stockService.LogTransactionsForRetrieval(mRetrieval.RetrievalId);
                return Json(new { status = "ok" });
            }
            return Json(new { status = "fail" });
        }

        //NEW KK
        // Get Department with active disbursements
        public JsonResult GetActiveDepartments()
        {
            List<StationeryDisbursementEF> activeDisbursements = rndService.FindDisbursementsByStatus("Retrieved");
            List<MobileActiveDepartmentDTO> activeDepartments = new List<MobileActiveDepartmentDTO>();
            foreach (var item in activeDisbursements)
            {
                activeDepartments.Add(
                    new MobileActiveDepartmentDTO
                    {
                        DisbursementId = item.DisbursementId,
                        DepartmentCode = item.DepartmentCode,
                        DepartmentName = item.Department.DepartmentName

                    }
                );
            }

            MobileDisbursementItemDTO disbursementInfo = new MobileDisbursementItemDTO
            {
                ActiveDepartments = activeDepartments
            };

            return Json(disbursementInfo, JsonRequestBehavior.AllowGet);
        }

        //NEW KK
        // Send disbursement details to android
        public JsonResult GetDepartmentDisbursement(int disbursementId)
        {

            //Get Departments with active disbursements
            List<StationeryDisbursementEF> activeDisbursements = rndService.FindDisbursementsByStatus("Retrieved");
            List<MobileActiveDepartmentDTO> activeDepartments = new List<MobileActiveDepartmentDTO>();
            foreach (var item in activeDisbursements)
            {
                activeDepartments.Add(
                    new MobileActiveDepartmentDTO
                    {
                        DisbursementId = item.DisbursementId,
                        DepartmentCode = item.DepartmentCode,
                        DepartmentName = item.Department.DepartmentName

                    }
                );
            }

            //Get items in the disbursement for the department
            //StationeryDisbursementDetailsEF stationeryDisbursement = rndService.FindDisbursementByDepartmentCode(departmentId);
            List<StationeryDisbursementDetailsEF> details = rndService.FindDisbursementDetailsByDisbursementId(disbursementId);
            List<MobileStationeryDisbursementDetailsDTO> detailsDTO = new List<MobileStationeryDisbursementDetailsDTO>();
            foreach (var item in details)
            {
                detailsDTO.Add(new MobileStationeryDisbursementDetailsDTO
                {
                    DisbursementDetailsId = item.DisbursementDetailsId,
                    DisbursementId = item.DisbursementId,
                    ItemCode = item.ItemCode,
                    Stock = item.Stock,
                    RequestQuantity = item.RequestQuantity,
                    RetrievedQuantity = item.RetrievedQuantity,
                    DisbursedQuantity = item.DisbursedQuantity
                }
                );
            }

            // list of staff in that department
            StationeryDisbursementEF disbursement = rndService.FindDisbursementById(disbursementId);
            List<StaffEF> deptStaff = staffService.FindAllEmployeeByDepartmentCode(disbursement.DepartmentCode);
            List<MobileStaffDTO> deptStaffDTO = new List<MobileStaffDTO>();
            foreach (var staff in deptStaff)
            {
                deptStaffDTO.Add(new MobileStaffDTO
                {
                    Name = staff.Name,
                    StaffId = staff.StaffId
                }
                );
            }
            //StaffEF storeClerk = staffService.GetStaff();

            // Create a DTO for disbursement which includes storeClerkId/Name and EmployeeId/Name
            MobileDisbursementItemDTO mobileDisbursementItem = new MobileDisbursementItemDTO
            {
                ActiveDepartments = activeDepartments,
                DisbursementDetails = detailsDTO,
                DisbursementId = disbursementId,
                DepartmentStaff = deptStaffDTO,
            };

            // send a list of disbursements 
            return Json(mobileDisbursementItem, JsonRequestBehavior.AllowGet);
        }

        //Get disbursement details from android
        public JsonResult SetDisbursement(MobileDisbursementItemDTO mobileDisbursementItemReturn)
        {

            //    // update disbursement details' Disbursed Quantity and disbursement status to disbursed
            //    // rndService.UpdateDisbursedQuantities(details, disbursementId, collectionRepId, storeClerkId);

            //    // update request details
            //    // rndService.UpdateRequestAfterDisbursement(details, disbursementId);

            //    // log any stock transaction (damaged goods) - compare retrievedQty with disbursedqty
            //    // stockService.LogTransactionsForActualDisbursement(disbursementId);

            //    // android side to send a notification to department rep

            return Json(new { status = "gottit" });
        }

        public JsonResult ViewDisbursement(int staffId)
        {
            StationeryDisbursementEF stationeryDisbursement = rndService.FindDisbursementByStatusAndStaffId(staffId, "Disbursed");
            List<StationeryDisbursementDetailsEF> stationeryDisbursementDetails = rndService.FindDisbursementDetailsByDisbursementId(stationeryDisbursement.DisbursementId);
            List<MobileStationeryDisbursementDetailsDTO> detailsDTO = new List<MobileStationeryDisbursementDetailsDTO>();
            foreach (var item in stationeryDisbursementDetails)
            {
                detailsDTO.Add(new MobileStationeryDisbursementDetailsDTO
                {
                    DisbursementDetailsId = item.DisbursementDetailsId,
                    DisbursementId = item.DisbursementId,
                    ItemCode = item.ItemCode,
                    Stock = item.Stock,
                    RequestQuantity = item.RequestQuantity,
                    RetrievedQuantity = item.RetrievedQuantity,
                    DisbursedQuantity = item.DisbursedQuantity
                }
                );
            }

            MobileDisbursementItemDTO disbursementInfo = new MobileDisbursementItemDTO
            {
                DisbursementDetails = detailsDTO,
                DisbursementId = stationeryDisbursement.DisbursementId

            };

            return Json(disbursementInfo, JsonRequestBehavior.AllowGet);

        }

        //public JsonResult SendEmailForAcknowledgment()
        //{
        //    // department rep to acknowledge 
        //    // system to update status of disbursement
        //}

        //public JsonResult AcknowledgeDisbursement()
        //{
        //    // department rep to acknowledge 
        //    // system to update status of disbursement
        //}

    }
}