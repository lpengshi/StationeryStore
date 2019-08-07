using StationeryStore.Models;
using StationeryStore.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StationeryStore.Filters
{
    public class StoreManagerFilter : ActionFilterAttribute, IAuthorizationFilter
    {
        StaffService staffService = new StaffService();
        public void OnAuthorization(AuthorizationContext context)
        {
            StaffEF staff = staffService.GetStaff();

            if (staff.Role.Description != "Store Manager")
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Error" },
                        { "action", "StoreRoleError" }
                    }
                );
            }
        }
    }
}