namespace BTL.Areas.Admin.ViewModel
{
    public class ProductPictureModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int PictureId { get; set; }
        public string PictureUrl { get; set; }
        public string Title { get; set; }
        public string Alt { get; set; }
        public int DisplayOrder { get; set; }
    }
}