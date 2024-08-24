using System;

namespace Runtime.Manager.Time
{
    public static class TimeExtension
    {
        public static long TimeInMilliseconds(this DateTime time)
        {
            var offset = (DateTimeOffset)time;
            return offset.ToUnixTimeMilliseconds();
        }

        public static string ConvertSecondsToMinutesAndSeconds(long seconds)
        {
            long minutes = seconds / 60;
            long remainingSeconds = seconds % 60;

            string formattedTime = string.Format("{0:00}:{1:00}", minutes, remainingSeconds);

            return formattedTime;
        }
        
        public static DateTime Next(this DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }
    }
}