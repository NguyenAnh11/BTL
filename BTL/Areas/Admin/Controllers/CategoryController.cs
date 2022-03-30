using BTL.Areas.Admin.ViewModel;
using BTL.Data;
using BTL.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BTL.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ShopDbContext _db = new ShopDbContext();
        [HttpGet]
        public ActionResult Index()
        {
            var model = new CategorySearchModel();

            model.CategoryPublishStatus = new SelectList(new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "0",
                    Text = "Chọn tình trạng",
                    Selected = true
                },
                new SelectListItem
                {
                    Value = "1",
                    Text = "Công khai"
                },
                new SelectListItem
                {
                    Value = "2",
                    Text = "Riêng tư"
                }
             });

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(CategorySearchModel model)
        {
            var query = _db.Categories.AsQueryable().Where(p => !p.IsDeleted);

            if(model.CategoryId > 0)
            {
                query = query.Where(p => p.Id == model.CategoryId);
            }

            if(model.CategoryPublishdStatusId == 1)
            {
                query = query.Where(p => p.IsPublished);
            }    
            else if(model.CategoryPublishdStatusId == 2)
            {
                query = query.Where(p => !p.IsPublished);
            }    

            if(!string.IsNullOrEmpty(model.CategoryName))
            {
                query = query.Where(p => p.Name.Contains(model.CategoryName));
            }

            var unSortedCategories = await query.OrderBy(p => p.ParentCategoryId).ThenBy(p => p.DisplayOrder).ToListAsync();

            var sortedCategories = SortTreeCategories(unSortedCategories);

            var categories = sortedCategories.Select(p => new CategoryModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ShortDescription = p.ShortDescription,
                IsPublished = p.IsPublished,
                IsIncludeOnTopMenu = p.IsIncludeOnTopMenu,
                IsShowOnHomePage = p.IsShowOnHomePage,
                DisplayOrder = p.DisplayOrder,
                MetaTitle = p.MetaTitle,
                MetaKeyword = p.MetaKeyword,
                MetaDescription = p.MetaDescription,
                CreateAt = p.CreateAt,
                UpdateAt = p.UpdateAt,
                PictureUrl = p.PictureUrl,
                ParentCategoryId = p.ParentCategoryId,
                Breadcrumb = GetBreadcrumbCategory(p, sortedCategories)
            });
            
            return Json(categories, JsonRequestBehavior.AllowGet);
        }
        private IList<Category> SortTreeCategories(IList<Category> source, int? parentCategoryId = null)
        {
            var items = new List<Category>();
            
            var list = source.Where(p => p.ParentCategoryId == parentCategoryId).OrderBy(p => p.DisplayOrder).ThenBy(p => p.Name).ToList();

            foreach(var item in list)
            {
                items.Add(item);

                items.AddRange(SortTreeCategories(source, item.Id));
            }

            return items;
        }
        private string GetBreadcrumbCategory(Category category, IList<Category> source)
        {
            var categories = new List<Category>();

            while(
                category != null &&
                !category.IsDeleted &&
                category.IsPublished
                )
            {
                categories.Add(category);

                var parentCategoryId = category.ParentCategoryId ?? 0;

                if (parentCategoryId == 0) break;

                category = source.FirstOrDefault(p => p.Id == parentCategoryId); 
            }

            categories.Reverse();

            var breadcrumb = string.Empty;

            foreach (var catagory in categories)
            {
                breadcrumb = string.IsNullOrEmpty(breadcrumb) ? catagory.Name : $"{breadcrumb} -> {catagory.Name}";
            }

            return breadcrumb;
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var model = new CategoryModel();

            var unsortedCategories = await _db.Categories.Where(p => !p.IsDeleted && p.IsPublished)
                .OrderBy(p => p.ParentCategoryId)
                .ThenBy(p => p.DisplayOrder)
                .ToListAsync();

            var sortedCategories = SortTreeCategories(unsortedCategories);

            var categoriesSelectListItem = sortedCategories
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = GetBreadcrumbCategory(p, sortedCategories)
                })
                .ToList();

            categoriesSelectListItem.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "Không có"
            });

            model.AvaliableCategories = new SelectList(categoriesSelectListItem);

            return View(model);
        }
    }
}