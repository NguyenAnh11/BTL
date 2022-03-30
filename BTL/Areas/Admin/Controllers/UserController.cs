using BTL.Areas.Admin.ViewModel;
using BTL.Data;
using BTL.Filters;
using BTL.Helpers;
using BTL.Models;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace BTL.Areas.Admin.Controllers
{
    [SessionAuthorize]
    public class UserController : Controller
    {
        private readonly ShopDbContext _db = new ShopDbContext();
        [HttpGet]
        public async Task<ActionResult> Index()
        {
            var users = await _db.Users.Where(p => !p.IsDeleted)
                .AsNoTracking()
                .Include(p => p.Role)
                .Select(p => new UserModel
                {
                    Id = p.Id,
                    Name = p.FirstName + " " + p.LastName,
                    Email = p.Email,
                    Phone = p.Phone,
                    Gender = p.Gender == 0 ? "Nam" : "Nữ",
                    RoleName = p.Role.Name
                })
                .ToListAsync();

            return View(users);
        }
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> Details(int id)
        {
            var user = await _db.Users
                .AsNoTracking()
                .Include(p => p.Role)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (user == null || user.IsDeleted)
            {
                return HttpNotFound();
            }

            ViewBag.Roles = new SelectList((await _db.Roles.Where(p => p.IsActive).ToListAsync()), "Id", "Name", user.RoleId);

            var model = new UserModel()
            {
                Id = user.Id,
                Name = user.FirstName + " " + user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Gender = user.Gender == 0 ? "Nam" : "Nữ",
                PictureUrl = user.PictureUrl,
                RoleName = user.Role.Name
            };

            return View(model);

        }
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            ViewBag.Roles = new SelectList((await _db.Roles.Where(p => p.IsActive).ToListAsync()), "Id", "Name");

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(UserCreateUpdateModel model)
        {
            ViewBag.Roles = new SelectList((await _db.Roles.Where(p => p.IsActive).ToListAsync()), "Id", "Name", model.RoleId);

            if (model.Gender == null)
            {
                ViewBag.Error = "Chọn giới tính";
                return View(model);
            }

            var user = await _db.Users.FirstOrDefaultAsync(p => p.Email == model.Email);
            if (user != null)
            {
                ViewBag.Error = "Email đã tồn tại";
                return View(model);
            }
            user = await _db.Users.FirstOrDefaultAsync(p => p.Phone == model.Phone);
            if (user != null)
            {
                ViewBag.Error = "Số điện thoại đã tồn tại";
                return View(model);
            } 

            var pictureUrl = string.Empty;

            if (model.Image != null && model.Image.ContentLength > 0)
            {
                var (error, msg) = GetPictureUrlFromFile(model.Image);

                if (error)
                {
                    ViewBag.Error = error;
                    return View(model);
                }
                else
                {
                    pictureUrl = msg;
                }
            }

            if (model.RoleId == 0)
            {
                ViewBag.Error = "Chọn chức vụ";
                return View(model);
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = model.Phone,
                        Gender = model.Gender.Value,
                    };

                    if (!string.IsNullOrEmpty(pictureUrl))
                    {
                        user.PictureUrl = pictureUrl;
                    }

                    user.RoleId = model.RoleId;

                    //password account
                    var saltKey = HashHelper.CreateSaltKey(ConstraintHelper.PASSWORD_SALTKEYSIZE);
                    var password = HashHelper.CreatePasswordHash(ConstraintHelper.PASSWORD_DEFAULT, saltKey, ConstraintHelper.PASSWORD_HASHALGORITHM);

                    _db.UserPasswords.Add(new UserPassword
                    {
                        SaltKey = saltKey,
                        Password = password,
                        User = user
                    });

                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    //save image
                    if (!string.IsNullOrEmpty(pictureUrl))
                    {
                        model.Image.SaveAs(pictureUrl);
                    }
                }
                catch
                {
                    transaction.Rollback();
                    ViewBag.Error = "Có lỗi xảy ra";
                }
            }

            return RedirectToAction("Index", "User", new { Area = "Admin" });
        } 
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (user == null || user.IsDeleted)
            {
                return HttpNotFound();
            }

            ViewBag.Roles = new SelectList((await _db.Roles.Where(p => p.IsActive).ToListAsync()), "Id", "Name", user.RoleId);

            var model = new UserCreateUpdateModel
            {
                Id = id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.RoleId
            };

            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Edit(UserCreateUpdateModel model)
        {
            ViewBag.Roles = new SelectList((await _db.Roles.Where(p => p.IsActive).ToListAsync()), "Id", "Name", model.RoleId);

            if(model.Gender == null)
            {
                ViewBag.Error = "Chọn giới tính";
                return View(model);
            }    

            var user = await _db.Users.FirstOrDefaultAsync(p => p.Id == model.Id);

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var pictureUrl = string.Empty;

                    if (model.Image != null && model.Image.ContentLength > 0)
                    {
                        var (err, msg) = GetPictureUrlFromFile(model.Image);

                        if (err)
                        {
                            ViewBag.Error = msg;
                            return View(model);
                        }
                        else
                        {
                            pictureUrl = msg;
                        }
                    }

                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.RoleId = model.RoleId;
                    user.Gender = model.Gender.Value;
                    
                    var previousUrl = user.PictureUrl;

                    if (!string.IsNullOrEmpty(pictureUrl))
                    {
                        //delete previous image
                        if (!string.IsNullOrEmpty(previousUrl))
                        {
                            System.IO.File.Delete(previousUrl);
                        }

                        model.Image.SaveAs(pictureUrl);

                        user.PictureUrl = pictureUrl;
                    }

                    await _db.SaveChangesAsync();

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    ViewBag.Error = "Có lỗi xảy ra";
                }
            }

            return RedirectToAction("Index");
        }
        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(p => p.Id == id);

            if(user == null || user.IsDeleted)
            {
                return HttpNotFound();
            }

            _db.Users.Remove(user);

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        private (bool, string) GetPictureUrlFromFile(HttpPostedFileBase file)
        {
            var extension = Path.GetExtension(file.FileName);
            var allowExtensions = ConstraintHelper.PICTURE_ALLOW_EXTENSIONS.Split('|');
            if (!allowExtensions.Any(p => p == extension))
            {
                return (true, "Định dạng file không đúng");
            }

            var pictureName = Guid.NewGuid().ToString() + extension;
            var pictureDirectory = ConstraintHelper.PICTURE_DIRECTORY;

            var pictureUrl = Path.Combine(pictureDirectory, pictureName);

            return (false, pictureUrl);
        }
    }
}