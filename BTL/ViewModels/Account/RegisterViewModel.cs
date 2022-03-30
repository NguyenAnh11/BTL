using System.ComponentModel.DataAnnotations;

namespace BTL.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nhập họ")]
        [Display(Name = "Họ")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Nhập tên")]
        [Display(Name = "Tên")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Chọn giới tính")]
        [Display(Name = "Giới tính")]
        public int Gender { get; set; } 
        [Required(ErrorMessage = "Nhập số điện thoại")]
        [RegularExpression(@"\d{10}", ErrorMessage = "Số điện thoại 10 ký tụ số")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } 
        [Required(ErrorMessage = "Nhập email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Nhập mật khẩu")]
        [RegularExpression(@"\d{6}", ErrorMessage = "Mật khẩu yêu cầu 6 ký tự số")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Nhập xác thực mật khẩu")]
        [RegularExpression(@"\d{6}", ErrorMessage = "Mật khẩu yêu cầu 6 ký tự số")]
        [Compare("Password", ErrorMessage = "Mật khẩu và xác thực mật khẩu không khớp")]
        [Display(Name = "Nhập lại mật khẩu")]
        public string ConfirmPassword { get; set; }
    }
}