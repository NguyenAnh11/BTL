using BTL.Areas.Admin.ViewModel;
using BTL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BTL.Services
{
    public interface IRoleService : IAbstractEntityService<Role>
    {
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<IList<Role>> GetRolesByUserAsync(User user, bool onlyActive = false);
        Task<bool> IsUserInRoleAsync(User user, string name);
        Task<bool> IsAdminRoleAsync(User user);
        Task<bool> IsRegisterRoleAsync(User user);
        Task<Response> DeleteRoleAsync(int id);
    }
}
