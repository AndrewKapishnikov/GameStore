using System;

namespace GameStore.Web.ExtensionsMethods
{
    public static class ConvertUtcTimeExtensions
    {
        public static DateTime ConvertUtcToMoscowTime(this DateTime dateTime)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
            DateTimeOffset time = TimeZoneInfo.ConvertTimeFromUtc(dateTime, tz);
            return time.DateTime;
        }
    }
    
}
