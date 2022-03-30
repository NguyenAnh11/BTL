namespace BTL.Models
{
    public class CategoryManufacturer
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public int DisplayOrder { get; set; }
    }
}