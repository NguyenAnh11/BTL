using System.Web.Mvc;

namespace BTL.Areas.Admin.ViewModel
{
    public class CategorySearchModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }    
        public int CategoryPublishdStatusId { get; set; }
        public SelectList CategoryPublishStatus { get; set; }
    }
}