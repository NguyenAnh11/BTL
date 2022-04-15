using BTL.Models;
using System.Threading.Tasks;

namespace BTL.Services
{
    public interface IUserService : IAbstractEntityService<User>
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByPhoneAsync(string phone);
        Task<UserPassword> GetPasswordByUserAsync(User user);
        Task SaveUserToRoleAsync(User user, int[] roleIds);
        Task SaveUserToPasswordAsync(User user, string pasword);
    }
}
