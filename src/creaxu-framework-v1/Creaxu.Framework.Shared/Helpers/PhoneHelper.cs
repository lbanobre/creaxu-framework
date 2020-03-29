using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Creaxu.Framework.Shared.Helpers
{
    public static class PhoneHelper
    {
        public static string Clean(string phone)
        {
            // First, remove everything except the numbers
            var regexObj = new Regex(@"[^\d]");
            phone = regexObj.Replace(phone, "");

            return phone;
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
            phone = Clean(phone);

            if (phone.Length == 10)
            {
                phone = $"1{Convert.ToInt64(phone)}";
            }

            return Convert.ToInt64(phone).ToString();
        }
    }
}