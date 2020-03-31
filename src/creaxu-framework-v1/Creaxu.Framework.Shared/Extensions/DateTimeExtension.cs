using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Creaxu.Framework.Shared.Extensions
{
    public static class DateTimeExtension
    {
        public static DateTime ToLocalDateTime(this DateTime utcDateTime)
        {
            var tzi  = TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == "Eastern Standard Time") ? 
                                TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time") : 
                                TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

            return ToLocalDateTime(utcDateTime, tzi.Id);
        }

        public static DateTime ToUtcDateTime(this DateTime localDateTime)
        {
            var tzi  = TimeZoneInfo.GetSystemTimeZones().Any(x => x.Id == "Eastern Standard Time") ? 
                       TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time") : 
                       TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

            return ToUtcDateTime(localDateTime, tzi.Id);
        }

        private static DateTime ToLocalDateTime(this DateTime utcDateTime, string timeZoneId)
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            utcDateTime = DateTime.SpecifyKind(utcDateTime, DateTimeKind.Utc);

            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, tzi);
        }

        private static DateTime ToUtcDateTime(this DateTime localDateTime, string timeZoneId)
        {
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            return TimeZoneInfo.ConvertTimeToUtc(localDateTime, tzi);
        }

        public static string ToRelativeString(this DateTime utcDateTime)
        {
            var localDateTime = utcDateTime.ToLocalDateTime();

            var s = localDateTime.Date.Subtract(DateTime.UtcNow.ToLocalDateTime().Date);

            var dayDiff = s.TotalDays;


            if (dayDiff == -1)
            {
                return "Yesterday at " + localDateTime.ToString("hh:mm tt");
            }

            if (dayDiff == 0)
            {
                return "Today at " + localDateTime.ToString("hh:mm tt");
            }

            if (dayDiff == 1)
            {
                return "Tomorrow at " + localDateTime.ToString("hh:mm tt");
            }

            return localDateTime.ToString("dddd, MMM dd, yyyy hh:mm tt");
        }
    }
}
