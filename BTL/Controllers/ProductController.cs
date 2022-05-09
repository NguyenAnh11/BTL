using BTL.Services;
using BTL.ViewModels;
using System;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace BTL.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IManufacturerService _manufacturerService;
        public ProductController(IProductService productService, IManufacturerService manufacturerService)
        {
            _productService = productService;
            _manufacturerService = manufacturerService; 
        }
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> Index(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if(product == null) 
                throw new ArgumentNullException(nameof(product));

            var model = new ProductModel
            {
                Id = id,
                Name = product.Name,
                ShortDescription = product.ShortDescription,
                Description = product.Description,
                Sku = product.Sku,
                Gtin = product.Gtin,
                Price = product.Price,
                IsMarkNew = product.IsMarkAsNew,
                StockQuantity = product.StockQuantity,
                ManufacturerName = string.Empty
            };

            var manufacturer = await _manufacturerService.GetByIdAsync(product.ManufacturerId ?? 0);

            if(manufacturer != null)
                model.ManufacturerName = manufacturer.Name;

            var pictures = await _productService.GetPicturesByProductIdAsync(product);

            model.PictureUrl = pictures[0].FileName;

            foreach(var picture in pictures)
            {
                model.Pictures.Add(new ProductPicture
                {
                    Src = picture.FileName,
                    Alt = picture.Alt,
                    Title = picture.Title,
                });
            }

            return View(model);
        }
    }
}