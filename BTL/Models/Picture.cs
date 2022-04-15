namespace BTL.Models
{
    public class Picture : BaseEntity
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public string Alt { get; set; }
    }
}