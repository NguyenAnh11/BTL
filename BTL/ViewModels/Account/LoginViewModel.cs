using System.ComponentModel.DataAnnotations;

namespace BTL.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Nhập email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Nhập mật khẩu")]
        [RegularExpression(@"\d{6}", ErrorMessage = "Mật khẩu yêu cầu 6 ký tự số")]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        [Display(Name = "Ghi nhớ")]
        public bool RememberMe { get; set; }
    }
}