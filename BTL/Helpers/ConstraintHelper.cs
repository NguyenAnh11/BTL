namespace BTL.Helpers
{
    public static class ConstraintHelper
    {
        public static string PASSWORD_HASHALGORITHM = "SHA512";
        public static int PASSWORD_SALTKEYSIZE = 16;
        public static string PASSWORD_DEFAULT = "123456";

        public static string ROLE_REGISTER = "Register";
        public static string ROLE_ADMINSTRATOR = "Adminstrator";

        public static int PAGE_PAGESIZE = 10;

        public static string PICTURE_DIRECTORY = @"D:\Image";
        public static string PICTURE_ALLOW_EXTENSIONS = ".png|.jpeg|.jpg";
    }
}