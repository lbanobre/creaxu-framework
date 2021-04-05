using System.Text.RegularExpressions;

namespace Creaxu.Core.Helpers
{
    public static class EmailHelper
    {
        public static bool IsValid(string email)  
        {  
            if (string.IsNullOrEmpty(email))
                return false;
            
            var regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");  
            var match = regex.Match(email);

            return match.Success;
        }
        
        public static string Clean(string email)
        {
            return string.IsNullOrEmpty(email) ? null : email.TrimEnd().ToLower();
        } 
    }
}