using BTL.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace BTL.Models
{
    public class ProductReview : BaseEntity, IDateCreatedEntity
    {
        public DateTime CreateAt { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public int? ParentId { get; set; }
        public IList<ProductReview> Replies { get; set; } = new List<ProductReview>();  
    }
}