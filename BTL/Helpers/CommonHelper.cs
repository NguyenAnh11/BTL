using System.Text.RegularExpressions;

namespace BTL.Helpers
{
    public static class CommonHelper
    {
        public static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; // suggested by @TK-421
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidPhone(string phone)
        {
            Regex regex = new Regex(@"(?<!\d)\d{10}(?!\d)");

            return regex.IsMatch(phone);
        }

        public static bool IsValidPassword(string password)
        {
            Regex regex = new Regex(@"(?<!\d)\d{6}(?!\d)");

            return regex.IsMatch(password);
        }
    }
}