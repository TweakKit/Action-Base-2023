using System;

namespace Runtime.Manager.Time
{
    public struct TimeConstant
    {
        public const int SECOND_OF_DAY = 86400;
        public const int SECOND_OF_HOUR = 3600;
        public const int SECOND_OF_MINUTE = 60;
        public const int SYNC_TIME_INTERVAL = 300;
        public static readonly DateTime TimeBase = new(1970, 1, 1);
        public static readonly DateTime TimeDefaultMin = new(2020, 1, 1);
        public const string FORMAT_DAY_HOUR_MINUTE_SECOND = "d'D:'hh'H'";
        public const string FORMAT_HOUR_MINUTE_SECOND = "hh':'mm':'ss";
        public const string FORMAT_MINUTE_SECOND = "mm':'ss";
    }
}