namespace BTL.Models
{
    public class CategoryProduct : BaseEntity
    {
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int DisplayOrder { get; set; }
    }
}