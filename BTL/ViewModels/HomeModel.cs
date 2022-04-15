using System.Collections.Generic;

namespace BTL.ViewModels
{
    public class HomeModel
    {
        public IList<ProductModel> Products { get; set; } = new List<ProductModel>();
        public IList<CategoryModel> Categories { get; set; } = new List<CategoryModel>();
    }
}