using BTL.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BTL.Areas.Admin.Controllers
{
    public class ImageController : Controller
    {
        private readonly IPictureService _pictureService;
        public ImageController(IPictureService pictureService)
        {
            _pictureService = pictureService;
        }
        [HttpPost]
        public async Task<ActionResult> Upload()
        {
            var files = Request.Files;

            var file = files[0];

            var (msg, picture) = await _pictureService.SavePictureAsync(file);

            if (picture == null)
            {
                return Json(new
                {
                    success = false,
                    msg
                },
                JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                success = true,
                id = picture.Id,
                pictureUrl = _pictureService.GetPictureUrl(picture)
            },
            JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> Remove(int pictureId)
        {
            var picture = await _pictureService.GetByIdAsync(pictureId);

            if (picture == null)
            {
                return HttpNotFound();
            }

            await _pictureService.DeletePictureAsync(picture);

            return Json(new
            {
                success = true,
                pictureUrl = _pictureService.GetDefaultPictureUrl()
            }, JsonRequestBehavior.AllowGet);
        }
    }
}