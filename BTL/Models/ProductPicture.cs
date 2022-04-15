namespace BTL.Models
{
    public class ProductPicture : BaseEntity
    {
        public int ProductId { get; set; }
        public Product  Product { get; set; }
        public int PictureId { get; set; }
        public Picture Picture { get; set; }
        public int DisplayOrder { get; set; }
    }
}