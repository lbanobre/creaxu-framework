using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creaxu.Core.Helpers
{
    public static class IdGenerator
    {
        private static readonly char[] BaseChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_".ToCharArray();

        private static readonly Random Random = new Random();

        public static string GetId10(int length)
        {
            return GetId(length, 10);
        }

        public static string GetId36(int length)
        {
            return GetId(length, 36);
        }

        public static string GetId62(int length)
        {
            return GetId(length, 62);
        }

        public static string GetId64(int length)
        {
            return GetId(length, 64);
        }

        private static string GetId(int length, int chars)
        {
            var sb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
                sb.Append(BaseChars[Random.Next(chars)]);

            return sb.ToString();
        }
    }
}
