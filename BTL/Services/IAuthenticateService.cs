using BTL.Models;
using System.Threading.Tasks;

namespace BTL.Services
{
    public interface IAuthenticateService
    {
        Task<(string, User)> SignInUserAsync(string email, string password);
    }
}
