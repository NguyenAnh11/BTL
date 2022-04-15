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
    [SessionAuthorize]
    public class CategoryController : Controller
    {
        private readonly ShopDbContext _db;
        private readonly ICommonService _commonService;
        private readonly IPictureService _pictureService;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICategoryService _categoryService;

        public CategoryController(
            ShopDbContext db,
            ICommonService commonService,
            IPictureService pictureService,
            IDateTimeService dateTimeService,
            ICategoryService categoryService)
        {
            _db = db;
            _commonService = commonService;
            _pictureService = pictureService;
            _dateTimeService = dateTimeService;
            _categoryService = categoryService;
        }
        [HttpGet]
        public ActionResult Index()
        {
            var model = new CategorySearchModel();

            //model.Page = 1;
            //model.PageSize = 5;

            ViewBag.CategoryStatuses = _commonService.PrepareStatusModel();

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(CategorySearchModel searchModel)
        {
            var categories = await _categoryService.GetCategoriesAsync((query) =>
            {
                if (searchModel.Id != 0)
                {
                    query = query.Where(c => c.Id == searchModel.Id);
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

                query = query
                    .OrderBy(p => p.ParentCategoryId)
                    .ThenBy(p => p.DisplayOrder);

                return query;
            });

            var models = new List<CategoryModel>();

            foreach (var category in categories)
            {
                var model = new CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    ShortDescription = category.ShortDescription,
                    Description = category.Description,
                    IsPublished = category.IsPublished,
                    IsIncludeOnTopMenu = category.IsIncludeOnTopMenu,
                    IsShowOnHomePage = category.IsShowOnHomePage,
                    DisplayOrder = category.DisplayOrder,
                    PictureId = category.PictureId ?? 0,
                    PictureUrl = await _pictureService.GetPictureUrlAsync(category.PictureId ?? 0),
                    Breadcrumb = await _categoryService.GetFormatedBreadcrumbAsync(category),
                    ParentCategoryId = category.ParentCategoryId ?? 0,
                    MetaTitle = category.MetaTitle,
                    MetaKeyword = category.MetaKeyword,
                    MetaDescription = category.MetaDescription,
                    CreateAt = _dateTimeService.Format(category.CreateAt),
                    UpdateAt = _dateTimeService.Format(category.UpdateAt),
                };

                models.Add(model);
            }

            return Json(new { data = models }, JsonRequestBehavior.AllowGet);
            //var pagedModel = new PagedList<CategoryModel>(models, searchModel.Page, searchModel.PageSize);
            //return Json(new { data = pagedModel }, JsonRequestBehavior.AllowGet);
        }
        public PartialViewResult Pagination(int page, int pageSize, int totalPages, bool hasPreviousPage, bool hasNextPage)
        {
            return PartialView("_Pagination", new { page, pageSize, totalPages, hasPreviousPage, hasNextPage });
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            ViewBag.AvaliableCategories = await _commonService.PrepareAvaliableCategoriesAsync();

            var model = new CategoryModel
            {
                IsIncludeOnTopMenu = true,
                IsPublished = true,
                DisplayOrder = 1,
                PictureId = 0,
                PictureUrl = await _pictureService.GetPictureUrlAsync(0),
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(CategoryModel model)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var category = new Category()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        ShortDescription = model.ShortDescription,
                        IsPublished = model.IsPublished,
                        IsShowOnHomePage = model.IsShowOnHomePage,
                        IsIncludeOnTopMenu = model.IsIncludeOnTopMenu,
                        DisplayOrder = model.DisplayOrder,
                        ParentCategoryId = model.ParentCategoryId,
                        PictureId = model.PictureId,
                        MetaTitle = model.MetaTitle,
                        MetaKeyword = model.MetaKeyword,
                        MetaDescription = model.MetaDescription,
                    };

                    if (category.ParentCategoryId == 0)
                        category.ParentCategoryId = null;

                    if (category.PictureId == 0)
                        category.PictureId = null;

                    _db.Set<Category>().Add(category);

                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    return Json(new { id = category.Id }, JsonRequestBehavior.AllowGet);
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
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
                return HttpNotFound();

            var model = new CategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ShortDescription = category.ShortDescription,
                DisplayOrder = category.DisplayOrder,
                IsPublished = category.IsPublished,
                IsShowOnHomePage = category.IsShowOnHomePage,
                IsIncludeOnTopMenu = category.IsIncludeOnTopMenu,
                PictureId = category.PictureId ?? 0,
                PictureUrl = await _pictureService.GetPictureUrlAsync(category.PictureId ?? 0),
                ParentCategoryId = category.ParentCategoryId ?? 0,
                MetaTitle = category.MetaTitle,
                MetaKeyword = category.MetaKeyword,
                MetaDescription = category.MetaDescription,
                CreateAt = _dateTimeService.Format(category.CreateAt),
                UpdateAt = _dateTimeService.Format(category.UpdateAt),
            };

            ViewBag.AvaliableCategories = await _commonService.PrepareAvaliableCategoriesAsync();

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(CategoryModel model)
        {
            var category = await _categoryService.GetByIdAsync(model.Id);

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    category.Name = model.Name;
                    category.Description = model.Description;
                    category.ShortDescription = model.ShortDescription;
                    category.DisplayOrder = model.DisplayOrder;
                    category.IsPublished = model.IsPublished;
                    category.IsIncludeOnTopMenu = model.IsIncludeOnTopMenu;
                    category.IsShowOnHomePage = model.IsShowOnHomePage;
                    category.MetaTitle = model.MetaTitle;
                    category.MetaKeyword = model.MetaKeyword;
                    category.MetaDescription = model.MetaDescription;

                    if (model.ParentCategoryId == 0)
                    {
                        category.ParentCategoryId = null;
                    }
                    else
                    {
                        category.ParentCategoryId = model.ParentCategoryId;
                    }

                    var previousPictureId = category.PictureId ?? 0;

                    if (previousPictureId != 0 && model.PictureId != category.PictureId)
                    {
                        var picture = await _pictureService.GetByIdAsync(previousPictureId);

                        await _pictureService.DeletePictureAsync(picture);
                    }

                    if (model.PictureId == 0)
                        category.PictureId = null;
                    else
                        category.PictureId = model.PictureId;

                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    return Json(new { id = category.Id }, JsonRequestBehavior.AllowGet);
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
            await _categoryService.DeleteCategoryAsync(id);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteSelected(int[] ids)
        {
            if (ids == null || !ids.Any())
                return Json(new { success = false, msg = "Chọn danh mục xóa" }, JsonRequestBehavior.AllowGet);

            if (ids.Contains(0))
                ids.ToList().Remove(0);

            foreach (var id in ids)
            {
                await _categoryService.DeleteCategoryAsync(id);
            }

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}