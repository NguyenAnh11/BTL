using System.Web;
using System.Web.Mvc;

namespace BTL.Filters
{
    public class AdminAuthrorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext.Session["IsAdmin"] == null)
                return false;

            if(bool.TryParse(httpContext.Session["IsAdmin"].ToString(), out bool isAdmin))
            {
                if (!isAdmin)
                    return false;

                return true;
            }

            return true;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home");
        }
    }
}