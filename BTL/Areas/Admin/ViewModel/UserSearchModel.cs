namespace BTL.Areas.Admin.ViewModel
{
    public class UserSearchModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int[] RoleId { get; set; }
    }
}