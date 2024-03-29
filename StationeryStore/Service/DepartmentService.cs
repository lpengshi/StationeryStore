﻿using StationeryStore.EntityFrameworkFacade;
using StationeryStore.Models;
using StationeryStore.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Service
{
    public class DepartmentService
    {
        DepartmentEFFacade departmentEFF = new DepartmentEFFacade();
        StaffService staffService = new StaffService();

        public List<DepartmentEF> FindDistinctDepartments(List<StationeryRequestDetailsEF> requests)
        {
            List<DepartmentEF> depts = (
                from r in requests
                select r.Request.Staff.Department)
                .Distinct().ToList<DepartmentEF>();

            return depts;
        }

        public List<CollectionPointEF> FindAllCollectionPoints()
        {
            List<CollectionPointEF> collectionPoints = departmentEFF.FindAllCollectionPoints();

            return collectionPoints;
        }

        public void UpdateDepartmentCollection(ManageCollectionDTO manageCollectionDTO)
        {
            if (manageCollectionDTO != null)
            {
                DepartmentEF department = departmentEFF.FindDepartmentByCode(manageCollectionDTO.Department);

                department.CollectionPointId = manageCollectionDTO.CollectionPointId;

                if (manageCollectionDTO.DepartmentRepId != null)
                {
                    department.DepartmentRepresentativeId = manageCollectionDTO.DepartmentRepId;
                }
              
                departmentEFF.SaveDepartment(department);

                List<StaffEF> clerkList = staffService.FindStaffByRole(3);
                foreach (StaffEF clerk in clerkList)
                {
                    if (clerk.Email != null)
                    {
                        string subject = "Update of Collection Point / Department Rep";
                        string body = department.DepartmentName + " has updated their collection point(" + department.CollectionPoint.Location + ", " + 
                            department.CollectionPoint.CollectionTime + ") and department representative(" +
                            department.DepartmentRepresentative.Name + ")";
                        Email.SendEmail(clerk.Email, subject, body);
                    }
                }
            }
        }

        public void CheckDelegation(DepartmentEF department)
        {
            long currentTimestamp = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            if (department.DelegationEndDate < currentTimestamp || department.DelegationEndDate == null)
            {
                StaffEF deptHead = FindDepartmentHead(department.DepartmentCode);
                RemoveStaffDelegation(deptHead);
                }
        }

        public StaffEF FindDepartmentHead(string departmentCode)
        {
            return departmentEFF.FindDepartmentHeadByDepartmentCode(departmentCode);
        }

        public void DelegateStaff(ManageDelegationDTO manageDelegationDTO)
        {
            long delegationStartDate = Timestamp.dateToUnixTimestamp(manageDelegationDTO.DelegationStartDate);
            long delegationEndDate = Timestamp.dateToUnixTimestamp(manageDelegationDTO.DelegationEndDate) + 86399;

            DepartmentEF department = departmentEFF.FindDepartmentByCode(manageDelegationDTO.DepartmentCode);
            department.AuthorityId = manageDelegationDTO.AuthorityId;
            department.DelegationStartDate = delegationStartDate;
            department.DelegationEndDate = delegationEndDate;

            departmentEFF.SaveDepartment(department);
        }

        public void RemoveStaffDelegation(StaffEF staff)
        {
            DepartmentEF department = staff.Department;

            department.AuthorityId = staff.StaffId; 
            department.DelegationStartDate = null;
            department.DelegationEndDate = null;

            departmentEFF.SaveDepartment(department);
        }

        public CollectionPointEF FindCollectionPointById(int id)
        {
            return departmentEFF.FindCollectionPointById(id);
        }

        public void UpdateCollectionPoint(CollectionPointEF point)
        {
            departmentEFF.SaveCollectionPoint(point);
        }
    }
}