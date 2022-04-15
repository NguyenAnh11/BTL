using BTL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace BTL.Services
{
    public interface ICategoryService : IAbstractEntityService<Category>
    {
        IList<Category> SortTreeCategories(IList<Category> source, int? parentCategoryId = null);
        Task<IList<Category>> GetCategoriesAsync(Func<IQueryable<Category>, IQueryable<Category>> func = null);
        Task<IList<Category>> GetCategoriesDisplayOnHomePageAsync(bool onlyPublished = true);
        Task<IList<Category>> GetCategoriesDisplayInTopMenuAsync(bool onlyPublished = true);
        Task<IList<Category>> GetCategoryByParentCategoryIdAsync(int? parentCategoryId = null, bool onlyPublished = true);
        Task<IList<int>> GetChildCategoriesIdAsync(int? parentCategoryId = null, bool onlyPublished = true);
        Task<IList<Category>> GetCategoryBreadcrumbAsync(Category category, IList<Category> sources = null, bool onlyPublished = true);
        Task<string> GetFormatedBreadcrumbAsync(Category category, IList<Category> categories = null, string separator = "->");
        Task<IList<CategoryProduct>> GetCategoryProductByProductIdAsync(int productId, bool onlyPublished = false);
        Task DeleteCategoryAsync(int id);
    }
}
