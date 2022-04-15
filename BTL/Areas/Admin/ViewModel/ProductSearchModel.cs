namespace BTL.Areas.Admin.ViewModel
{
    public class ProductSearchModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Gtin { get; set; }
        public int Status { get; set; }
        public bool IsShowOnHomePage { get; set; }
        public int CategoryId { get; set; }
        public int ManufacturerId { get; set; }
    }
}