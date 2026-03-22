using System;

namespace GameStore.Web.ExtensionsMethods
{
    public static class ConvertUtcTimeExtensions
    {
        public static DateTime ConvertUtcToMoscowTime(this DateTime dateTime)
        {
            return ConvertUtcToLocalTimeByTimeZoneId(dateTime, "Russian Standard Time");
        }
        public static DateTime ConvertUtcToSaratovTime(this DateTime dateTime)
        {
            return ConvertUtcToLocalTimeByTimeZoneId(dateTime, "Saratov Standard Time");
        }

        public static DateTime ConvertUtcToLocalTimeByTimeZoneId(DateTime dateTime, string timeZoneId)
        {
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, tz);
        }
    }
    
}
