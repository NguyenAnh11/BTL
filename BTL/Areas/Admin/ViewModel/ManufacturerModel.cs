namespace BTL.Areas.Admin.ViewModel
{
    public class ManufacturerModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublished { get; set; }
        public int DisplayOrder { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public int PictureId { get; set; }
        public string PictureUrl { get; set; }
        public string CreateAt { get; set; }
        public string UpdateAt { get; set; }
    }
}