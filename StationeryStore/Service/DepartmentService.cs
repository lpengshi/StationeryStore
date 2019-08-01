using StationeryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.Service
{
    public class DepartmentService
    {
        DepartmentEF departmentEF = new DepartmentEF();

        public List<DepartmentEF> FindDistinctDepartments(List<StationeryRequestDetailsEF> requests)
        {
            List<DepartmentEF> depts = (
                from r in requests
                select r.Request.Staff.Department)
                .Distinct().ToList<DepartmentEF>();

            return depts;
        }
    }
}