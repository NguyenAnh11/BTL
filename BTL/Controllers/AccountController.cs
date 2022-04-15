using BTL.ViewModels.Account;
using BTL.Data;
using BTL.Models;
using BTL.Services;
using BTL.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BTL.Controllers
{
    public class AccountController : Controller
    {
        private readonly ShopDbContext _db;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService; 
        private readonly IEncryptionService _encryptionService;
        private readonly IAuthenticateService _authenticateService;
        public AccountController(
            ShopDbContext db,
            IRoleService roleService, 
            IUserService userService,
            IEncryptionService encryptionService,
            IAuthenticateService authenticateService)
        {
            _db = db;
            _roleService = roleService; 
            _userService = userService; 
            _encryptionService = encryptionService;
            _authenticateService = authenticateService;
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model)
        {
            var (msg, user) = await _authenticateService.SignInUserAsync(model.Email, model.Password);

            if(user == null)
            {
                return Json(new { success = false, msg }, JsonRequestBehavior.AllowGet);
            }

            var roles = await _roleService.GetRolesByUserAsync(user);

            var returnUrl = string.Empty;

            bool isAdmin = roles.Any(p => p.Name == ConstraintHelper.ROLE_ADMINSTRATOR);

            if (isAdmin)
                returnUrl = "/Admin/Product";
            else
                returnUrl = "/";

            Session["Id"] = user.Id;
            Session["Name"] = string.Join(" ", user.FirstName, user.LastName);
            Session["Email"] = user.Email;
            Session["Phone"] = user.Phone;
            Session["Role"] = roles;
            Session["IsAdmin"] = isAdmin;

            return Json(new { success = true, returnUrl = returnUrl }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if(!CommonHelper.IsValidEmail(model.Email))
            {
                return Json(new { success = false, msg = "Email không hợp lệ" }, JsonRequestBehavior.AllowGet);
            }    

            if(!CommonHelper.IsValidPhone(model.PhoneNumber))
            {
                return Json(new { success = false, msg = "Số điện thoại chỉ chứa số" }, JsonRequestBehavior.AllowGet);
            }    

            if(!CommonHelper.IsValidPassword(model.Password))
            {
                return Json(new { success = false, msg = "Mật khẩu chỉ chứa số và có 6 ký tự" }, JsonRequestBehavior.AllowGet);
            }    

            if(model.Password != model.ConfirmPassword)
            {
                return Json(new { success = false, msg = "Mật khẩu và xác thực mật khẩu không khớp" }, JsonRequestBehavior.AllowGet);
            }    

            var user = await _userService.GetUserByEmailAsync(model.Email);

            if(user != null)
            {
                return Json(new { success = false, msg = "Tài khoản email đã được đăng ký" }, JsonRequestBehavior.AllowGet);
            }

            user = await _userService.GetUserByPhoneAsync(model.PhoneNumber);

            if(user != null)
            {
                return Json(new { success = false, msg = "Tài khoản số điện thoại đã được đăng ký" }, JsonRequestBehavior.AllowGet);
            }    

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Gender = model.Gender,
                        Email = model.Email,
                        Phone = model.PhoneNumber
                    };

                    _db.Set<User>().Add(user);  
                    
                    await _db.SaveChangesAsync();

                    var saltKey = _encryptionService.CreateSaltKey(ConstraintHelper.PASSWORD_SALTKEYSIZE);
                    var password = _encryptionService.CreatePasswordHash(model.Password, saltKey, ConstraintHelper.PASSWORD_HASHALGORITHM);

                    _db.Set<UserPassword>().Add(new UserPassword
                    {
                        UserId = user.Id,
                        Password = password,
                        SaltKey = saltKey
                    });

                    var registerRole = await _roleService.GetRoleByNameAsync(ConstraintHelper.ROLE_REGISTER);
                    _db.Set<UserRole>().Add(new UserRole
                    {
                        RoleId = registerRole.Id,
                        UserId = user.Id,
                    });

                    await _db.SaveChangesAsync();

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
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}
