using System.Text.RegularExpressions;

namespace DataValidator
{
    public class DataValidator
    {
        public bool ValidateEmail(string email)
        {
            string pattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                           + "@"
                           + @"((([\w]+([-\w]*[\w]+)*\.)+[a-zA-Z]+)|"
                           + @"((([01]?[0-9]{1,2}|2[0-4][0-9]|25[0-5]).){3}[01]?[0-9]{1,2}|2[0-4][0-9]|25[0-5]))\z";
            return Regex.IsMatch(email, pattern);
        }
        public bool ValidatePassword(string password)
        {
            return true;
        }

        public bool ValidateMD5(string hash)
        {
            return Regex.IsMatch(hash, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }
    }
}
