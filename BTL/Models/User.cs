using System.Collections.Generic;
using BTL.Models.Interfaces;

namespace BTL.Models
{
    public class User : ISoftDeletedEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }    
        public string Email { get; set; }
        public string Phone { get; set; }
        public int Gender { get; set; }
        public string PictureUrl { get; set; }
        public bool IsDeleted { get; set; }
        public int RoleId { get; set; }  
        public Role Role { get; set; }  
        public IList<Order> Order { get; set; } = new List<Order>(); 
    }
}