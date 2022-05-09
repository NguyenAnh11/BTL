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
    public class ProductController : Controller
    {
        private readonly ShopDbContext _db;
        private readonly ICommonService _commonService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(
            ShopDbContext db,
            ICommonService commonService,
            IDateTimeService dateTimeService,
            IPictureService pictureService,
            IProductService productService,
            ICategoryService categoryService)
        {
            _db = db;
            _commonService = commonService;
            _dateTimeService = dateTimeService;
            _pictureService = pictureService;
            _productService = productService;
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewBag.AvaliableCategories = await _commonService.PrepareAvaliableCategoriesAsync();

            ViewBag.AvaliableManufacturers = await _commonService.PrepareAvaliableManufacturerAsync();

            ViewBag.ProductStatuses = _commonService.PrepareStatusModel();

            var model = new ProductSearchModel()
            {
                IsShowOnHomePage = true
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(ProductSearchModel searchModel)
        {
            var products = await _productService.GetAllAsync((query) =>
            {
                if (searchModel.Id != 0)
                {
                    query = query.Where(p => p.Id == searchModel.Id);
                }

                if (!searchModel.Name.IsEmpty())
                {
                    query = query.Where(p => p.Name.Contains(searchModel.Name));
                }

                if (!searchModel.Gtin.IsEmpty())
                {
                    query = query.Where(p => p.Gtin == searchModel.Gtin);
                }

                if (!searchModel.Sku.IsEmpty())
                {
                    query = query.Where(p => p.Sku == searchModel.Sku);
                }

                if (searchModel.Status == 1)
                {
                    query = query.Where(p => p.IsPublished);
                }
                else if (searchModel.Status == 2)
                {
                    query = query.Where(p => !p.IsPublished);
                }

                if (searchModel.CategoryId != 0)
                {
                    var source = _db.Set<CategoryProduct>().Where(p => p.CategoryId == searchModel.CategoryId);

                    query = query.Join(source, p => p.Id, cp => cp.ProductId, (p, cp) => p);
                }

                if (searchModel.ManufacturerId != 0)
                {
                    query = query.Where(p => p.ManufacturerId == searchModel.ManufacturerId);
                }


                query = query.Where(p => p.IsShowOnHomePage == searchModel.IsShowOnHomePage);

                return query;
            });

            var models = new List<ProductModel>();

            foreach (var product in products)
            {
                var model = new ProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    ShortDescription = product.ShortDescription,
                    Sku = product.Sku,
                    Gtin = product.Gtin,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    DisplayOrder = product.DisplayOrder,
                    IsPublished = product.IsPublished,
                    IsMarkAsNew = product.IsMarkAsNew,
                    IsShowOnHomePage = product.IsShowOnHomePage,
                    MetaTitle = product.MetaTitle,
                    MetaKeyword = product.MetaKeyword,
                    MetaDescription = product.MetaDescription,
                    CreateAt = _dateTimeService.Format(product.CreateAt),
                    UpdateAt = _dateTimeService.Format(product.UpdateAt),
                };

                var pictures = await _productService.GetPicturesByProductIdAsync(product, 1);

                int pictureId = 0;

                if (pictures != null && pictures.Count > 0)
                {
                    var picture = pictures[0];
                    pictureId = picture.Id;
                }

                model.PictureUrl = await _pictureService.GetPictureUrlAsync(pictureId); //get default image

                models.Add(model);
            }

            return Json(new { data = models }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> GoToSku(string sku)
        {
            var product = await _productService.GetProductBySkuAsync(sku);

            if (product == null)
            {
                return Json(new { data = product }, JsonRequestBehavior.AllowGet);
            }

            var model = new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                Sku = product.Sku,
                Gtin = product.Gtin,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                DisplayOrder = product.DisplayOrder,
                IsPublished = product.IsPublished,
                IsMarkAsNew = product.IsMarkAsNew,
                IsShowOnHomePage = product.IsShowOnHomePage,
                MetaTitle = product.MetaTitle,
                MetaKeyword = product.MetaKeyword,
                MetaDescription = product.MetaDescription,
                CreateAt = _dateTimeService.Format(product.CreateAt),
                UpdateAt = _dateTimeService.Format(product.UpdateAt),
            };

            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            ViewBag.AvaliableCategories = await _commonService.PrepareAvaliableCategoriesAsync(true);

            ViewBag.AvaliableManufacturers = await _commonService.PrepareAvaliableManufacturerAsync(true);

            var model = new ProductModel
            {
                IsPublished = true,
                IsShowOnHomePage = true,
                DisplayOrder = 1,
                IsMarkAsNew = false,
                Price = 1000000,
                StockQuantity = 100
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(ProductModel model)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var product = new Product
                    {
                        Name = model.Name,
                        Description = model.Description,
                        ShortDescription = model.ShortDescription,
                        Sku = model.Sku,
                        Gtin = model.Gtin,
                        Price = model.Price,
                        DisplayOrder = model.DisplayOrder,
                        StockQuantity = model.StockQuantity,
                        IsPublished = model.IsPublished,
                        IsShowOnHomePage = model.IsShowOnHomePage,
                        IsMarkAsNew = model.IsMarkAsNew,
                        MetaTitle = model.MetaTitle,
                        MetaKeyword = model.MetaKeyword,
                        MetaDescription = model.MetaDescription,
                    };

                    if (model.ManufacturerId == 0)
                        product.ManufacturerId = null;

                    _db.Set<Product>().Add(product);

                    await _db.SaveChangesAsync();

                    if(model.CategoryId != null)
                        await _productService.SaveCategoriesToProductAsync(product, model.CategoryId);

                    transaction.Commit();


                    return Json(new { id = product.Id }, JsonRequestBehavior.AllowGet);
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
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return HttpNotFound();

            var model = new ProductModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                Sku = product.Sku,
                Gtin = product.Gtin,
                Price = product.Price,
                DisplayOrder = product.DisplayOrder,
                StockQuantity = product.StockQuantity,
                IsPublished = product.IsPublished,
                IsMarkAsNew = product.IsMarkAsNew,
                IsShowOnHomePage = product.IsShowOnHomePage,
                MetaTitle = product.MetaTitle,
                MetaKeyword = product.MetaKeyword,
                MetaDescription = product.MetaDescription,
                CreateAt = _dateTimeService.Format(product.CreateAt),
                UpdateAt = _dateTimeService.Format(product.UpdateAt),
                ManufacturerId = product.ManufacturerId ?? 0,
                PictureUrl = await _pictureService.GetPictureUrlAsync(0)
            };

            var categoryProducts = await _categoryService.GetCategoryProductByProductIdAsync(product.Id);

            if (categoryProducts != null && categoryProducts.Any())
                model.CategoryId = categoryProducts.Select(p => p.CategoryId).ToArray();

            var avaliableCategories = await _commonService.PrepareAvaliableCategoriesAsync(false);

            if(model.CategoryId != null)
            {
                foreach (var category in avaliableCategories)
                {
                    category.Selected = int.TryParse(category.Value, out var categoryId) && model.CategoryId.Contains(categoryId);
                }
            }

            ViewBag.AvaliableCategories = avaliableCategories;

            ViewBag.AvaliableManufacturers = await _commonService.PrepareAvaliableManufacturerAsync(false);

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(ProductModel model)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var product = await _productService.GetByIdAsync(model.Id);

                    if (product == null)
                        throw new ArgumentNullException(nameof(product));

                    product.Name = model.Name;
                    product.Description = model.Description;
                    product.ShortDescription = model.ShortDescription;
                    product.Sku = model.Sku;
                    product.Gtin = model.Gtin;
                    product.Price = model.Price;
                    product.StockQuantity = model.StockQuantity;
                    product.DisplayOrder = model.DisplayOrder;
                    product.IsPublished = model.IsPublished;
                    product.IsPublished = product.IsPublished;
                    product.IsShowOnHomePage = model.IsShowOnHomePage;
                    product.IsMarkAsNew = model.IsMarkAsNew;
                    product.MetaTitle = model.MetaTitle;
                    product.MetaKeyword = model.MetaKeyword;
                    product.MetaDescription = model.MetaDescription;

                    if (model.ManufacturerId == 0)
                        product.ManufacturerId = null;

                    if (model.CategoryId != null)
                        await _productService.SaveCategoriesToProductAsync(product, model.CategoryId);

                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    return Json(new { id = product.Id }, JsonRequestBehavior.AllowGet);
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
            await _productService.DeleteProductAsync(id);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteSelected(int[] ids)
        {
            if (ids == null || !ids.Any())
                return Json(new { success = false, msg = "Chọn sản phẩm xóa" }, JsonRequestBehavior.AllowGet);

            if (ids.Contains(0))
                ids.ToList().Remove(0);

            await Task.WhenAll(ids.Select(p => _productService.DeleteProductAsync(p)));

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        [Route("{productId}")]
        [HttpPost]
        public async Task<ActionResult> ProductPictureList(int productId)
        {
            var product = await _productService.GetByIdAsync(productId);

            if (product == null)
                return HttpNotFound();

            var productPictures = await _productService.GetProductPicturesByProductIdAsync(product);

            var models = new List<ProductPictureModel>();

            foreach (var productPicture in productPictures)
            {
                var picture = await _pictureService.GetByIdAsync(productPicture.PictureId);

                models.Add(new ProductPictureModel
                {
                    Id = productPicture.Id,
                    ProductId = product.Id,
                    PictureId = picture.Id,
                    PictureUrl = _pictureService.GetPictureUrl(picture),
                    Alt = picture.Alt ?? string.Empty,
                    Title = picture.Title ?? string.Empty,
                    DisplayOrder = productPicture.DisplayOrder
                }); ;
            }

            return Json(new { data = models }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> ProducctPictureAdd(ProductPictureModel model)
        {
            var product = await _productService.GetByIdAsync(model.ProductId);

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var productPicture = new ProductPicture
                    {
                        ProductId = model.ProductId,
                        PictureId = model.PictureId,
                        DisplayOrder = model.DisplayOrder,
                    };

                    var picture = await _pictureService.GetByIdAsync(model.PictureId);

                    picture.Alt = model.Alt;
                    picture.Title = model.Title;

                    _db.Set<ProductPicture>().Add(productPicture);

                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
        public async Task<ActionResult> ProductPictureDelete(int id)
        {
            await _productService.DeleteProductPictureAsync(id);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}