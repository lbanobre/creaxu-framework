using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Creaxu.Framework.Helpers
{
    public static class PhoneHelper
    {
        public static string Clean(string phone)
        {
            return Regex.Replace(phone, @"[^\d]", "");
        }


        public static string Format(string phone)
        {
            phone = Clean(phone);

            // Second, format numbers to phone string 
            if (phone.Length > 0)
            {
                phone = Convert.ToInt64(phone).ToString("+# (###) ###-####");
            }

            return phone;
        }

        public static string Raw(string phone)
        {
            var cleaned = Clean(phone);

            if (cleaned.Length == 10)
            {
                return $"1{cleaned}";
            }

            return cleaned;
        }

        public static bool IsValid(string phone)
        {
            var cleaned = Clean(phone);

            if (cleaned.Length == 10)
                return true;

            if (cleaned.Length == 11 && cleaned[0] == '1')
                return true;

            return false;
        }
    }
}