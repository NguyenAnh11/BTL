using BTL.Data;
using BTL.Extensions;
using BTL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BTL.Services
{
    public class ProductService : AbstractSoftDeleteEntityService<Product>, IProductService
    {
        private readonly IPictureService _pictureService;
        public ProductService(ShopDbContext db, IPictureService pictureService) : base(db)
        {
            _pictureService = pictureService;
        }

        public async Task<Product> GetProductByGtinAsync(string gtin, bool onlyPublished = true)
        {
            if (gtin.IsEmpty())
                throw new ArgumentNullException(nameof(gtin));

            var query = Table.Where(p => !p.IsDeleted);

            if (onlyPublished)
                query = query.Where(p => !p.IsPublished);

            var product = await query.FirstOrDefaultAsync(p => p.Gtin == gtin);

            return product;
        }

        public async Task<Product> GetProductBySkuAsync(string sku, bool onlyPublished = true)
        {
            if (sku.IsEmpty())
                throw new ArgumentNullException(nameof(sku));

            var query = Table.Where(p => !p.IsDeleted);

            if (onlyPublished)
                query = query.Where(p => p.IsPublished);

            var product = await query.FirstOrDefaultAsync(p => p.Sku == sku);

            return product;
        }

        public async Task<IList<Product>> GetProductBySkusAsync(string[] skus, bool onlyPublished = true)
        {
            if (skus == null)
                throw new ArgumentNullException(nameof(skus));

            if (!skus.Any()) return null;

            var products = await GetAllAsync((query) =>
            {
                if (onlyPublished)
                    query = query.Where(p => p.IsPublished);

                return query.Where(p => skus.Contains(p.Sku));
            });

            return products;
        }

        public async Task<IList<Product>> GetProductsDisplayOnHomePageAsync(bool onlyPublished = true)
        {
            var products = await GetAllAsync((query) =>
            {
                if (onlyPublished)
                    query = query.Where(p => p.IsPublished);

                return query.Where(p => p.IsShowOnHomePage);
            });
            return products;
        }

        public async Task<IList<Picture>> GetPicturesByProductIdAsync(Product product, int? recordToReturn = default)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var query = _db.Set<ProductPicture>()
                 .Where(p => p.ProductId == product.Id)
                 .OrderBy(p => p.DisplayOrder)
                 .Join(
                    _pictureService.Table,
                    pp => pp.PictureId,
                    p => p.Id,
                    (pp, p) => p
                  );

            if(recordToReturn != null && recordToReturn.Value > 0)
                query = query.Take(recordToReturn.Value);

            var pictures = await query.ToListAsync();

            return pictures;
        }

        public async Task<IList<ProductPicture>> GetProductPicturesByProductIdAsync(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var query = await _db.Set<ProductPicture>()
                .Where(p => p.ProductId == product.Id)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            return query;
        }

        public async Task<IList<CategoryProduct>> GetCategoryProductsByCategoryIdAsync(int categortId, bool onlyPublished = true)
        {
            if (categortId == 0)
                throw new ArgumentNullException(nameof(categortId));

            var source = !onlyPublished ? Table : Table.Where(p => p.IsPublished);

            if (onlyPublished)
                source = source.Where(p => p.IsPublished);

            var model = await _db.Set<CategoryProduct>().Where(p => p.CategoryId == categortId)
                .Join(source, cp => cp.ProductId, p => p.Id, (cp, p) => cp)
                .ToListAsync();

            return model;
        }

        public async Task<int> GetNumOfProductInCategoryAsync(int categoryId, bool onlyPublished = false)
        {
            return (await GetCategoryProductsByCategoryIdAsync(categoryId, onlyPublished)).Count();
        }

        public async Task SaveCategoriesToProductAsync(Product product, int[] categoryIds)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (categoryIds == null)
                throw new ArgumentNullException(nameof(categoryIds));

            var existCategoryProducts = await _db.Set<CategoryProduct>().Where(p => p.ProductId == product.Id).ToListAsync();

            if (existCategoryProducts.Any())
            {
                foreach (var categoryProduct in existCategoryProducts)
                {
                    if (!categoryIds.Contains(categoryProduct.CategoryId))
                    {
                        _db.Set<CategoryProduct>().Remove(categoryProduct);
                    }
                }
            }

            foreach (var categoryId in categoryIds)
            {
                if (!existCategoryProducts.Any(p => p.CategoryId == categoryId))
                {
                    var model = new CategoryProduct
                    {
                        ProductId = product.Id,
                        CategoryId = categoryId,
                        DisplayOrder = 1
                    };

                    _db.Set<CategoryProduct>().Add(model);
                }
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(id));

            var product = await GetByIdAsync(id);

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            _db.Set<Product>().Remove(product);

            await _db.SaveChangesAsync();
        }

        public async Task DeleteProductPictureAsync(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(id));

            var productPicture = await _db.Set<ProductPicture>().FirstOrDefaultAsync(p => p.Id == id);

            if (productPicture == null)
                throw new ArgumentNullException(nameof(productPicture));

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Set<ProductPicture>().Remove(productPicture);

                    var picture = await _pictureService.GetByIdAsync(productPicture.PictureId);

                    await _pictureService.DeletePictureAsync(picture);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw ex;
                }
            }
        }
    }
}