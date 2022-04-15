using BTL.Models;
using BTL.Helpers;
using System.Threading.Tasks;

namespace BTL.Services
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IUserService _userService;
        private readonly IEncryptionService _encryptionService;
        public AuthenticateService(IUserService userService, IEncryptionService encryptionService)
        {
            _userService = userService;
            _encryptionService = encryptionService;
        }

        private bool IsMatch(UserPassword userPassword, string pasword)
        {
            var savedPassword = _encryptionService.CreatePasswordHash(pasword, userPassword.SaltKey, ConstraintHelper.PASSWORD_HASHALGORITHM);

            return savedPassword == userPassword.Password;
        }

        public async Task<(string, User)> SignInUserAsync(string email, string password)
        {
            if(!CommonHelper.IsValidEmail(email))
            {
                return ("Email không đúng định dạng", null);
            }    

            if(!CommonHelper.IsValidPassword(password))
            {
                return ("Mật khẩu chỉ chứa số và 6 ký tự", null);
            }  
            
            var user = await _userService.GetUserByEmailAsync(email);

            if(user == null || user.IsDeleted)
            {
                return ("Email không tồn tại", null);
            }

            var match = IsMatch(await _userService.GetPasswordByUserAsync(user), password);

            if (!match)
            {
                return ("Email hoặc mật khẩu không chính xác", null);
            }

            return (string.Empty, user);
        }
    }
}