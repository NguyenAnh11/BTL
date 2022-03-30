using System;
using System.Collections.Generic;
using BTL.Models.Interfaces;

namespace BTL.Models
{
    public class Product : ISoftDeletedEntity, IDateCreatedEntity, IDateUpdatedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsShowOnHomePage { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public string PictureUrl { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }  
        public IList<CategoryProduct> CategoryProducts { get; set; } = new List<CategoryProduct>();
    }
}