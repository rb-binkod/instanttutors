using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InstantTutors.Helpers
{
    public class RedirectLoginFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            // First check if authentication succeed and user authenticated:           

            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                bool IsAdmin = filterContext.HttpContext.User.IsInRole("Admin");

                //Then check for user role(s) and assign view accordingly, don't forget the 
                //[Authorize(Roles = "YourRoleHere")] on your controller / action
                if (filterContext.HttpContext.User.IsInRole("Admin"))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                    (new
                    {
                        area = "Admin",
                        controller = "Dashboard",
                        action = "Index"
                    }));
                }
                else if (filterContext.HttpContext.User.IsInRole("Tutor"))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                    (new
                    {
                        area = "Tutor",
                        controller = "Dashboard",
                        action = "Index"
                    }));
                }
                else if (filterContext.HttpContext.User.IsInRole("Student"))
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                    (new
                    {
                        area = "Student",
                        controller = "Dashboard",
                        action = "Index"
                    }));
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary
                    (new
                    {
                        area = "",
                        controller = "Home",
                        action = "Index"
                    }));
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}