using BTL.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace BTL.Models
{
    public class User : BaseEntity, ISoftDeletedEntity, IDateCreatedEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Gender { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateAt { get; set; }
        public int? PictureId { get; set; }
        public Picture Picture { get; set; }
        public IList<Order> Order { get; set; } = new List<Order>();
        public IList<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}