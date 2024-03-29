﻿using StationeryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace StationeryStore.EntityFrameworkFacade
{
    public class RequestAndDisburseEFFacade
    {
        StoreContext context = new StoreContext();

        public List<StationeryDisbursementEF> FindDisbursementByDepartmentCode(string departmentCode)
        {
           return context.StationeryDisbursements
                .Where(a => a.DepartmentCode == departmentCode)
                .ToList<StationeryDisbursementEF>();
        }

        public List<RequestTemplateEF> FindRequestTemplateByStaffId(int staffId)
        {
            return context.RequestTemplates
                .Where(a => a.StaffId == staffId)
                .ToList<RequestTemplateEF>();
        }

        public List<StationeryRequestEF> FindRequestsByDepartmentAndStatus(string departmentCode, string status)
        {
            if (status == "all")
            {
                return context.StationeryRequests
               .Where(a => a.RequestId.StartsWith(departmentCode))
               .OrderBy(a => a.RequestDate)
               .ToList<StationeryRequestEF>();
            }

            return context.StationeryRequests
                .Where(a => a.RequestId.StartsWith(departmentCode) && a.Status == status)
                .OrderBy(a => a.RequestDate)
                .ToList<StationeryRequestEF>();
        }

        public RequestTemplateEF FindRequestTemplateByTemplateId(int templateId)
        {
            return context.RequestTemplates
                .Where(a => a.TemplateId == templateId)
                .SingleOrDefault();
        }

        public void SaveRequestTemplate(RequestTemplateEF requestTemplate)
        {
            var existingTemplate = context.RequestTemplates.Find(requestTemplate.TemplateId);
            if (existingTemplate == null)
            {
                context.RequestTemplates.Add(requestTemplate);
            }
            else
            {
                context.Entry(existingTemplate).CurrentValues.SetValues(requestTemplate);
            }
            context.SaveChanges();
        }

        public List<RequestTemplateDetailsEF> FindRequestTemplateDetailsByTemplateId(int templateId)
        {
            return context.RequestTemplateDetails
                .Where(a => a.RequestTemplateId == templateId)
                .ToList<RequestTemplateDetailsEF>();
        }

        public List<StationeryRequestEF> FindRequestsByStaffIdAndStatus(int staffId, string status)
        {
            return context.StationeryRequests
                .Where(a => a.StaffId == staffId && a.Status == status)
                .OrderBy(a => a.RequestDate)
                .ToList<StationeryRequestEF>();
        }

        public void DeleteRequestTemplate(int templateId)
        {
            var existingTemplateDetails = this.FindRequestTemplateDetailsByTemplateId(templateId);

            if (existingTemplateDetails != null)
            {
                context.RequestTemplateDetails.RemoveRange(existingTemplateDetails);
            }

            var existingTemplate = context.RequestTemplates.Find(templateId);
            if (existingTemplate != null)
            {
                context.RequestTemplates.Remove(existingTemplate);
            }

            context.SaveChanges();
        }

        public List<StationeryRequestEF> FindAllStationeryRequestByStaffId(int staffId)
        {
            return context.StationeryRequests
                .Where(a => a.StaffId == staffId)
                .OrderBy(a => a.RequestDate)
                .ToList<StationeryRequestEF>();
        }

        public void SaveRequestDetails(StationeryRequestDetailsEF details)
        {
            var existingRequestDet = context.StationeryRequestDetails.Find(details.RequestDetailsId);
            if (existingRequestDet != null)
            {
                existingRequestDet.RetrievedQuantity = details.RetrievedQuantity;
                existingRequestDet.FulfilledQuantity = details.FulfilledQuantity;
                existingRequestDet.FulfilmentStatus = details.FulfilmentStatus;
                context.SaveChanges();
            }
        }

        public int FindDisbursedQuantityByItemCodeAndTimePeriod(string itemCode, long startMonth, long endMonth)
        {
            int itemCount = -1;

            List<StationeryDisbursementEF> disbursedList = context.StationeryDisbursements
                .Where(a => a.DateDisbursed >= startMonth && a.DateDisbursed <= endMonth)
                .ToList<StationeryDisbursementEF>();

            if (disbursedList.Count > 0)
            {
                int disbursedQty = 0;

                foreach(StationeryDisbursementEF disbursement in disbursedList)
                {
                    StationeryDisbursementDetailsEF disbursedDetails = context.StationeryDisbursementDetails
                        .Where(a => a.DisbursementId == disbursement.DisbursementId && a.ItemCode == itemCode)
                        .FirstOrDefault();

                    if (disbursedDetails != null)
                    {
                        disbursedQty += disbursedDetails.DisbursedQuantity;
                    }
                }

                if (disbursedQty > 0)
                {
                    itemCount = disbursedQty;
                }

            }

            return itemCount;
        }

        public List<StationeryRequestDetailsEF> FindAllRequestDetailsByStatus(string status)
        {
            return context.StationeryRequestDetails
                .Where(a => a.FulfilmentStatus == status)
                .ToList<StationeryRequestDetailsEF>();
        }

        public List<StationeryRequestDetailsEF> FindAllRequestDetailsByStatusAndItemCode(string status, string itemCode)
        {
            return context.StationeryRequestDetails
                .Where(a => a.FulfilmentStatus == status && a.ItemCode == itemCode)
                .ToList<StationeryRequestDetailsEF>();
        }

        public void SaveRequestAndRequestDetails(StationeryRequestEF newRequest, List<StationeryRequestDetailsEF> requestList)
        {
            var existingRequest = context.StationeryRequests.Find(newRequest.RequestId);
            if (existingRequest == null)
            {
                context.StationeryRequests.Add(newRequest);
            }
            else
            {
                context.Entry(existingRequest).CurrentValues.SetValues(newRequest);
            }

            List<StationeryRequestDetailsEF> updatedList = new List<StationeryRequestDetailsEF>();


            foreach (var item in requestList)
            {
                var existingRequestDetails = context.StationeryRequestDetails.Find(item.RequestDetailsId);
                if (existingRequestDetails == null)
                {
                    updatedList.Add(item);

                } else
                {
                    context.Entry(existingRequestDetails).CurrentValues.SetValues(item);
                }

            }
            context.StationeryRequestDetails.AddRange(updatedList);
            context.SaveChanges();
            
        }

        public int FindLastRequestNoByYearAndDepartment(string departmentCode, int year)
        {
            var num = context.StationeryRequests.Count(a => a.RequestId.StartsWith(departmentCode + "/" + year));

            return num;
        }

        public StationeryRequestEF FindRequestById(string requestId)
        {
            return context.StationeryRequests.Find(requestId);
          
        }

        public List<StationeryRequestDetailsEF> FindAllStationeryRequestDetailsByRequestId(string requestId)
        {
            return context.StationeryRequestDetails.Where(a => a.RequestId == requestId).ToList();
        }

        public RequestTemplateDetailsEF FindRequestTemplateDetailsByTemplateIdAndItemCode(int templateId, string itemCode)
        {
            return context.RequestTemplateDetails
           .Where(a => a.RequestTemplateId == templateId && a.ItemCode == itemCode)
           .SingleOrDefault();
        }

        public StationeryRequestDetailsEF FindRequestDetailsByRequestIdAndItemCode(string requestId, string itemCode)
        {
            return context.StationeryRequestDetails
            .Where(a => a.RequestId == requestId && a.ItemCode == itemCode)
            .SingleOrDefault();
        }

        public void DropRequestDetails(StationeryRequestDetailsEF stationeryRequestDetails)
        {
            context.Entry(stationeryRequestDetails).State = EntityState.Deleted;
            context.SaveChanges();
        }
        public void DropRequestTemplateDetails(RequestTemplateDetailsEF requestTemplateDetails)
        {
            context.Entry(requestTemplateDetails).State = EntityState.Deleted;
            context.SaveChanges();
        }
        public List<StationeryRequestDetailsEF> FindAllRequestDetailsByStatusAndDepartmentCode(string status, string itemCode, string departmentCode)
        {
            return context.StationeryRequestDetails
                .Where(a => a.FulfilmentStatus == status 
                && a.Request.Staff.DepartmentCode == departmentCode 
                && a.ItemCode == itemCode)
                .ToList<StationeryRequestDetailsEF>();
        }

        public List<StationeryRequestDetailsEF> FindAllProcessedRequestDetailsByDepartmentCode(string status, string departmentCode)
        {
            return context.StationeryRequestDetails
                .Where(a => a.FulfilmentStatus == status
                && a.Request.Staff.DepartmentCode == departmentCode)
                .ToList<StationeryRequestDetailsEF>();
        }

        public void SaveRequestTemplateDetails(List<RequestTemplateDetailsEF> requestTemplateList)
        {
            foreach (var item in requestTemplateList)
            {
                var existingRequestTemplateDetails = FindRequestTemplateDetailsByTemplateIdAndItemCode(item.RequestTemplateId, item.ItemCode);
                if (existingRequestTemplateDetails == null)
                {
                    context.RequestTemplateDetails.Add(item);
                }
                else
                {
                    context.Entry(existingRequestTemplateDetails).CurrentValues.SetValues(item);
                }

            }
            context.SaveChanges();
        }


        //RETRIEVAL
        public void AddToRetrieval(StationeryRetrievalEF retrieval)
        {
            context.StationeryRetrievals.Add(retrieval);
            context.SaveChanges();
        }

        public void SaveRetrieval(StationeryRetrievalEF retrieval)
        {
            var existingRetrieval = context.StationeryRetrievals.Find(retrieval.RetrievalId);
            if (existingRetrieval != null)
            {
                context.Entry(existingRetrieval).CurrentValues.SetValues(retrieval);
                context.SaveChanges();
            }
        }

        public List<StationeryRetrievalEF> FindAllRetrieval()
        {
            return context.StationeryRetrievals.ToList();
        }

        public StationeryRetrievalEF FindRetrievalById(int id)
        {
            return context.StationeryRetrievals.Find(id);
        }

        public StationeryRetrievalEF FindRetrievalByStatus(string status)
        {
            return context.StationeryRetrievals.SingleOrDefault(x => x.Status == status);
        }


        //DISBURSEMENT
        public void AddToDisbursement(StationeryDisbursementEF disbursement)
        {
            context.StationeryDisbursements.Add(disbursement);
            context.SaveChanges();
        }

        public void SaveDisbursement(StationeryDisbursementEF disbursement)
        {
            var existingDisbursement = context.StationeryDisbursements.Find(disbursement.DisbursementId);
            if (existingDisbursement != null)
            {
                context.Entry(existingDisbursement).CurrentValues.SetValues(disbursement);
                context.SaveChanges();
            }
        }

        public List<StationeryDisbursementEF> FindAllDisbursement()
        {
            return context.StationeryDisbursements.ToList();
        }

        public StationeryDisbursementEF FindDisbursementById(int id)
        {
            return context.StationeryDisbursements.Find(id);
        }

        public List<StationeryDisbursementEF> FindDisbursementsByRetrievalId(int retrievalId)
        {
            return context.StationeryDisbursements
                .Where(a => a.RetrievalId == retrievalId)
                .ToList<StationeryDisbursementEF>();
        }

        public List<StationeryDisbursementEF> FindDisbursementByStatus(string status)
        {
            return context.StationeryDisbursements.Where(a => a.Status == status).ToList();
        }

        public StationeryDisbursementEF StationFindDisbursementByStatusAndStaffId(int staffId, string status)
        {
            return context.StationeryDisbursements
                    .Where(a => a.CollectionRepId == staffId && a.Status == status).First();

        }

        public void AddToDisbursementDetails(StationeryDisbursementDetailsEF details)
        {
            context.StationeryDisbursementDetails.Add(details);
            context.SaveChanges();
        }
        public void SaveDisbursementDetails(StationeryDisbursementDetailsEF details)
        {
            var existingDisbursementDetails = context.StationeryDisbursementDetails.Find(details.DisbursementDetailsId);
            if (existingDisbursementDetails != null)
            {
                context.Entry(existingDisbursementDetails).CurrentValues.SetValues(details);
                context.SaveChanges();
            }
        }

        public List<StationeryDisbursementDetailsEF> FindDisbursementDetailsByDisbursementId(int disbursementId)
        {
            return context.StationeryDisbursementDetails
                .Where(x => x.DisbursementId == disbursementId)
                .ToList<StationeryDisbursementDetailsEF>();
        }

        public List<StationeryDisbursementDetailsEF> FindDisbursementDetailsByRetrievalId(int retrievalId)
        {
            return context.StationeryDisbursementDetails
                .Where(a => a.StationeryDisbursement.RetrievalId == retrievalId)
                .ToList<StationeryDisbursementDetailsEF>();
        }
    }
}