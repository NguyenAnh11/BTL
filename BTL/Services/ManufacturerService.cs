using BTL.Areas.Admin.ViewModel;
using BTL.Data;
using BTL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BTL.Services
{
    public class ManufacturerService : AbstractSoftDeleteEntityService<Manufacturer>, IManufacturerService
    {
        private readonly IPictureService _pictureService;

        public ManufacturerService(ShopDbContext db, IPictureService pictureService) : base(db)
        {
            _pictureService = pictureService;
        }

        public async Task DeleteManufacturerAsync(int id)
        {
            if(id == 0)
                throw new ArgumentException(nameof(id));    

            var manufacturer = await GetByIdAsync(id);  
            
            if(manufacturer == null)
                throw new ArgumentException(nameof(manufacturer));

            _db.Set<Manufacturer>().Remove(manufacturer);

            await _db.SaveChangesAsync();
        }

        public async Task<IList<Manufacturer>> GetManufacturerByCategoryIdAsync(int categoryId, bool onlyPublished = false)
        {
            if(categoryId == 0)
                throw new ArgumentException(nameof(categoryId));

            var source = !onlyPublished ? Table : Table.Where(p => p.IsPublished);

            var manufacturers = await _db.Set<CategoryManufacturer>()
                    .Where(p => p.CategoryId == categoryId)
                    .Join(source, cm => cm.ManufacturerId, m => m.Id, (cm, m) => m)
                    .ToListAsync();

            return manufacturers;
        }
    }
}