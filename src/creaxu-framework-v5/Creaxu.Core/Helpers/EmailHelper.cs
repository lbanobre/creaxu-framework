using System.Text.RegularExpressions;

namespace Creaxu.Core.Helpers
{
    public static class EmailHelper
    {
        public static bool IsValid(string email)  
        {  
            if (string.IsNullOrEmpty(email))
                return false;
            
            return Regex.IsMatch(email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }
        
        public static string Clean(string email)
        {
            return string.IsNullOrEmpty(email) ? null : email.TrimEnd().ToLower();
        } 
    }
}