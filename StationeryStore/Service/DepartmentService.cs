using StationeryStore.EntityFrameworkFacade;
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

            }
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
    }
}