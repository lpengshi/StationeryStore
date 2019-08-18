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

            MobileStaffDTO mobileStaff = new MobileStaffDTO
            {
                StaffId = staff.StaffId,
                Name = staff.Name,
                Role = staff.Role.Description,
            };
            return Json(mobileStaff, JsonRequestBehavior.AllowGet);
        }
        // Send retrieval details to android
        public JsonResult GetRetrieval()
        {
            // find retrieval where status = Processed
            StationeryRetrievalEF retrieval = rndService.FindRetrievalByStatus("Processing");
            MobileRetrievalItemDTO mRetrieval = null;
            if (retrieval != null)
            {
                // get the retrieval details 

                List<RetrievalItemDTO> details = rndService.ViewRetrievalListById(retrieval.RetrievalId);
                // send the retrieval list over to android app
                mRetrieval = new MobileRetrievalItemDTO()
                {
                    RetrievalId = retrieval.RetrievalId,
                    RetrievalItems = details
                };
            }

            return Json(mRetrieval, JsonRequestBehavior.AllowGet);
        }

        // Receive retrieval details from android
        public JsonResult SetRetrieval(MobileRetrievalItemDTO mRetrieval)
        {
            if (mRetrieval != null)
            {

                // save it to the database
                // update individual request's fulfilled quantity and retrieved quantity in current retrieval
                rndService.UpdateRequestAfterRetrieval(mRetrieval.RetrievalItems, mRetrieval.RetrievalId);

                // update retrieval status from processing to retrieved
                StationeryRetrievalEF retrieval = rndService.FindRetrievalById(mRetrieval.RetrievalId);
                retrieval.Status = "Retrieved";
                rndService.SaveRetrieval(retrieval);

                // get disbursements and save the dates
                string[] date = mRetrieval.DateDisbursed.Split('/');
                int year = int.Parse(date[2]);
                int month = int.Parse(date[1]);
                int day = int.Parse(date[0]);
                DateTimeOffset disbursedDate = new DateTimeOffset(year, month, day, 12, 0, 0,
                                 new TimeSpan(8, 0, 0));
                rndService.UpdateDisbursementDate(mRetrieval.RetrievalId, disbursedDate);

                // update disbursement list
                rndService.UpdateRetrievedQuantities(mRetrieval.RetrievalId);

                // and log stock transaction (deduction for department) (StockService)
                stockService.LogTransactionsForRetrieval(mRetrieval.RetrievalId);
                return Json(new { status = "ok" });
            }
            return Json(new { status = "Retreival Obtained" });
        }


        // Get Department with active disbursements
        public JsonResult GetActiveDepartments()
        {
            MobileDisbursementItemDTO disbursementInfo = null;
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
            if (activeDepartments.Count > 0)
            {
                List<StationeryDisbursementDetailsEF> details = rndService.FindDisbursementDetailsByDisbursementId(activeDepartments[0].DisbursementId);
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

                List<StaffEF> deptStaff = staffService.FindAllEmployeeByDepartmentCode(activeDepartments[0].DepartmentCode);
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
                disbursementInfo = new MobileDisbursementItemDTO
                {
                    ActiveDepartments = activeDepartments,
                    DepartmentStaff = deptStaffDTO,
                    DisbursementDetails = detailsDTO,
                };
            }
           
            return Json(disbursementInfo, JsonRequestBehavior.AllowGet);
        }

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
        public JsonResult SetDisbursement(MobileDisbursementItemDTO mobileDisbursementItem)
        {
            List<StationeryDisbursementDetailsEF> details = new List<StationeryDisbursementDetailsEF>();
            foreach (var item in mobileDisbursementItem.DisbursementDetails)
            {
                details.Add(new StationeryDisbursementDetailsEF
                {
                    DisbursementId = item.DisbursementId,
                    DisbursementDetailsId = item.DisbursementDetailsId,
                    ItemCode = item.ItemCode,
                    StationeryDisbursement = rndService.FindDisbursementById(item.DisbursementId),
                    Stock = item.Stock,
                    RequestQuantity = item.RequestQuantity,
                    DisbursedQuantity = item.DisbursedQuantity,
                    RetrievedQuantity = item.RetrievedQuantity

                });
            }

            int disbursementId = mobileDisbursementItem.DisbursementId;
            int collectionRepId = mobileDisbursementItem.CollectionRepId;
            int storeClerkId = mobileDisbursementItem.ClerkId;

            // update disbursement details' Disbursed Quantity and disbursement status to disbursed
            rndService.UpdateDisbursedQuantities(details, disbursementId, collectionRepId, storeClerkId);

            // update request details
            rndService.UpdateRequestAfterDisbursement(details, disbursementId);

            // log any stock transaction (damaged goods) - compare retrievedQty with disbursedqty
            stockService.LogTransactionsForActualDisbursement(disbursementId);

            // email collection rep for acknowledgement of disbursement
            string collectionRepEmail = staffService.FindStaffById(collectionRepId).Email;
            string subject = "Disbursement #" + disbursementId + " : Request for Acknowledgement";
            string body = "Disbursement #" + disbursementId + " has been disbursed. Please click " +
                "<a href='http://localhost/StationeryStore/ViewDisbursement/ViewDisbursement/?disbursementId=" + disbursementId + "'>" +
                "here</a> to view the details of the disbursement and acknowledge receipt of stationery item(s).";
            Email.SendEmail(collectionRepEmail, subject, body);

            return Json(new { status = "Received Disbursement" });
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
                DisbursementId = stationeryDisbursement.DisbursementId,
                ClerkId = stationeryDisbursement.StoreClerk.StaffId

            };


            return Json(disbursementInfo, JsonRequestBehavior.AllowGet);

        }

        public JsonResult AcknowledgeDisbursement(int disbursementId)
        {
            StaffEF staff = staffService.GetStaff();
            ViewBag.staff = staff;
            StationeryDisbursementEF disbursement = rndService.FindDisbursementById(disbursementId);
            rndService.UpdateDisbursementStatus(disbursement);

            return Json(new { status = "Delivery Acknowledged" });
        }

        public JsonResult GetRequests(int staffId)
        {
            //Find the staff by their Id
            StaffEF staff = staffService.FindStaffById(staffId);

            // Pass all submitted request from the department
            List<StationeryRequestEF> pendingList = rndService.FindRequestByDepartmentAndStatus(staff.Department, "Submitted");
            MobileStationeryRequestListDTO requestListDTO = new MobileStationeryRequestListDTO()
            {
                StationeryRequests = new List<MobileStationeryRequestDTO>()
            };
            foreach (var item in pendingList)
            {
                requestListDTO.StationeryRequests.Add(new MobileStationeryRequestDTO
                {
                    RequestId = item.RequestId,
                    StaffId = item.Staff.StaffId,
                    StaffName = item.Staff.Name,
                    DecisionById = item.DecisionBy.StaffId,
                    DecisionByName = item.DecisionBy.Name,
                    RequestDate = item.RequestDate,
                    DecisionDate = (long)item.DecisionDate,
                    Comment = item.Comment,
                    Status = item.Status,
                    Decision = ""

                });
            }

            return Json(requestListDTO, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRequestDetails(MobileStationeryRequestDTO Request)
        {
            string requestId = Request.RequestId;
            int staffId = Request.DecisionById;

            //Find the staff by their Id
            StaffEF staff = staffService.FindStaffById(staffId);

            // See request details
            StationeryRequestEF request = rndService.FindRequestById(requestId);

            if (request.Staff.DepartmentCode != staff.DepartmentCode)
            {
                return Json(new { status = "Department Mismatch" });
            }

            List<StationeryRequestDetailsEF> requestDetails = rndService.FindRequestDetailsByRequestId(requestId);
            MobileStationeryRequestDetailsListDTO requestDetailsDTO = new MobileStationeryRequestDetailsListDTO()
            {
                RequestDetails = new List<MobileStationeryRequestDetailsDTO>()
            };
            foreach (var item in requestDetails)
            {
                requestDetailsDTO.RequestDetails.Add(new MobileStationeryRequestDetailsDTO
                {
                    RequestDetailsId = item.RequestDetailsId,
                    RequestId = item.RequestId,
                    ItemCode = item.ItemCode,
                    ItemDescription = item.Stock.Description,
                    RequestQuantity = item.RequestQuantity
                }
                );
            }

            return Json(requestDetailsDTO, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApproveRequest(MobileStationeryRequestDTO Request)
        {

            string requestId = Request.RequestId;
            string comment = Request.Comment;
            string decision = Request.Decision;
            int staffId = Request.DecisionById;

            //Find the staff by their Id
            StaffEF staff = staffService.FindStaffById(staffId);

            StationeryRequestEF request = rndService.FindRequestById(requestId);
            List<StationeryRequestDetailsEF> requestDetails = rndService.FindRequestDetailsByRequestId(requestId);
            // Update approval/rejection and comments to request
            rndService.UpdateRequestDecision(staff, request, comment, decision);

            return Json(new { status = "Decision Updated" });
        }

    }
}