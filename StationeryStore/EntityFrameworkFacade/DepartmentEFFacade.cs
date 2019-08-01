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

        public void AddToDepartment(DepartmentEF department)
        {
            context.Departments.Add(department);
            context.SaveChanges();
        }

        public List<DepartmentEF> FindAllDepartments()
        {
            return context.Departments.ToList();
        }

        public DepartmentEF FindDepartmentByCode(int code)
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
    }
}