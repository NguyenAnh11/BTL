using BTL.Areas.Admin.ViewModel;
using BTL.Data;
using BTL.Extensions;
using BTL.Models;
using BTL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BTL.Filters;

namespace BTL.Areas.Admin.Controllers
{
    [SessionAuthentication]
    [AdminAuthrorize]
    public class RoleController : Controller
    {
        private readonly ShopDbContext _db;
        private readonly IRoleService _roleService; 
        private readonly IDateTimeService _dateTimeService;
        public RoleController(
            ShopDbContext db,
            IRoleService roleService,
            IDateTimeService dateTimeService)
        {
            _db = db;
            _roleService = roleService;
            _dateTimeService = dateTimeService;
        }
        [HttpGet]
        public ActionResult Index()
        {
            var model = new RoleSearchModel
            {
                IsSystemRole = true,
                IsActive = true,
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Index(RoleSearchModel searchModel)
        {
            var roles = await _roleService.GetAllAsync(query =>
            {
                query = query.Where(p => p.IsActive == searchModel.IsActive);

                query = query.Where(p => p.IsSystemRole == searchModel.IsSystemRole);   

                if (!searchModel.Name.IsEmpty())
                {
                    query = query.Where(p => p.Name.Contains(searchModel.Name));
                }

                return query;
            });

            var models = new List<RoleModel>();

            foreach (var role in roles)
            {
                models.Add(new RoleModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    IsActive = role.IsActive,
                    IsAdmin = role.IsAdmin,
                    IsSystemRole = role.IsSystemRole,
                    DisplayOrder = role.DisplayOrder,
                    CreateAt = _dateTimeService.Format(role.CreateAt),
                    UpdateAt = _dateTimeService.Format(role.UpdateAt),
                });
            }

            return Json(new { data = models }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Create() => View(new RoleModel());
        [HttpPost]
        public async Task<ActionResult> Create(RoleModel model)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var role = await _roleService.GetRoleByNameAsync(model.Name);

                    if (role != null)
                        return Json(new { success = false, msg = "Tên quyền đã tồn tại" }, JsonRequestBehavior.AllowGet);

                    role = new Role
                    {
                        Name = model.Name,
                        IsActive = model.IsActive,
                        IsAdmin = model.IsAdmin,
                        IsSystemRole = model.IsSystemRole,
                        DisplayOrder = model.DisplayOrder
                    };

                    _db.Set<Role>().Add(role);

                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    return Json(new { success = true, id = role.Id }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw ex;
                }
            }
        }
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var role = await _roleService.GetByIdAsync(id);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            var model = new RoleModel
            {
                Id = role.Id,
                Name = role.Name,
                IsActive = role.IsActive,
                IsAdmin = role.IsAdmin,
                IsSystemRole = role.IsSystemRole,
                DisplayOrder = role.DisplayOrder,
                CreateAt = _dateTimeService.Format(role.CreateAt),
                UpdateAt = _dateTimeService.Format(role.UpdateAt),
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(RoleModel model)
        {
            var role = await _roleService.GetByIdAsync(model.Id);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    if (!role.Name.Equals(model.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        var role1 = await _roleService.GetRoleByNameAsync(model.Name);

                        if (role1 != null && role1.Id != role.Id)
                            return Json(new { success = false, msg = "Tên quyền đã tồn tại " }, JsonRequestBehavior.AllowGet);
                    }

                    if (!model.IsActive)
                    {
                        var roles = await _roleService.GetAllAsync(query => query.Where(p => p.IsActive));

                        if (roles.Count == 1 && roles[0].Id == role.Id)
                            return Json(new { success = false, msg = "Yêu cầu 1 quyền tồn tại" }, JsonRequestBehavior.AllowGet);
                    }

                    role.Name = model.Name;
                    role.IsActive = model.IsActive;
                    role.IsAdmin = model.IsAdmin;
                    role.IsSystemRole = model.IsSystemRole;
                    role.DisplayOrder = model.DisplayOrder;

                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    return Json(new { success = true, id = role.Id }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw ex;
                }
            }
        }
        [Route("{id}")]
        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            var response = await _roleService.DeleteRoleAsync(id);    

            return Json(new { response }, JsonRequestBehavior.AllowGet);
        }
    }
}