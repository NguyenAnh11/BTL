using System.ComponentModel.DataAnnotations;

namespace BTL.Areas.Admin.ViewModel
{
    public class UserModel
    {
        [Display(Name = "Mã nguời dùng")]
        public int Id { get; set; }
        [Display(Name = "Tên người dùng")]
        public string Name { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }
        [Display(Name = "Giới tính")]
        public string Gender { get; set; }
        [Display(Name = "Chức vụ")]
        public string RoleName { get; set; }
        [Display(Name = "Ảnh")]
        public string PictureUrl { get; set; }  
    }
}