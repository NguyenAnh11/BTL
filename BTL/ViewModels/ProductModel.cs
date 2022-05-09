using System.Collections.Generic;

namespace BTL.ViewModels
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Gtin { get; set; }
        public string Sku { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public bool IsMarkNew { get; set; }
        public string PictureUrl { get; set; }
        public string ManufacturerName { get; set; }
        public IList<ProductPicture> Pictures { get; set; } = new List<ProductPicture>();
    }

    public class ProductPicture
    {
        public string Src { get; set; }
        public string Alt { get; set; }
        public string Title { get; set; }
    }
}