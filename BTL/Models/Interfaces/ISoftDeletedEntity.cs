namespace BTL.Models.Interfaces
{
    public interface ISoftDeletedEntity
    {
        bool IsDeleted { get; set; }
    }
}