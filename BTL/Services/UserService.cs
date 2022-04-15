using BTL.Data;
using BTL.Extensions;
using BTL.Models;
using BTL.Helpers;
using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

namespace BTL.Services
{
    public class UserService : AbstractSoftDeleteEntityService<User>, IUserService
    {
        private readonly IEncryptionService _encryptionService;
        public UserService(
            ShopDbContext db, 
            IEncryptionService encryptionService
            ) : base(db)
        {
            _encryptionService = encryptionService;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (email.IsEmpty())
                return null;

            var user = await Table.FirstOrDefaultAsync(p => p.Email == email);

            return user;
        }

        public async Task<User> GetUserByPhoneAsync(string phone)
        {
            if (phone.IsEmpty())
                return null;

            var user = await Table.FirstOrDefaultAsync(p => p.Phone == phone);

            return user;
        }

        public async Task<UserPassword> GetPasswordByUserAsync(User user)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            var password = await _db.Set<UserPassword>().FirstOrDefaultAsync(p => p.UserId == user.Id);

            return password;
        }
        
        public async Task SaveUserToPasswordAsync(User user, string pasword)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            if (pasword.IsEmpty())
                throw new ArgumentNullException(nameof(pasword));

            var saltKey = _encryptionService.CreateSaltKey(ConstraintHelper.PASSWORD_SALTKEYSIZE);
            var pwd = _encryptionService.CreatePasswordHash(pasword, saltKey, ConstraintHelper.PASSWORD_HASHALGORITHM);

            _db.Set<UserPassword>().Add(new UserPassword
            {
                UserId = user.Id,
                Password = pwd,
                SaltKey = saltKey
            });

            await _db.SaveChangesAsync();
        }

        public async Task SaveUserToRoleAsync(User user, int[] roleIds)
        {
            if(user == null)
                throw new ArgumentNullException(nameof(user));

            if (roleIds == null)
                throw new ArgumentNullException(nameof(roleIds));

            var existUserRoles = await _db.Set<UserRole>().Where(p => p.UserId == user.Id).ToListAsync();

            if(existUserRoles.Any())
            {
                foreach(var userRole in existUserRoles)
                {
                    if(!roleIds.Contains(userRole.RoleId))
                    {
                        _db.Set<UserRole>().Remove(userRole);
                    }    
                }    
            }    

            foreach(var roleId in roleIds)
            {
                if(!existUserRoles.Any(p => p.RoleId == roleId))
                {
                    var model = new UserRole
                    {
                        UserId = user.Id,
                        RoleId = roleId,
                    };

                    _db.Set<UserRole>().Add(model);

                    await _db.SaveChangesAsync();   
                }    
                    
            }    
        }
    }
}

















































































































































































































































































































































































































































































