using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StationeryStore.EntityFrameworkFacade;
using StationeryStore.Models;

namespace StationeryStore.Service
{
    public class StaffService
    {
        StaffEFFacade staffEFF = new StaffEFFacade();

        public StaffEF FindStaffByUsername(string username)
        {
            StaffEF staff = staffEFF.FindStaffByUsername(username);
            return staff;
        }

        public StaffEF GetStaff()
        {
            StaffEF staff = null;

            if (HttpContext.Current.Session["sessionId"] != null)
            {
                string sessionId = HttpContext.Current.Session["sessionId"].ToString();
                staff = this.FindStaffBySessionId(sessionId);
            }

            return staff;
        }

        public List<StaffEF> FindAllEmployeeByDepartmentCode(string departmentCode)
        {
            List<StaffEF> staffList = staffEFF.FindAllStaffByDepartmentCode(departmentCode);
            List<StaffEF> employeeList = new List<StaffEF>();

            foreach (StaffEF staff in staffList)
            {
                if (staff.Role.Description == "Employee")
                {
                    employeeList.Add(staff);
                }
            }
            return employeeList;
        }

        public StaffEF FindStaffBySessionId(string sessionId)
        {
            StaffEF staff = staffEFF.FindStaffBySessionId(sessionId);
            return staff;
        }
        
        internal void Logout(StaffEF staff)
        {
            staff.SessionId = null;
            staffEFF.SaveStaff(staff);
        }

        public string CreateSession(StaffEF staff)
        {
            string sessionId = Guid.NewGuid().ToString();
            staff.SessionId = sessionId;
            staffEFF.SaveStaff(staff);
            return sessionId;
        }

        public List<StaffEF> FindStaffByRole(int roleId)
        {
            return staffEFF.FindStaffByRoleId(roleId);
        }
    }
}