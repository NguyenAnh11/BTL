using System.Web.Mvc;
using BTL.Areas.Admin.ViewModel;

namespace BTL.Filters
{
    public class ValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var viewData = filterContext.Controller.ViewData;

            if (!viewData.ModelState.IsValid)
            {
                var response = Response.Fail("");
                var res = new JsonResult()
                {
                    Data = response,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet  
                };
                filterContext.Result = res;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}