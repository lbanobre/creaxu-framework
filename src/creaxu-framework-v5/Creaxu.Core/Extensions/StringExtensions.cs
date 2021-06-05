using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Creaxu.Core.Extensions
{
    public static class StringExtensions
    {
        public static string GenerateSlug(this string phrase)
        {
            var str = phrase.ToLower();

            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // invalid chars          
            str = Regex.Replace(str, @"\s+", " ").Trim(); // convert multiple spaces into one space  
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim(); // cut and trim it  
            str = Regex.Replace(str, @"\s", "-"); // hyphens  

            return str;
        }
        
        public static string Capitalize(this string text)
        {
            return char.ToUpper(text[0]) + text.Substring(1);
        }
        
        public static string EncodeToBase64(this string toEncode, Encoding encoding = null)
        {
            return encoding == null
                ? Convert.ToBase64String(Encoding.UTF8.GetBytes(toEncode))
                : Convert.ToBase64String(encoding.GetBytes(toEncode));
        }

        public static string DecodeFromBase64(this string encodeData, Encoding encoding = null)
        {
            return encoding?.GetString(Convert.FromBase64String(encodeData))
                   ?? Encoding.UTF8.GetString(Convert.FromBase64String(encodeData));
        }

        public static string RemoveSpecialCharacters(this string input)
        {
            return Regex.Replace(input, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled);
        }
    }
}
