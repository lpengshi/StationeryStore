using StationeryStore.EntityFrameworkFacade;
using StationeryStore.Models;
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