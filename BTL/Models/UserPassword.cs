namespace BTL.Models
{
    public class UserPassword
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Password { get; set; }
        public string SaltKey { get; set; }
    }
}