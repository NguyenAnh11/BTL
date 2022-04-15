using BTL.Data;
using BTL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BTL.Services
{
    public class CategoryService : AbstractSoftDeleteEntityService<Category>, ICategoryService
    {
        public CategoryService(ShopDbContext db) : base(db)
        {

        }

        public IList<Category> SortTreeCategories(IList<Category> source, int? parentCategoryId = null)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var categories = new List<Category>();

            var childCategories = source.Where(p => p.ParentCategoryId == parentCategoryId)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.Name)
                .ToList();

            foreach (var category in childCategories)
            {
                categories.Add(category);

                categories.AddRange(SortTreeCategories(source, category.Id));
            }

            return categories;
        }

        public async Task<IList<Category>> GetCategoriesAsync(Func<IQueryable<Category>, IQueryable<Category>> func = null)
        {
            var unsortedCategories = await GetAllAsync((query) =>
            {
                if (func != null)
                    query = func(query);

                return query;
            });

            var sortedCategories = SortTreeCategories(unsortedCategories);

            return sortedCategories;
        }

        public async Task<IList<Category>> GetCategoriesDisplayOnHomePageAsync(bool onlyPublished = true)
        {
            var categories = await GetAllAsync((query) =>
            {
                if (onlyPublished)
                    query = query.Where(p => p.IsPublished);

                return query
                    .Where(p => p.IsShowOnHomePage)
                    .OrderBy(p => p.ParentCategoryId)
                    .ThenBy(p => p.DisplayOrder);
            });

            return categories;
        }

        public async Task<IList<Category>> GetCategoriesDisplayInTopMenuAsync(bool onlyPublished = true)
        {
            var categories = await GetAllAsync((query) =>
            {
                if (onlyPublished)
                    query = query.Where(p => p.IsPublished);

                return query.Where(p => p.IsIncludeOnTopMenu);
            });

            return categories;
        }

        public async Task<IList<Category>> GetCategoryByParentCategoryIdAsync(int? parentCategoryId = null, bool onlyPublished = true)
        {
            var categories = await GetAllAsync((query) =>
            {
                if (onlyPublished)
                    query = query.Where(p => p.IsPublished);

                query = query.Where(p => p.ParentCategoryId == parentCategoryId).OrderBy(p => p.DisplayOrder).ThenBy(p => p.Name);

                return query;
            });

            return categories;
        }

        public async Task<IList<int>> GetChildCategoriesIdAsync(int? parentCategoryId = null, bool onlyPublished = true)
        {
            var categories = await GetAllAsync((query) =>
            {
                if (onlyPublished)
                    query = query.Where(p => p.IsPublished);

                query = query.Where(p => p.ParentCategoryId == parentCategoryId);

                return query;
            });

            var ids = new List<int>();

            foreach (var category in categories)
            {
                ids.Add(category.Id);

                var childIds = await GetChildCategoriesIdAsync(category.Id, onlyPublished);

                if (ids.Count > 0)
                    ids.AddRange(childIds);
            }

            return ids;
        }

        public async Task<IList<Category>> GetCategoryBreadcrumbAsync(Category category, IList<Category> sources = null, bool onlyPublished = true)
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            var categories = new List<Category>();

            while (
                category != null &&
                !category.IsDeleted
            )
            {
                if (onlyPublished && !category.IsPublished)
                    break;

                categories.Add(category);

                var parentCategoryId = category.ParentCategoryId ?? 0;

                if (parentCategoryId == 0) break;

                category = sources == null ?
                    await GetByIdAsync(parentCategoryId) :
                    sources.FirstOrDefault(p => p.Id == parentCategoryId);
            }

            categories.Reverse();

            return categories;
        }

        public async Task<string> GetFormatedBreadcrumbAsync(Category category, IList<Category> categories = null, string separator = "->")
        {
            if (category == null) throw new ArgumentNullException(nameof(category));

            var breadcrumb = await GetCategoryBreadcrumbAsync(category, categories);

            var format = string.Empty;

            foreach (var item in breadcrumb)
            {
                format += string.IsNullOrEmpty(format) ? item.Name : separator + item.Name;
            }

            return format;
        }

        public async Task<IList<CategoryProduct>> GetCategoryProductByProductIdAsync(int productId, bool onlyPublished = false)
        {
            if (productId == 0)
                throw new ArgumentNullException(nameof(productId));

            var source = !onlyPublished ? Table : Table.Where(p => p.IsPublished);

            if (onlyPublished)
                source = source.Where(p => p.IsPublished);

            var model = await _db.Set<CategoryProduct>().Where(p => p.ProductId == productId)
                .Join(source, cp => cp.CategoryId, c => c.Id, (cp, c) => cp)
                .ToListAsync();

            return model;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(id));

            var category = await GetByIdAsync(id);

            if (category == null)
                throw new ArgumentNullException(nameof(category));

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Set<Category>().Remove(category);

                    var childCategories = await GetCategoryByParentCategoryIdAsync(category.Id, false);

                    foreach (var childCategory in childCategories)
                    {
                        childCategory.ParentCategoryId = null;
                    }

                    await _db.SaveChangesAsync();

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