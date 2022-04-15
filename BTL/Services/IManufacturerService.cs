using BTL.Areas.Admin.ViewModel;
using BTL.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BTL.Services
{
    public interface IManufacturerService : IAbstractEntityService<Manufacturer>
    {
        Task<IList<Manufacturer>> GetManufacturerByCategoryIdAsync(int categoryId, bool onlyPublished = false);
        Task DeleteManufacturerAsync(int id);
    }
}