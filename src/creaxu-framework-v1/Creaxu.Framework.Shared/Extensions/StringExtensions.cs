using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Creaxu.Framework.Shared.Extensions
{
   public static class StringExtensions
   {
      public static string RemoveSpecialCharacters(this string str)
      {
         return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "-", RegexOptions.Compiled);
      }

      public static bool ValidateEmail(this string email)
      {
         const string pattern = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

         if (Regex.IsMatch(email, pattern))
            return Regex.Replace(email, pattern, string.Empty).Length == 0;

         return false;
      }

      public static bool StartsWith(this byte[] original, byte[] subString)
      {
         for (var i = 0; i < subString.Length; i++)
            if (original[i] != subString[i])
               return false;

         return true;
      }

      public static byte[] SubArray(this byte[] original, int start)
      {
         return SubArray(original, start, original.Length);
      }

      public static byte[] SubArray(this byte[] original, int start, int end)
      {
         var length = end - start;
         var result = new byte[length];
         for (var i = 0; i < length; i++)
            result[i] = original[start + i];

         return result;
      }

      public static byte[] ReadFully(this Stream input)
      {
         using (var ms = new MemoryStream())
         {
            input.CopyTo(ms);
            return ms.ToArray();
         }
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

      public static Guid ToGuid(this string code)
      {
         using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
         {
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(code);
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            return new Guid(hashBytes);
         }
      }

      public static bool IsBase64(this string base64String)
      {
         if (string.IsNullOrEmpty(base64String) || base64String.Length % 4 != 0
            || base64String.Contains(" ") || base64String.Contains("\t") || base64String.Contains("\r") || base64String.Contains("\n"))
            return false;

         try
         {
            Convert.FromBase64String(base64String);
            return true;
         }
         catch (Exception exception)
         {
            // Handle the exception
         }
         return false;
      }

      public static string ReplaceFirst(this string text, string search, string replace)
      {
         int pos = text.IndexOf(search);
         if (pos < 0)
         {
            return text;
         }
         return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
      }

      public static List<string> ExtractEmails(this string text)
      {
         List<string> r = new List<string>();

         Regex emailRegex = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*",
             RegexOptions.IgnoreCase);
         //find items that matches with our pattern
         MatchCollection emailMatches = emailRegex.Matches(text);

         foreach (Match emailMatch in emailMatches)
         {
            r.Add(emailMatch.Value);
         }

         return r;
      }

        public static string Capitalize(this string text)
        {
            return char.ToUpper(text[0]) + text.Substring(1);
        }
    }
}
