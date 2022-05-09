using BTL.Services;
using BTL.ViewModels;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BTL.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
        private readonly ICategoryService _categoryService;
        public HomeController(IProductService productService, IPictureService pictureService, ICategoryService categoryService)
        {
            _productService = productService;
            _pictureService = pictureService;
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var products = await _productService.GetAllAsync((query) => query);

            var homeModel = new HomeModel();

            foreach (var product in products)
            {
                var productModel = new ProductModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    StockQuantity = product.StockQuantity,
                    Price = product.Price,
                    IsMarkNew = product.IsMarkAsNew,
                };

                var pictures = await _productService.GetPicturesByProductIdAsync(product, 1);

                int pictureId = 0;
                if (pictures != null && pictures.Count == 1)
                    pictureId = pictures[0].Id;

                productModel.PictureUrl = await _pictureService.GetPictureUrlAsync(pictureId);

                homeModel.Products.Add(productModel);
            }

            var categories = await _categoryService.GetCategoriesDisplayOnHomePageAsync();

            foreach (var category in categories)
            {
                var categoryModel = new CategoryModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    MetaTitle = category.MetaTitle,
                    MetaKeyWord = category.MetaKeyword,
                    MetaDescription = category.MetaDescription
                };

                int pictureId = category.PictureId ?? 0;

                if (pictureId == 0)
                {
                    categoryModel.Picture = new PictureModel
                    {
                        Url = await _pictureService.GetPictureUrlAsync(pictureId),
                        Alt = string.Empty,
                        Title = string.Empty
                    };
                }
                else
                {
                    var picture = await _pictureService.GetByIdAsync(pictureId);

                    categoryModel.Picture = new PictureModel
                    {
                        Url = _pictureService.GetPictureUrl(picture),
                        Alt = picture.Alt,
                        Title = picture.Title
                    };
                }

                homeModel.Categories.Add(categoryModel);
            }

            return View(homeModel);
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}