using System.ComponentModel.DataAnnotations;
using System.Web;

namespace BTL.Areas.Admin.ViewModel
{
    public class UserCreateUpdateModel
    {
        [Display(Name = "Mã người dùng")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Nhập họ")]
        [Display(Name = "Họ")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Nhập tên")]
        [Display(Name = "Tên")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Nhập email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Nhập số điện thoại")]
        [RegularExpression(@"\d{10}", ErrorMessage = "Số điện thoại 10 ký tụ số")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Nhập giới tính")]
        [Display(Name = "Giới tính")]
        public int? Gender { get; set; }
        [Display(Name = "Chức vụ")]
        public int RoleId { get; set; }
        [Display(Name = "Ảnh")]
        public HttpPostedFileBase Image { get; set; }
    }
}