namespace BTL.Extensions
{
    public static class StringExtension
    {
        public static bool IsEmpty(this string value)
        {
            if (value == null)
                return true;

            return string.IsNullOrEmpty(value.Trim());
        }
    }
}