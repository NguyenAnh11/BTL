using BTL.Areas.Admin.ViewModel;
using BTL.Data;
using BTL.Extensions;
using BTL.Filters;
using BTL.Models;
using BTL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BTL.Areas.Admin.Controllers
{
    [SessionAuthentication]
    [AdminAuthrorize]
    public class ManufacturerController : Controller
    {
        private readonly ShopDbContext _db;
        private readonly ICommonService _commonService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IPictureService _pictureService;
        private readonly IManufacturerService _manufacturerService;

        public ManufacturerController(
            ShopDbContext db,
            ICommonService commonService,
            IDateTimeService dateTimeService,
            IPictureService pictureService,
            IManufacturerService manufacturerService)
        {
            _db = db;
            _commonService = commonService;
            _dateTimeService = dateTimeService;
            _pictureService = pictureService;
            _manufacturerService = manufacturerService;
        }
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Status = _commonService.PrepareStatusModel();

            var model = new ManufacturerSearchModel
            {
                Page = 1
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(ManufacturerSearchModel searchModel)
        {
            var manufacturers = await _manufacturerService.GetAllAsync((query) =>
            {
                if (searchModel.Id != 0)
                {
                    query = query.Where(p => p.Id == searchModel.Id);
                }

                if (!searchModel.Name.IsEmpty())
                {
                    query = query.Where(p => p.Name.Contains(searchModel.Name));
                }

                if (searchModel.Status == 1)
                {
                    query = query.Where(p => p.IsPublished);
                }
                else if (searchModel.Status == 2)
                {
                    query = query.Where(p => !p.IsPublished);
                }

                return query;
            });

            ViewBag.TotalPages = manufacturers.Count;

            var models = new List<ManufacturerModel>();

            foreach (var manufacturer in manufacturers)
            {
                var model = new ManufacturerModel
                {
                    Id = manufacturer.Id,
                    Name = manufacturer.Name,
                    Description = manufacturer.Description,
                    IsPublished = manufacturer.IsPublished,
                    DisplayOrder = manufacturer.DisplayOrder,
                    MetaTitle = manufacturer.MetaTitle,
                    MetaKeyword = manufacturer.MetaKeyword,
                    MetaDescription = manufacturer.MetaDescription,
                    CreateAt = _dateTimeService.Format(manufacturer.CreateAt),
                    UpdateAt = _dateTimeService.Format(manufacturer.UpdateAt),
                    PictureId = manufacturer.PictureId ?? 0,
                    PictureUrl = await _pictureService.GetPictureUrlAsync(manufacturer.PictureId ?? 0)
                };

                models.Add(model);
            }

            return Json(new { data = models }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new ManufacturerModel
            {
                IsPublished = true,
                DisplayOrder = 1,
                PictureId = 0,
                PictureUrl = await _pictureService.GetPictureUrlAsync(0),
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(ManufacturerModel model)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var manufacturer = new Manufacturer
                    {
                        Name = model.Name,
                        Description = model.Description,
                        IsPublished = model.IsPublished,
                        DisplayOrder = model.DisplayOrder,
                        MetaTitle = model.MetaTitle,
                        MetaKeyword = model.MetaKeyword,
                        MetaDescription = model.MetaDescription,
                    };

                    if (model.PictureId == 0)
                        manufacturer.PictureId = null;
                    else
                        manufacturer.PictureId = model.PictureId;

                    _db.Set<Manufacturer>().Add(manufacturer);

                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    return Json(new { id = manufacturer.Id }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw ex;
                }
            }
        }
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var manufacturer = await _manufacturerService.GetByIdAsync(id);

            if (manufacturer == null)
                return HttpNotFound();

            var model = new ManufacturerModel
            {
                Id = manufacturer.Id,
                Name = manufacturer.Name,
                Description = manufacturer.Description,
                DisplayOrder = manufacturer.DisplayOrder,
                IsPublished = manufacturer.IsPublished,
                PictureId = manufacturer.PictureId ?? 0,
                PictureUrl = await _pictureService.GetPictureUrlAsync(manufacturer.PictureId ?? 0),
                MetaTitle = manufacturer.MetaTitle,
                MetaKeyword = manufacturer.MetaKeyword,
                MetaDescription = manufacturer.MetaDescription,
                CreateAt = _dateTimeService.Format(manufacturer.CreateAt),
                UpdateAt = _dateTimeService.Format(manufacturer.UpdateAt),
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(ManufacturerModel model)
        {
            var manufacturer = await _manufacturerService.GetByIdAsync(model.Id);

            if (manufacturer == null)
                throw new ArgumentNullException(nameof(manufacturer));

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    manufacturer.Name = model.Name;
                    manufacturer.Description = model.Description;
                    manufacturer.DisplayOrder = model.DisplayOrder;
                    manufacturer.IsPublished = model.IsPublished;
                    manufacturer.MetaTitle = model.MetaTitle;
                    manufacturer.MetaKeyword = model.MetaKeyword;
                    manufacturer.MetaDescription = model.MetaDescription;

                    var previousPictureId = manufacturer.PictureId ?? 0;

                    if (previousPictureId != 0 && previousPictureId != model.PictureId)
                    {
                        var picture = await _pictureService.GetByIdAsync(previousPictureId);

                        await _pictureService.DeletePictureAsync(picture);
                    }

                    if (model.PictureId == 0)
                        manufacturer.PictureId = null;
                    else
                        manufacturer.PictureId = model.PictureId;

                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    return Json(new { id = manufacturer.Id }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
        [Route("{id}")]
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            await _manufacturerService.DeleteManufacturerAsync(id);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteSelected(int[] ids)
        {
            if (ids == null || !ids.Any())
                return Json(new { success = false, msg = "Chọn hãng sản xuất cần xóa" }, JsonRequestBehavior.AllowGet);

            if (ids.Contains(0))
                ids.ToList().Remove(0);

            foreach (var id in ids)
            {
                await _manufacturerService.DeleteManufacturerAsync(id);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}