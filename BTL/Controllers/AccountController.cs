using BTL.Data;
using BTL.Helpers;
using BTL.Models;
using BTL.ViewModels.Account;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BTL.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly ShopDbContext _db = new ShopDbContext();
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _db.Users.FirstOrDefaultAsync(p => p.Email == model.Email);
            if (user == null || user.IsDeleted)
            {
                ViewBag.Error = $"Email {model.Email} không tồn tại";
                return View(model);
            }
            var userPassword = await _db.UserPasswords.FirstOrDefaultAsync(p => p.UserId == user.Id);

            var password = HashHelper.CreatePasswordHash(model.Password, userPassword.SaltKey, ConstraintHelper.PASSWORD_HASHALGORITHM);
            if (password != userPassword.Password)
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng";
                return View(model);
            }

            var role = await _db.Roles.FirstOrDefaultAsync(p => p.Id == user.RoleId);

            Session["Id"] = user.Id;
            Session["Name"] = string.Join(" ", user.FirstName, user.LastName);
            Session["Email"] = user.Email;
            Session["Phone"] = user.Phone;
            Session["Role"] = role;
            
            if(role.IsAdmin)
            {
                return RedirectToAction("Index", "Home", new { Area = "Admin" });
            }

            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _db.Users.FirstOrDefaultAsync(p => p.Email == model.Email);
            if(user != null)
            {
                ViewBag.Error = $"Email {model.Email} đã tồn tại";
                return View(model);
            }

            user = await _db.Users.FirstOrDefaultAsync(p => p.Phone == model.PhoneNumber);
            if (user != null)
            {
                ViewBag.Error = $"Số điện thoại đã được sử dụng";
                return View(model);
            }

            user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.PhoneNumber,
                Gender = model.Gender,
            };

            var saltKey = HashHelper.CreateSaltKey(ConstraintHelper.PASSWORD_SALTKEYSIZE);
            var password = HashHelper.CreatePasswordHash(model.Password, saltKey, ConstraintHelper.PASSWORD_HASHALGORITHM);
            _db.UserPasswords.Add(new UserPassword { Password = password, SaltKey = saltKey, User = user });

            var role = await _db.Roles.FirstOrDefaultAsync(p => p.Name == ConstraintHelper.ROLE_REGISTER);

            if (role == null)
                throw new Exception($"Not find role with name {ConstraintHelper.ROLE_REGISTER}");

            user.RoleId = role.Id;

            _db.Users.Add(user);

            await _db.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction("Login", "Account", new { Area = string.Empty });
        }
    }
}
