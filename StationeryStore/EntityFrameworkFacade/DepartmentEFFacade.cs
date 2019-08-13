using StationeryStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StationeryStore.EntityFrameworkFacade
{
    public class DepartmentEFFacade
    {
        StoreContext context = new StoreContext();

        public List<DepartmentEF> FindAllDepartments()
        {
            return context.Departments.ToList();
        }

        public DepartmentEF FindDepartmentByCode(string code)
        {
            return context.Departments.Find(code);
        }

        public void AddToCollectionPoint(CollectionPointEF collectionPoint)
        {
            context.CollectionPoints.Add(collectionPoint);
            context.SaveChanges();
        }

        public void RemoveFromCollectionPoint(CollectionPointEF collectionPoint)
        {
            context.CollectionPoints.Remove(collectionPoint);
            context.SaveChanges();
        }

        public List<CollectionPointEF> FindAllCollectionPoints()
        {
            return context.CollectionPoints.ToList();
        }

        public void SaveDepartment(DepartmentEF department)
        {
            var existingDepartment = context.Departments.Find(department.DepartmentCode);
            if (existingDepartment == null)
            {
                context.Departments.Add(department);
            }
            else
            {
                context.Entry(existingDepartment).CurrentValues.SetValues(department);
            }
            context.SaveChanges();
        }

        public CollectionPointEF FindCollectionPointById(int id)
        {
            return context.CollectionPoints.Find(id);
        }

        public void SaveCollectionPoint(CollectionPointEF point)
        {
            var existingPoint = context.CollectionPoints.Find(point.CollectionPointId);
            if (existingPoint != null)
            {
                context.Entry(existingPoint).CurrentValues.SetValues(point);
                context.SaveChanges();
            }
        }

        public StaffEF FindDepartmentHeadByDepartmentCode(string departmentCode)
        {
            return context.Staff.
                Where(a => a.DepartmentCode == departmentCode && a.Role.Description == "Department Head")
                .SingleOrDefault();
        }
    }
}