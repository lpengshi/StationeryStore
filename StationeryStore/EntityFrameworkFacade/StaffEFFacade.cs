using StationeryStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace StationeryStore.EntityFrameworkFacade
{
    public class StaffEFFacade
    {
        StoreContext context = new StoreContext();
        public void AddToStaff(StaffEF staff)
        {
            context.Staff.Add(staff);
            context.SaveChanges();
        }

        public void SaveStaff(StaffEF staff)
        {
            var existingStaff = context.Staff.Find(staff.StaffId);
            if (existingStaff == null)
            {
                context.Staff.Add(staff);
            }
            else
            {
                context.Entry(existingStaff).CurrentValues.SetValues(staff);
            }
            context.SaveChanges();
        }

        public StaffEF FindStaffByStaffId(int id)
        {
            var staff = context.Staff.Find(id);
            staff.Role = context.Roles.SingleOrDefault(a => a.RoleId == staff.RoleId);

            return staff;
        }

        public StaffEF FindStaffByUsername(string username)
        {
            return context.Staff.SingleOrDefault(a => a.Username == username);
        }

        public StaffEF FindStaffBySessionId(string sessionid)
        {
            StoreContext newContext = new StoreContext();
            return newContext.Staff.SingleOrDefault(a => a.SessionId == sessionid);
        }

        public List<StaffEF> FindAllStaffByDepartmentCode(string departmentid)
        {
            return context.Staff.Where(a => a.DepartmentCode == departmentid).ToList<StaffEF>();
        }

        public List<StaffEF> FindStaffByRoleId(int id)
        {
            return context.Staff.Where(a => a.RoleId == id).ToList();
        }

    }

}

