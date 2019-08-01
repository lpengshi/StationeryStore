using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Routing;
using StationeryStore.Models;
using StationeryStore.Service;

namespace StationeryStore.Filters
{
    public class AuthorizeFilter: ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext ac)
        {
            bool isAuthOK = false;
            StaffService staffService = new StaffService();
            if (ac.HttpContext.Session["sessionId"] != null)
            {
                string sessionId = ac.HttpContext.Session["sessionId"].ToString();
                StaffEF staff = staffService.FindStaffBySessionId(sessionId);

                if (staff != null)
                {
                    isAuthOK = true;
                }
            }

            if (!isAuthOK)
            {
                ac.Result = new RedirectToRouteResult(
                    new RouteValueDictionary{
                        { "controller", "Login" },
                        {"action", "Index" }
                    });
            }
        }
    }
}