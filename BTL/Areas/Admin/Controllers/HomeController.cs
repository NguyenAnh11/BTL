using BTL.Filters;
using System.Web.Mvc;

namespace BTL.Areas.Admin.Controllers
{
    [SessionAuthorize]
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}