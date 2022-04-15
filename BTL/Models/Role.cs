using System;
using System.Collections.Generic;
using BTL.Models.Interfaces;

namespace BTL.Models
{
    public class Role : BaseEntity, IDateCreatedEntity, IDateUpdatedEntity
    {
        public string Name { get; set; }
        public bool IsSystemRole { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public IList<Role> Roles { get; set; } = new List<Role>();  
    }
}