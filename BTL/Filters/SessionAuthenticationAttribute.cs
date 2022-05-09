using System.Web;
using System.Web.Mvc;

namespace BTL.Filters
{
    public class SessionAuthenticationAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if(httpContext.Session["Id"] == null)
            {
                return false;
            }

            return true;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Account/Login");
        }
    }
}