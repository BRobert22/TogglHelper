using System;

namespace TogglHelper.Helpers
{
    internal class TimeHelper
    {
        internal static string ConvertSecondsToString(long seconds)
        {
            var milliseconds = seconds * 1000;
            var t = TimeSpan.FromMilliseconds(milliseconds);
            return $"{t.Hours:D2}h:{t.Minutes:D2}m:{t.Seconds:D2}s";
        }

        internal static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();

            var date = new DateTime(dtDateTime.Year, dtDateTime.Month, dtDateTime.Day, 0, 0, 0);
            return date;
        }

        internal static DateTime ConvertToStartDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
        }

        internal static DateTime ConvertToEndDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        }
    }
}