using BTL.Areas.Admin.ViewModel;
using BTL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BTL.Services
{
    public interface IProductService : IAbstractEntityService<Product>
    {
        Task<Product> GetProductBySkuAsync(string sku, bool onlyPublished = true);
        Task<IList<Product>> GetProductBySkusAsync(string[] skus, bool onlyPublished = true);
        Task<Product> GetProductByGtinAsync(string gtin, bool onlyPublished = true);
        Task<IList<Product>> GetProductsDisplayOnHomePageAsync(bool onlyPublished = true);
        Task<IList<ProductPicture>> GetProductPicturesByProductIdAsync(Product product);
        Task<IList<Picture>> GetPicturesByProductIdAsync(Product product, int recordToReturn = 0);
        Task<IList<CategoryProduct>> GetCategoryProductsByCategoryIdAsync(int categortId, bool onlyPublished = true);
        Task<int> GetNumOfProductInCategoryAsync(int categoryId, bool onlyPublished = false);
        Task SaveCategoriesToProductAsync(Product product, int[] categoryIds);
        Task DeleteProductAsync(int id);
        Task DeleteProductPictureAsync(int id);
    }
}
