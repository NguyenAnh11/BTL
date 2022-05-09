using BTL.Data;
using BTL.Extensions;
using BTL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BTL.Areas.Admin.ViewModel;

namespace BTL.Services
{
    public class RoleService : AbstractEntityService<Role>, IRoleService
    {
        public RoleService(ShopDbContext db) : base(db)
        {

        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            if(roleName.IsEmpty())
                throw new ArgumentNullException(nameof(roleName));

            var role = await Table.FirstOrDefaultAsync(p => p.Name == roleName);

            return role;
        }
        public async Task<IList<Role>> GetRolesByUserAsync(User user, bool onlyActive = false)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var source = !onlyActive ? Table : Table.Where(p => p.IsActive);

            var roles = await _db.Set<UserRole>()
                .Where(p => p.UserId == user.Id)
                .Join(
                    source,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r
                 )
                .ToListAsync();

            return roles;
        }

        public async Task<bool> IsAdminRoleAsync(User user)
        {
            return (await IsUserInRoleAsync(user, "ADMIN"));
        }

        public async Task<bool> IsRegisterRoleAsync(User user)
        {
            return (await IsUserInRoleAsync(user, "REGISTER"));
        }

        public async Task<bool> IsUserInRoleAsync(User user, string name)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (name.IsEmpty())
                throw new ArgumentNullException(nameof(name));

            var role = await Table.FirstOrDefaultAsync(p => p.Name == name);

            if (role == null)
                throw new ArgumentNullException(nameof(role));

            return (await _db.Set<UserRole>().AnyAsync(p => p.UserId == user.Id && p.RoleId == role.Id));
        }
        public async Task<Response> DeleteRoleAsync(int id)
        {
            using(var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var role = await GetByIdAsync(id);

                    if (role == null)
                        throw new ArgumentNullException(nameof(role));

                    if (role.IsSystemRole)
                        return Response.Fail("Không được xóa quyền hệ thống");

                    var roles = await Table.Where(p => p.IsActive).ToListAsync();

                    if (roles.Count == 1 && roles[0].IsSystemRole)
                        return Response.Fail("Tồn tại 1 quyền trong hệ thống hoạt động");

                    _db.Set<Role>().Remove(role);

                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    return Response.Ok();
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