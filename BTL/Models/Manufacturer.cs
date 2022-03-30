using System;
using System.Collections.Generic;
using BTL.Models.Interfaces;

namespace BTL.Models
{
    public class Manufacturer :  ISoftDeletedEntity, IDateCreatedEntity, IDateUpdatedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public int DisplayOrder { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public string PictureUrl { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public IList<Product> Products { get; set; } = new List<Product>();
        public IList<CategoryManufacturer> CategoryManufacturers { get; set; } = new List<CategoryManufacturer>(); 
    }
}