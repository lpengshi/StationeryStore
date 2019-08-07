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

        public void RemoveFromStaff(StaffEF staff)
        {
            context.Staff.Remove(staff);
            context.SaveChanges();
        }

        public List<StaffEF> FindAllStaff()
        {
            return context.Staff.ToList();
        }
        public List<StaffEF> FindAllStaffUsingNativeSQL()
        {
            return context.Staff.SqlQuery("Select * from Staff").ToList<StaffEF>();
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
            return context.Staff.SingleOrDefault(a => a.SessionId == sessionid);
        }

        public List<StaffEF> FindAllStaffByDepartmentCode(string departmentid)
        {
            return context.Staff.Where(a => a.DepartmentCode == departmentid).ToList<StaffEF>();
        }


        //ROLES

        public void SaveRole(RoleEF role)
        {
            var existingRole = context.Roles.Find(role.RoleId);
            if (existingRole == null)
            {
                context.Roles.Add(role);
            }
            else
            {
                context.Entry(existingRole).CurrentValues.SetValues(role);
            }
            context.SaveChanges();
        }
        public RoleEF FindRoleByRoleId(int id)
        {
            return context.Roles.Find(id);
        }

        public void AddToRole(RoleEF role)
        {
            context.Roles.Add(role);
            context.SaveChanges();
        }

        public void RemoveFromRole(RoleEF role)
        {
            context.Roles.Remove(role);
            context.SaveChanges();
        }

        public List<RoleEF> FindAllRoles()
        {
            return context.Roles.ToList();
        }
    }

}

