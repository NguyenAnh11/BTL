using BTL.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace BTL.Models
{
    public class Product : BaseEntity, ISoftDeletedEntity, IDateCreatedEntity, IDateUpdatedEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public string Sku { get; set; }
        public string Gtin { get; set; }
        public decimal Price { get; set; }
        public int DisplayOrder { get; set; }
        public int StockQuantity { get; set; }
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsShowOnHomePage { get; set; }
        public bool IsMarkAsNew { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public IList<ProductPicture> ProductPictures { get; set; } = new List<ProductPicture>();    
        public IList<CategoryProduct> CategoryProducts { get; set; } = new List<CategoryProduct>();
    }
}