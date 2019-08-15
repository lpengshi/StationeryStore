using StationeryStore.EntityFrameworkFacade;
using StationeryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StationeryStore.Util;

namespace StationeryStore.Service
{
    public class RequestAndDisburseService
    {
        RequestAndDisburseEFFacade rndEFF = new RequestAndDisburseEFFacade();
        StockEFFacade stockEFF = new StockEFFacade();

        public List<StationeryRequestDetailsEF> FindOutstandingRequestDetails()
        {
            List<StationeryRequestDetailsEF> unfulfilledRequests = rndEFF.FindAllRequestDetailsByStatus("Outstanding");
            List<StationeryRequestDetailsEF> approvedRequests = rndEFF.FindAllRequestDetailsByStatus("Approved");
            unfulfilledRequests.AddRange(approvedRequests);

            return unfulfilledRequests;
        }

        public List<StationeryDisbursementEF> FindDisbursementByDepartmentCode(string departmentCode)
        {
            List<StationeryDisbursementEF> disbursementList = rndEFF.FindDisbursementByDepartmentCode(departmentCode);

            return disbursementList;
        }

        public List<RequestTemplateEF> FindRequestTemplateByStaffId(int staffId)
        {
            List<RequestTemplateEF> requestTemplate = rndEFF.FindRequestTemplateByStaffId(staffId);

            return requestTemplate;
        }

        public List<string> ConvertToDate(List<StationeryRequestEF> pendingList)
        {
            string date;
            List<string> dateList = new List<string>();
            for (int i = 0; i < pendingList.Count; i++)
            {
                date = Timestamp.dateFromTimestamp(pendingList[i].RequestDate);
                dateList.Add(date);
            }

            return dateList;
        }

        public void CreateRequestTemplate(string templateName, int staffId)
        {
            RequestTemplateEF requestTemplate = new RequestTemplateEF()
            {
                TemplateName = templateName,
                StaffId = staffId,
            };

            rndEFF.SaveRequestTemplate(requestTemplate);
        }

        public void UpdateDisbursementStatus(StationeryDisbursementEF disbursement)
        {
            disbursement.Status = "Acknowledged";
            if (disbursement.StoreClerk.Email != null)
            {
                string subject = "Acknowledgement for Disbursement#" + disbursement.DisbursementId;
                string body = disbursement.Staff.Name + " has acknowledged disbursement#" + disbursement.DisbursementId + ".";
                Email.SendEmail(disbursement.StoreClerk.Email, subject, body);
            }
            rndEFF.SaveDisbursement(disbursement);
        }

        public RequestTemplateEF FindRequestTemplateByTemplateId(int templateId)
        {
            return rndEFF.FindRequestTemplateByTemplateId(templateId);
        }

        public List<StationeryRequestEF> FindRequestByDepartmentAndStatus(DepartmentEF department, string status)
        {
            List<StationeryRequestEF> pendingList = rndEFF.FindRequestsByDepartmentAndStatus(department.DepartmentCode, status);

            return pendingList;
        }

        public RequestTemplateDTO FindRequestTemplateDetailsByTemplateId(int templateId)
        {
            List<RequestTemplateDetailsEF> requestTemplateDetailsList = rndEFF.FindRequestTemplateDetailsByTemplateId(templateId);

            RequestTemplateDTO requestTemplateDTO = new RequestTemplateDTO()
            {
                TemplateId = templateId,
            };

           foreach (var item in requestTemplateDetailsList)
            {
                requestTemplateDTO.ItemDescription.Add(item.Stock.Description);
                requestTemplateDTO.ItemUom.Add(item.Stock.Uom);
                requestTemplateDTO.Quantity.Add(item.RequestQuantity);
                requestTemplateDTO.Remove.Add(false);
            }

            return requestTemplateDTO;
        }

        public RequestTemplateDTO AddToRequestTemplateDTO(RequestTemplateDTO requestTemplateDTO, string description, string uom)
        {
            requestTemplateDTO.ItemDescription.Add(description);
            requestTemplateDTO.ItemUom.Add(uom);
            requestTemplateDTO.Quantity.Add(1);
            requestTemplateDTO.Remove.Add(false);

            return requestTemplateDTO;
        }

        public RequestTemplateDTO RemoveFromRequestTemplateDTO(RequestTemplateDTO requestTemplateDTO)
        {
            for (int i = 0; i < requestTemplateDTO.ItemDescription.Count; i++)
            {
                if (requestTemplateDTO.Remove[i] == true)
                {
                    requestTemplateDTO.ItemDescription.RemoveAt(i);
                    requestTemplateDTO.Quantity.RemoveAt(i);
                    requestTemplateDTO.ItemUom.RemoveAt(i);
                    requestTemplateDTO.Remove.RemoveAt(i);
                    i--;
                }
            }

            return requestTemplateDTO;
        }

        public void DeleteRequestTemplate(int templateId)
        {
            rndEFF.DeleteRequestTemplate(templateId);
        }

        public List<RequestDTO> ConvertToRequestDTO(List<StationeryRequestEF> requestList)
        {
            RequestDTO requestDTO;
            List<RequestDTO> requestDTOList = new List<RequestDTO>();
            for (int i = 0 ; i < requestList.Count; i++)
            {
                string requestDate = Timestamp.dateFromTimestamp(requestList[i].RequestDate);
                requestDTO = new RequestDTO(requestList[i].RequestId, requestDate, requestList[i].Status);
                requestDTOList.Add(requestDTO);
            }

            return requestDTOList;
        }

        public void UpdateRequestDecision(StaffEF staff, StationeryRequestEF request, string comment, string decision)
        {
            request.DecisionById = staff.StaffId;
            request.DecisionDate = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            if (staff.Role.Description == "Department Head")
            {
                request.Designation = "Department Head";
            }
            else
            {
                request.Designation = "Covering Department Head";
            }

            List<StationeryRequestDetailsEF> requestList = rndEFF.FindAllStationeryRequestDetailsByRequestId(request.RequestId);

            request.Comment = comment;

            if (decision == "Approve")
            {
                request.Status = "Approved";
                for (int i = 0; i < requestList.Count; i++)
                {
                    requestList[i].FulfilmentStatus = "Approved";
                }
            }
            else if (decision == "Reject")
            {
                request.Status = "Rejected";
                for (int i = 0; i < requestList.Count; i++)
                {
                    requestList[i].FulfilmentStatus = "Rejected";
                }
            }

            rndEFF.SaveRequestAndRequestDetails(request, requestList);
            if (request.Staff.Email != null)
            {
                string subject = "Request Status Update";
                string body = "Request #" + request.RequestId + " has been " + request.Status.ToLower() + " by " + staff.Name + "(" + request.Designation + "). Please click " +
                "<a href='http://localhost/StationeryStore/ManageRequest/ViewRequest/?requestId=" + request.RequestId + "'>" + "here</a> to view the details.";
                Email.SendEmail(request.Staff.Email, subject, body);
            }
        }
        
        public RequestListDTO ConvertToRequestListDTO(RequestTemplateDTO requestTemplateDTO)
        {
            RequestListDTO requestListDTO = new RequestListDTO();

            for (int i = 0; i < requestTemplateDTO.ItemDescription.Count; i++)
            {
                requestListDTO.ItemDescription.Add(requestTemplateDTO.ItemDescription[i]);
                requestListDTO.ItemUom.Add(requestTemplateDTO.ItemUom[i]);
                requestListDTO.Quantity.Add(requestTemplateDTO.Quantity[i]);
                requestListDTO.Remove.Add(requestTemplateDTO.Remove[i]);
            }

            return requestListDTO;
        }

        public void UpdateRequestTemplate(StaffEF staff, RequestTemplateDTO requestTemplateDTO)
        {
            RequestTemplateDetailsEF requestTemplateItem; bool existingItem;
            List<RequestTemplateDetailsEF> requestTemplateList = new List<RequestTemplateDetailsEF>();
            List <RequestTemplateDetailsEF> requestTemplateDetails = rndEFF.FindRequestTemplateDetailsByTemplateId(requestTemplateDTO.TemplateId);
                for (int i = 0; i < requestTemplateDetails.Count; i++)
                {
                    existingItem = false;
                    for (int j = 0; j < requestTemplateDTO.ItemDescription.Count; j++)
                    {
                        if (requestTemplateDetails[i].Stock.Description == requestTemplateDTO.ItemDescription[j])
                        {
                            requestTemplateItem = rndEFF.FindRequestTemplateDetailsByTemplateIdAndItemCode(requestTemplateDTO.TemplateId, requestTemplateDetails[i].Stock.ItemCode);
                            requestTemplateItem.RequestQuantity = requestTemplateDTO.Quantity[j];
                            requestTemplateList.Add(requestTemplateItem);
                            existingItem = true;
                            requestTemplateDTO.Remove[j] = true;
                            break;
                        }
                    }

                    if (!existingItem)
                    {
                        rndEFF.DropRequestTemplateDetails(requestTemplateDetails[i]);
                    }
                }

                requestTemplateDTO = RemoveFromRequestTemplateDTO(requestTemplateDTO);

            for (int k = 0; k < requestTemplateDTO.ItemDescription.Count; k++)
            {
                string stockId = stockEFF.FindStockByDescription(requestTemplateDTO.ItemDescription[k]).ItemCode;
                requestTemplateItem = new RequestTemplateDetailsEF(requestTemplateDTO.TemplateId, stockId, requestTemplateDTO.Quantity[k]);
                requestTemplateList.Add(requestTemplateItem);
            }

            rndEFF.SaveRequestTemplateDetails(requestTemplateList);
        }

        public List<RequestDTO> FindRequestByStaffAndStatus(int staffId, string status)
        {
            List<StationeryRequestEF> pendingList = rndEFF.FindRequestsByStaffIdAndStatus(staffId, status);
            List<RequestDTO> requestDTOList = ConvertToRequestDTO(pendingList);
          

            return requestDTOList;
        }

        public List<RequestDTO> FindRequestsByStaff(int staffId)
        {
           List<StationeryRequestEF> requestList = rndEFF.FindAllStationeryRequestByStaffId(staffId);
            List<RequestDTO> requestDTOList = ConvertToRequestDTO(requestList);

            return requestDTOList;
        }

        public void DeleteRequest(string requestId)
        {
            StationeryRequestEF request = rndEFF.FindRequestById(requestId);
            List<StationeryRequestDetailsEF> requestList = rndEFF.FindAllStationeryRequestDetailsByRequestId(requestId);
            request.Status = "Deleted";
            foreach (var detail in requestList)
            {
                detail.FulfilmentStatus = "Deleted";
            }

            rndEFF.SaveRequestAndRequestDetails(request, requestList);
        }

        public void UpdateRequestDetails(StationeryRequestDetailsEF details)
        {
            rndEFF.SaveRequestDetails(details);
        }

        public void UpdateRequestAfterRetrieval(List<RetrievalItemDTO> details, int retrievalId)
        {
            // for each item in that retrieval, distribute it among the departments according to date
            foreach (RetrievalItemDTO item in details)
            {
                // list of requests in that retrieval
                List<StationeryRequestDetailsEF> requestDetails = rndEFF.FindAllRequestDetailsByStatusAndItemCode("Processed", item.ItemCode);
                int itemCount = (int)item.RetrievedQty;

                // if every request can be fulfilled, update fulfilled qty of request
                if (item.TotalOutstandingQty <= item.RetrievedQty)
                {
                    foreach (StationeryRequestDetailsEF request in requestDetails)
                    {
                        int outstandingQty = request.RequestQuantity - request.FulfilledQuantity;
                        request.RetrievedQuantity = outstandingQty;
                        rndEFF.SaveRequestDetails(request);
                    }
                }
                // if not every request can be fulfilled due to insufficient stock
                // item.TotalOutstandingQty > item.RetrievedQuantity
                else
                {
                    // sort the request by date in ascending order and distribute
                    List<StationeryRequestDetailsEF> sortedRequests = requestDetails.OrderBy(x => x.Request.RequestDate).ToList<StationeryRequestDetailsEF>();
                    foreach (StationeryRequestDetailsEF request in sortedRequests)
                    {
                        int outstandingQty = request.RequestQuantity - request.FulfilledQuantity;
                        if (outstandingQty <= itemCount)
                        {
                            request.RetrievedQuantity = outstandingQty;
                            rndEFF.SaveRequestDetails(request);
                            itemCount = itemCount - outstandingQty;
                        }
                        // insufficient stock for that request (outstandingQty > itemCount)
                        else
                        {
                            if (itemCount != 0)
                            {
                                request.RetrievedQuantity = itemCount;
                                rndEFF.SaveRequestDetails(request);
                                itemCount = 0;
                            }
                            // itemCount == 0, means no more of that item to distribute
                            else
                            {
                                // fulfilled quantity does not change
                                request.RetrievedQuantity = 0;
                                rndEFF.SaveRequestDetails(request);
                            }
                        }
                    }
                }
            }
        }

        public RequestListDTO AddToRequestListDTO(string requestId, string status, List<StationeryRequestDetailsEF> requestDetails)
        {
            RequestListDTO requestListDTO = new RequestListDTO
            {
                RequestId = requestId,
                Status = status
            };

            foreach (var item in requestDetails)
            {
                requestListDTO.ItemDescription.Add(item.Stock.Description);
                requestListDTO.ItemUom.Add(item.Stock.Uom);
                requestListDTO.Quantity.Add(item.RequestQuantity);
                requestListDTO.Remove.Add(false);
            }

            return requestListDTO;
        }

        public RequestListDTO RemoveFromRequestListDTO(RequestListDTO requestListDTO)
        {
            for (int i = 0; i < requestListDTO.ItemDescription.Count; i++)
            {
                if (requestListDTO.Remove[i] == true)
                {
                    requestListDTO.ItemDescription.RemoveAt(i);
                    requestListDTO.Quantity.RemoveAt(i);
                    requestListDTO.ItemUom.RemoveAt(i);
                    requestListDTO.Remove.RemoveAt(i);
                    i--;       
                }
            }

            return requestListDTO;
        }

        public RequestListDTO AddToRequestListDTO(RequestListDTO requestListDTO, string description, string uom)
        {
            requestListDTO.ItemDescription.Add(description);
            requestListDTO.ItemUom.Add(uom);
            requestListDTO.Quantity.Add(1);
            requestListDTO.Remove.Add(false);

            return requestListDTO;
        }

        public List<StationeryRequestDetailsEF> FindRequestDetailsByRequestId(string requestId)
        {
            List<StationeryRequestDetailsEF> requestList = rndEFF.FindAllStationeryRequestDetailsByRequestId(requestId);

            return requestList;
        }

        public StationeryRequestEF FindRequestById(string requestId)
        {
            StationeryRequestEF request = rndEFF.FindRequestById(requestId);

            return request;
        }

        public void UpdateRequestAfterDisbursement(List<StationeryDisbursementDetailsEF> details, int disbursementId)
        {
            StationeryDisbursementEF disbursement = rndEFF.FindDisbursementById(disbursementId);
            foreach (StationeryDisbursementDetailsEF d in details)
            {
                // list of request in a specific disbursement by departmentCode & request details status & itemCode
                List<StationeryRequestDetailsEF> requests = rndEFF.FindAllRequestDetailsByStatusAndDepartmentCode("Processed", d.ItemCode, disbursement.DepartmentCode);
                if (d.DisbursedQuantity == 0)
                {
                    foreach (StationeryRequestDetailsEF r in requests)
                    {
                        r.RetrievedQuantity = null;
                        r.FulfilmentStatus = "Outstanding";
                        rndEFF.SaveRequestDetails(r);
                    }
                }
                // d.disbursedQuantity != 0
                else
                {
                    if (d.RequestQuantity == d.DisbursedQuantity)
                    {
                        foreach (StationeryRequestDetailsEF r in requests)
                        {
                            if (r.RetrievedQuantity + r.FulfilledQuantity == r.RequestQuantity)
                            {
                                r.FulfilledQuantity = r.RequestQuantity;
                                r.RetrievedQuantity = null;
                                r.FulfilmentStatus = "Completed";
                                rndEFF.SaveRequestDetails(r);
                            }
                            else if (r.RetrievedQuantity + r.FulfilledQuantity < r.RequestQuantity)
                            {
                                r.FulfilledQuantity = (int)r.RetrievedQuantity + r.FulfilledQuantity;
                                r.RetrievedQuantity = null;
                                r.FulfilmentStatus = "Outstanding";
                                rndEFF.SaveRequestDetails(r);
                            }
                        }
                    }
                    //insufficient stock
                    else if (d.RequestQuantity > d.DisbursedQuantity)
                    {
                        List<StationeryRequestDetailsEF> sortedRequests = requests.OrderBy(x => x.Request.RequestDate).ToList<StationeryRequestDetailsEF>();
                        int itemCount = d.DisbursedQuantity;
                        foreach (StationeryRequestDetailsEF r in sortedRequests)
                        {
                            if (r.RetrievedQuantity <= itemCount && r.RetrievedQuantity != 0)
                            {
                                itemCount = itemCount - (int)r.RetrievedQuantity;
                                if (r.RetrievedQuantity + r.FulfilledQuantity == r.RequestQuantity)
                                {
                                    r.FulfilledQuantity = r.RequestQuantity;
                                    r.RetrievedQuantity = null;
                                    r.FulfilmentStatus = "Completed";
                                    rndEFF.SaveRequestDetails(r);
                                }
                                else if (r.RetrievedQuantity + r.FulfilledQuantity < r.RequestQuantity)
                                {
                                    r.FulfilledQuantity = r.FulfilledQuantity + (int)r.RetrievedQuantity;
                                    r.RetrievedQuantity = null;
                                    r.FulfilmentStatus = "Outstanding";
                                    rndEFF.SaveRequestDetails(r);
                                }
                            }
                            // insufficient stock for that request (outstandingQty > itemCount)
                            else if (r.RetrievedQuantity > itemCount & itemCount != 0)
                            {
                                r.FulfilledQuantity = r.FulfilledQuantity + itemCount;
                                r.RetrievedQuantity = null;
                                r.FulfilmentStatus = "Outstanding";
                                rndEFF.SaveRequestDetails(r);
                                itemCount = 0;
                            }
                            else if (itemCount == 0 || r.RetrievedQuantity == 0)
                            {
                                r.RetrievedQuantity = null;
                                r.FulfilmentStatus = "Outstanding";
                                rndEFF.SaveRequestDetails(r);
                            }
                        }
                    }
                }
            }

        }

        public void CreateRequest(StaffEF staff, RequestListDTO requestListDTO)
        {
            StationeryRequestEF newRequest; string requestId; StationeryRequestDetailsEF requestItem; bool existingItem;
            List<StationeryRequestDetailsEF> requestList = new List<StationeryRequestDetailsEF>();
            long unixTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            int year = DateTime.Now.Year;
            
            if (requestListDTO.RequestId != null)
            {
                requestId = requestListDTO.RequestId;
                List<StationeryRequestDetailsEF> currentList = rndEFF.FindAllStationeryRequestDetailsByRequestId(requestId);
                for (int i = 0; i < currentList.Count; i++)
                {
                    existingItem = false;
                    for (int j = 0; j < requestListDTO.ItemDescription.Count; j++)
                    {
                        if (currentList[i].Stock.Description == requestListDTO.ItemDescription[j])
                        {
                            requestItem = rndEFF.FindRequestDetailsByRequestIdAndItemCode(requestId, currentList[i].Stock.ItemCode);
                            requestItem.RequestQuantity = requestListDTO.Quantity[j];
                            requestList.Add(requestItem);
                            existingItem = true;
                            requestListDTO.Remove[j] = true;
                            break;
                        }
                    }

                    if(!existingItem)
                    {
                        rndEFF.DropRequestDetails(currentList[i]);
                    }
                }

                requestListDTO = RemoveFromRequestListDTO(requestListDTO);

                for (int k = 0; k < requestListDTO.ItemDescription.Count; k++)
                {
                    string stockId = stockEFF.FindStockByDescription(requestListDTO.ItemDescription[k]).ItemCode;
                    requestItem = new StationeryRequestDetailsEF(requestId, stockId, requestListDTO.Quantity[k]);
                    requestList.Add(requestItem);
                }
            }
            else
            {
                int num = rndEFF.FindLastRequestNoByYearAndDepartment(staff.DepartmentCode, year) + 1;
                requestId = staff.DepartmentCode + "/" + year + "/" + num;

                for (int i = 0; i < requestListDTO.ItemDescription.Count; i++)
                {
                    string stockId = stockEFF.FindStockByDescription(requestListDTO.ItemDescription[i]).ItemCode;
                    requestItem = new StationeryRequestDetailsEF(requestId, stockId, requestListDTO.Quantity[i]);
                    requestList.Add(requestItem);
                }
            }

            newRequest = new StationeryRequestEF(requestId, staff.StaffId, unixTimestamp);
            rndEFF.SaveRequestAndRequestDetails(newRequest, requestList);

            StaffEF currentAuthority = staff.Department.Authority;
            if (currentAuthority.Email != null)
            {
                string subject = "Pending Stationery Request";
                string body = "Request #" + requestId + " has been submitted by " + staff.Name + " for your approval. Please click " +
                "<a href='http://localhost/StationeryStore/ApproveRequest/ViewRequest/?requestId=" + requestId + "'>" + "here</a> to view the details.";
                Email.SendEmail(currentAuthority.Email, subject, body);
            }
        }

        //RETREIVAL

        public void CreateRetrieval(StationeryRetrievalEF retrieval)
        {
            rndEFF.AddToRetrieval(retrieval);
        }

        public List<StationeryRetrievalEF> FindAllRetrieval()
        {
            return rndEFF.FindAllRetrieval();
        }

        public StationeryRetrievalEF FindRetrievalById(int id)
        {
            return rndEFF.FindRetrievalById(id);
        }

        public void SaveRetrieval(StationeryRetrievalEF retrieval)
        {
            rndEFF.SaveRetrieval(retrieval);
        }

        public List<RetrievalItemDTO> ViewRetrievalListById(int retrievalId)
        {
            StationeryRetrievalEF retrieval = rndEFF.FindRetrievalById(retrievalId);
            List<StationeryDisbursementDetailsEF> details = rndEFF.FindDisbursementDetailsByRetrievalId(retrievalId);

            // group by item code
            List<RetrievalItemDTO> retrievalList = new List<RetrievalItemDTO>();
            var items = from d in details
                        group d by d.ItemCode into itemGroup
                        select new
                        {
                            ItemCode = itemGroup.Key,
                            OutstandingQuantity = itemGroup.Sum(d => d.RequestQuantity),
                            RetrievedQty = itemGroup.Sum(a => a.RetrievedQuantity)
                        };
            foreach (var item in items)
            {
                RetrievalItemDTO rItem = new RetrievalItemDTO()
                {
                    ItemCode = item.ItemCode,
                    ItemDescription = stockEFF.FindStockByItemCode(item.ItemCode).Description,
                    TotalOutstandingQty = item.OutstandingQuantity,
                    Bin = stockEFF.FindStockByItemCode(item.ItemCode).Bin,
                    RetrievedQty = item.RetrievedQty
                };
                retrievalList.Add(rItem);
            }
            return retrievalList;
        }

        public bool FindOngoingRetrieval()
        {
            return rndEFF.FindAllRetrieval().Where(x => x.Status == "Processing").ToList().Any();
        }

        public StationeryRetrievalEF FindRetrievalByStatus(string status)
        {
            return rndEFF.FindRetrievalByStatus(status);
        }



        //DISBURSE
        public void CreateDisbursement(StationeryDisbursementEF disbursement)
        {
            rndEFF.AddToDisbursement(disbursement);
        }

        public void CreateDisbursementDetails(StationeryDisbursementDetailsEF details)
        {
            rndEFF.AddToDisbursementDetails(details);
        }

        public void GenerateDisbursementList(List<StationeryRequestDetailsEF> outstandingRequests, int retrievalId)
        {
            // Create Disbursement
            // Get list of distinct depts with outstanding requests
            DepartmentService deptService = new DepartmentService();
            List<DepartmentEF> depts = deptService.FindDistinctDepartments(outstandingRequests);

            // Create disbursement for each department
            foreach (DepartmentEF dept in depts)
            {
                // get the department's outstanding request details
                List<StationeryRequestDetailsEF> reqByDept = outstandingRequests
                    .Where(x => x.Request.Staff.Department == dept)
                    .ToList<StationeryRequestDetailsEF>();

                StationeryDisbursementEF disbursement = new StationeryDisbursementEF()
                {
                    RetrievalId = retrievalId,
                    DepartmentCode = dept.DepartmentCode,
                    Status = "Processing",
                    CollectionRepId = dept.DepartmentRepresentativeId
                };
                CreateDisbursement(disbursement);

                //foreach item in the dept disbursement
                //create disbursement details
                var disbursementDetails = from d in reqByDept
                                          group d by d.ItemCode
                                          into grp
                                          select new
                                          {
                                              grp.Key,
                                              RequestQuantity = grp.Sum(d => d.RequestQuantity - d.FulfilledQuantity)
                                          };

                List<StationeryDisbursementDetailsEF> detailsList = new List<StationeryDisbursementDetailsEF>();
                foreach (var dis in disbursementDetails)
                {
                    StationeryDisbursementDetailsEF det = new StationeryDisbursementDetailsEF()
                    {
                        DisbursementId = disbursement.DisbursementId,
                        ItemCode = dis.Key,
                        RequestQuantity = dis.RequestQuantity
                    };
                    CreateDisbursementDetails(det);
                }
            }
        }

        public void UpdateRetrievedQuantities(int retrievalId)
        {
            // get list of request details with status = processed
            List<StationeryRequestDetailsEF> processedRequests = rndEFF.FindAllRequestDetailsByStatus("Processed");
            // group them by department and itemcode
            var reqs = from r in processedRequests
                       group r by new
                       {
                           r.Request.Staff.DepartmentCode,
                           r.ItemCode
                       }
                       into grp
                       select new
                       {
                           grp.Key.DepartmentCode,
                           grp.Key.ItemCode,
                           RetrievedQuantity = grp.Sum(r => r.RetrievedQuantity)
                       };

            // update disbursement status to Retrieved
            List<StationeryDisbursementEF> disbursements = rndEFF.FindDisbursementsByRetrievalId(retrievalId);
            List<StationeryDisbursementDetailsEF> disbursementDetails = rndEFF.FindDisbursementDetailsByRetrievalId(retrievalId);
            foreach (StationeryDisbursementEF disburse in disbursements)
            {
                //get sum
                var sum = reqs.Where(x => x.DepartmentCode == disburse.DepartmentCode).Sum(x => x.RetrievedQuantity);
                if (sum == 0)
                {
                    // nothing to disburse
                    disburse.Status = "Cancelled";
                    disburse.CollectionRepId = disburse.Department.DepartmentRepresentativeId;
                    rndEFF.SaveDisbursement(disburse);
                    // update the request details
                    List<StationeryRequestDetailsEF> requests = rndEFF.FindAllProcessedRequestDetailsByDepartmentCode("Processed", disburse.DepartmentCode);
                    foreach(StationeryRequestDetailsEF r in requests)
                    {
                        r.FulfilmentStatus = "Outstanding";
                        r.RetrievedQuantity = null;
                        rndEFF.SaveRequestDetails(r);
                    }
                }
                else
                {
                    disburse.Status = "Retrieved";
                    disburse.CollectionRepId = disburse.Department.DepartmentRepresentativeId;
                    rndEFF.SaveDisbursement(disburse);
                }
            }

            // insert retrieved quantity into disbursement details
            foreach (var req in reqs)
            {
                // use departmentCode to find disbursementId
                int disbursementId = disbursements.SingleOrDefault(x => x.DepartmentCode == req.DepartmentCode).DisbursementId;
                // use disbursementId, itemCode to find the disbursement details
                StationeryDisbursementDetailsEF d = disbursementDetails.SingleOrDefault(y => y.DisbursementId == disbursementId && y.ItemCode == req.ItemCode);
                // update the disbursement retrieved quantity
                d.RetrievedQuantity = (int)req.RetrievedQuantity;
                rndEFF.SaveDisbursementDetails(d);
            }
        }
        public List<StationeryDisbursementEF> FindAllDisbursements()
        {
            return rndEFF.FindAllDisbursement().OrderByDescending(x => x.DisbursementId).ToList();
        }

        public StationeryDisbursementEF FindDisbursementById(int disbursementId)
        {
            return rndEFF.FindDisbursementById(disbursementId);
        }

        public List<StationeryDisbursementDetailsEF> FindDisbursementDetailsByDisbursementId(int disbursementId)
        {
            return rndEFF.FindDisbursementDetailsByDisbursementId(disbursementId);
        }

        public void UpdateDisbursedQuantities(List<StationeryDisbursementDetailsEF> details, int disbursementId, 
            int collectionRepId, int storeClerkId)
        {
            // update disbursement details' Disbursed Quantity 
            foreach (StationeryDisbursementDetailsEF d in details)
            {
                rndEFF.SaveDisbursementDetails(d);
            }
            // set disbursement status to disbursed
            StationeryDisbursementEF disbursement = rndEFF.FindDisbursementById(disbursementId);
            disbursement.Status = "Disbursed";
            disbursement.CollectionRepId = collectionRepId;
            disbursement.StoreClerkId = storeClerkId;
            rndEFF.SaveDisbursement(disbursement);
        }

        public bool FindOngoingDisbursement()
        {
            return rndEFF.FindAllDisbursement().Where(x => x.Status == "Retrieved").ToList().Any();
        }

        public List<StationeryDisbursementEF> FindDisbursementsByRetrievalId(int retrievalId)
        {
            return rndEFF.FindDisbursementsByRetrievalId(retrievalId);
        }

        public void UpdateDisbursementDate(int retrievalId, DateTimeOffset date)
        {
            List<StationeryDisbursementEF> disbursements = rndEFF.FindDisbursementsByRetrievalId(retrievalId);
            foreach (StationeryDisbursementEF d in disbursements)
            {
                d.DateDisbursed = Timestamp.dateToUnixTimestamp(date);
                rndEFF.SaveDisbursement(d);
            }
        }

        public List<StationeryDisbursementEF> FindDisbursementsByStatus(string status)
        {
            return rndEFF.FindDisbursementByStatus(status);
        }
    }
}