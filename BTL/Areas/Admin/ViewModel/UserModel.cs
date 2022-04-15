using System;

namespace BTL.Areas.Admin.ViewModel
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Gender { get; set; }
        public string CreateAt { get; set; }
        public string LastActivity { get; set; }
        public int PictureId { get; set; }
        public string PictureUrl { get; set; }
        public int[] RoleId { get; set; }
        public string Role { get; set; }
    }
}