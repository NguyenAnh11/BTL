namespace BTL.ViewModels
{
    public class ProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public bool IsMarkNew { get; set; }
        public string PictureUrl { get; set; }
        public string ManufacturerName { get; set; }
    }
}