using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BTL.Services
{
    public class CommonService : ICommonService
    {
        private readonly IRoleService _roleService;
        private readonly ICategoryService _categoryService;
        private readonly IDateTimeService _dateTimeService;
        private readonly IManufacturerService _manufacturerService;

        public CommonService(
            IRoleService roleService,
            ICategoryService categoryService,
            IDateTimeService dateTimeService,
            IManufacturerService manufacturerService)
        {
            _roleService = roleService;
            _categoryService = categoryService;
            _dateTimeService = dateTimeService;
            _manufacturerService = manufacturerService;
        }

        private async Task<IList<SelectListItem>> GetCategoryListAsync(bool onlyPublished = false)
        {
            var selectListItems = new List<SelectListItem>();

            var categories = await _categoryService.GetAllAsync((query) =>
            {
                if (onlyPublished)
                    query = query.Where(p => p.IsPublished);
                return query;
            });

            foreach (var category in categories)
            {
                var breadcrumb = await _categoryService.GetFormatedBreadcrumbAsync(category, categories);

                selectListItems.Add(new SelectListItem
                {
                    Text = breadcrumb,
                    Value = category.Id.ToString()
                });
            }

            return selectListItems;
        }

        private async Task<IList<SelectListItem>> GetManufacturerListAsync(bool onlyPublished = false)
        {
            var selectListItems = new List<SelectListItem>();

            var manufacturers = await _manufacturerService.GetAllAsync((query) =>
            {
                if (onlyPublished)
                    query = query.Where(p => p.IsPublished);
                return query;
            });

            foreach (var manufacturer in manufacturers)
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = manufacturer.Name,
                    Value = manufacturer.Id.ToString()
                });
            }

            return selectListItems;
        }

        private async Task<IList<SelectListItem>> GetRoleListAsync(bool onlyActive = false)
        {
            var selectListItems = new List<SelectListItem>();

            var roles = await _roleService.GetAllAsync((query) =>
            {
                if(onlyActive)
                    query = query.Where(p => p.IsActive);

                return query;
            });

            foreach(var role in roles)
            {
                selectListItems.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString()
                });
            }

            return selectListItems;
        }

        private void PrepareDefaultItem(IList<SelectListItem> selectListItems, bool includeDefault, string defaulTextItem)
        {
            if (selectListItems == null)
                throw new ArgumentNullException(nameof(selectListItems));

            if (!includeDefault) return;

            if (defaulTextItem == null)
                defaulTextItem = "Tất cả";

            selectListItems.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = defaulTextItem
            });
        }

        public async Task<IList<SelectListItem>> PrepareAvaliableRolesAsync(bool includeDefault = true, string defaultText = null)
        {
            var avaliableRoles = await GetRoleListAsync();

            PrepareDefaultItem(avaliableRoles, includeDefault, defaultText);

            return avaliableRoles;
        }

        public async Task<IList<SelectListItem>> PrepareAvaliableCategoriesAsync(bool includeDefault = true, string defaultText = null)
        {
            var avaliableCategories = await GetCategoryListAsync();

            PrepareDefaultItem(avaliableCategories, includeDefault, defaultText);

            return avaliableCategories;
        }

        public async Task<IList<SelectListItem>> PrepareAvaliableManufacturerAsync(bool includeDefault = true, string defaultText = null)
        {
            var avaliableManufacturer = await GetManufacturerListAsync();

            PrepareDefaultItem(avaliableManufacturer, includeDefault, defaultText);

            return avaliableManufacturer;
        }

        public IList<SelectListItem> PrepareTimezone(bool includeDefault = true, string defaulText = null)
        {
            var timezones = _dateTimeService.GetSystemTimeZones();

            var avaliableTimeZones = new List<SelectListItem>();

            foreach (var timezone in timezones)
            {
                avaliableTimeZones.Add(new SelectListItem
                {
                    Value = timezone.Id,
                    Text = timezone.DisplayName
                });
            }

            PrepareDefaultItem(avaliableTimeZones, includeDefault, defaulText);

            return avaliableTimeZones;
        }

        public IList<SelectListItem> PrepareStatusModel()
        {
            var selectListItems = new List<SelectListItem>();

            selectListItems.Add(new SelectListItem
            {
                Value = "0",
                Text = "Chọn tình trạng",
                Selected = true
            });

            selectListItems.Add(new SelectListItem
            {
                Value = "1",
                Text = "Công khai"
            });

            selectListItems.Add(new SelectListItem
            {
                Value = "2",
                Text = "Riêng tư"
            });

            return selectListItems;
        }
    }
}