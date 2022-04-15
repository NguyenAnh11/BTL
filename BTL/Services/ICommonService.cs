using System.Web.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BTL.Services
{
    public interface ICommonService
    {
        IList<SelectListItem> PrepareStatusModel();
        Task<IList<SelectListItem>> PrepareAvaliableRolesAsync(bool includeDefault = true, string defaultText = null);
        Task<IList<SelectListItem>> PrepareAvaliableCategoriesAsync(bool includeDefault = true, string defaultText = null);
        Task<IList<SelectListItem>> PrepareAvaliableManufacturerAsync(bool includeDefault = true, string defaultText = null);
        IList<SelectListItem> PrepareTimezone(bool includeDefault = true, string defaulText = null);
    }
}
