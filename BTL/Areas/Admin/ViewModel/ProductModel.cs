namespace BTL.Areas.Admin.ViewModel
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Sku { get; set; }
        public string Gtin { get; set; }
        public decimal Price { get; set; }
        public int DisplayOrder { get; set; }
        public int StockQuantity { get; set; }
        public bool IsPublished { get; set; }
        public bool IsShowOnHomePage { get; set; }
        public bool IsMarkAsNew { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public string PictureUrl { get; set; }
        public string CreateAt { get; set; }
        public string UpdateAt { get; set; }
        public int ManufacturerId { get; set; }
        public int[] CategoryId { get; set; }
        public ProductPictureModel AddProductPictureModel { get; set; } = new ProductPictureModel();
    }
}