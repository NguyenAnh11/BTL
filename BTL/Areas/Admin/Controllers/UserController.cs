using BTL.Areas.Admin.ViewModel;
using BTL.Data;
using BTL.Models;
using BTL.Services;
using BTL.Extensions;
using BTL.Helpers;
using BTL.Filters;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BTL.Areas.Admin.Controllers
{
    [SessionAuthentication]
    [AdminAuthrorize]
    public class UserController : Controller
    {
        private readonly ShopDbContext _db;
        private readonly ICommonService _commonService;
        private readonly IPictureService _pictureService;
        private readonly IDateTimeService _dateService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UserController(
            ShopDbContext db, 
            ICommonService commonService, 
            IPictureService pictureService,
            IDateTimeService dateTimeService,
            IUserService userService, 
            IRoleService roleService)
        {
            _db = db;   
            _commonService = commonService;
            _pictureService = pictureService;
            _dateService = dateTimeService; 
            _userService = userService;
            _roleService = roleService; 
        }
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            ViewBag.AvaliableRoles = await _commonService.PrepareAvaliableRolesAsync();

            return View(new UserSearchModel()
            {
                RoleId = new int[] { 0 }
            });
        }
        [HttpPost]
        public async Task<ActionResult> Index(UserSearchModel searchModel)
        {
            var users = await _userService.GetAllAsync((query) =>
            {
                if(searchModel.Id != 0)
                {
                    query = query.Where(p => p.Id == searchModel.Id);
                }

                if(!searchModel.Email.IsEmpty())
                {
                    query = query.Where(p => p.Email.Contains(searchModel.Email));  
                }

                if(!searchModel.Phone.IsEmpty())
                {
                    query = query.Where(p => p.Phone == searchModel.Phone);
                }

                if(searchModel.RoleId.Length > 0)
                {
                    var roleIds = searchModel.RoleId.ToList();

                    if (roleIds.Contains(0))
                        roleIds.Remove(0);

                    if(roleIds.Count > 0)
                    {
                        var source = _db.Set<UserRole>().Where(p => roleIds.Contains(p.Id)).Distinct().ToList();

                        query = query.Join(source, u => u.Id, ur => ur.UserId, (u, ur) => u);
                    }    
                }

                return query;
            }); 

            var models = new List<UserModel>(); 

            foreach(var user in users)
            {
                var model = new UserModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Gender = user.Gender,
                    CreateAt = _dateService.Format(user.CreateAt),
                    PictureId = user.PictureId ?? 0,
                    PictureUrl = await _pictureService.GetPictureUrlAsync(user.PictureId ?? 0, PictureType.Avatar),
                };

                var roles = await _roleService.GetRolesByUserAsync(user);

                model.RoleId = roles.Select(p => p.Id).ToArray();
                model.Role = string.Join(" ", roles.Select(p => p.Name));

                models.Add(model);
            }

            return Json(new { data = models }, JsonRequestBehavior.AllowGet); ;
        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var avaliableRole = await _commonService.PrepareAvaliableRolesAsync(false);

            var registerRole = await _roleService.GetRoleByNameAsync(ConstraintHelper.ROLE_REGISTER);

            foreach(var role in avaliableRole)
            {
                if(int.TryParse(role.Value, out int roleId) && roleId == registerRole.Id)
                {
                    role.Selected = true;
                }    
            }    

            ViewBag.AvaliableRoles = avaliableRole;

            var model = new UserModel
            {
                PictureId = 0,
                PictureUrl = await _pictureService.GetPictureUrlAsync(0, PictureType.Avatar),
                RoleId = new int[] { registerRole.Id },
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Create(UserModel model)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    if(!CommonHelper.IsValidEmail(model.Email))
                    {
                        return Json(new { success = false, msg = "Email không hợp lệ" });
                    }  
                    
                    if(!CommonHelper.IsValidPhone(model.Phone))
                    {
                        return Json(new { success = false, msg = "Số điện thoại chỉ chứa số và 10 ký tự " });
                    }    

                    var user = await _userService.GetUserByEmailAsync(model.Email);

                    if(user != null)
                    {
                        return Json(new { success = false, msg = "Email đã tồn tại" });
                    }    

                    user = await _userService.GetUserByPhoneAsync(model.Phone);

                    if(user != null)
                    {
                        return Json(new { success = false, msg = "Số điện thoại đã tồn tại" });
                    }

                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Gender = model.Gender
                    };

                    if (model.PictureId == 0)
                        user.PictureId = null;
                    else
                        user.PictureId = model.PictureId;

                    _db.Set<User>().Add(user);

                    await _db.SaveChangesAsync();

                    //save role
                    await _userService.SaveUserToRoleAsync(user, model.RoleId);

                    //save password
                    await _userService.SaveUserToPasswordAsync(user, ConstraintHelper.PASSWORD_DEFAULT);

                    transaction.Commit();

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    throw ex;
                }
            }
        }
    }
}