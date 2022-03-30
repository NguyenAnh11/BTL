using System;

namespace BTL.Models.Interfaces
{
    public interface IDateUpdatedEntity
    {
        DateTime? UpdateAt { get; set; }
    }
}
