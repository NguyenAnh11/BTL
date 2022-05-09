namespace BTL.Areas.Admin.ViewModel
{
    public class RoleSearchModel : BaseSearchModel
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystemRole { get; set; }
    }
}