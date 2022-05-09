namespace BTL.Areas.Admin.ViewModel
{
    public class RoleModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSystemRole { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public string CreateAt { get; set; }
        public string UpdateAt { get; set; }
    }
}