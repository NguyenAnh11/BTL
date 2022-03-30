using System;
using System.Collections.Generic;
using BTL.Models.Interfaces;

namespace BTL.Models
{
    public class Role : IDateCreatedEntity, IDateUpdatedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSystemRole { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
    }
}