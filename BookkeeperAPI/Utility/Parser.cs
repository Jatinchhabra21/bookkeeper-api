namespace BookkeeperAPI.Utility
{
    #region usings
    using Crypt = BCrypt.Net.BCrypt;
    #endregion

    internal static class Parser
    {
        internal static T ToEnum<T>(this string value) where T : struct  
        { 
            if(string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            T result;

            return Enum.TryParse<T>(value, out result) ? result : throw new Exception("Invalid value");
        }

        internal static string EncryptPassword(string password)
        {
            return Crypt.EnhancedHashPassword(password);
        }

        internal static bool IsValidPassword(string password, string hashedPassword)
        {
            return Crypt.EnhancedVerify(password, hashedPassword);
        }
    }
}
