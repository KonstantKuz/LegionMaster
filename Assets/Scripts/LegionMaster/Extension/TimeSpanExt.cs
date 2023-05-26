using System;
using LegionMaster.Localization.Service;

namespace LegionMaster.Extension
{
    public static class TimeSpanExt
    {
        private const string DAY = "d";
        private const string HOUR = "h";
        private const string MINUTE = "m";
        private const string SECOND = "s";
        
        public static string ToFormattedString(this TimeSpan timeSpan, LocalizationService localization, TimeUnit precision = TimeUnit.Seconds)
        {
            if (precision == TimeUnit.Day)
            {
                return $"{(int) Math.Ceiling(timeSpan.TotalDays)}{localization.Get(DAY)}";
            }
            
            if (timeSpan > TimeSpan.FromDays(1))
            {
                return $"{timeSpan.Days}{localization.Get(DAY)} {timeSpan.Hours}{localization.Get(HOUR)}";
            }
            
            if (precision == TimeUnit.Hour)
            {
                return $"{(int) Math.Ceiling(timeSpan.TotalHours)}{localization.Get(HOUR)}";
            }

            if (timeSpan > TimeSpan.FromHours(1))
            {
                return $"{timeSpan.Hours}{localization.Get(HOUR)} {timeSpan.Minutes}{localization.Get(MINUTE)}"; 
            }
            
            if (precision == TimeUnit.Minute)
            {
                return $"{(int) Math.Ceiling(timeSpan.TotalMinutes)}{localization.Get(MINUTE)}";
            }

            if (timeSpan > TimeSpan.FromMinutes(1))
            {
                return $"{timeSpan.Minutes}{localization.Get(MINUTE)} ${timeSpan.Seconds}{localization.Get(SECOND)}";
            }

            return $"{timeSpan.Seconds}{localization.Get(SECOND)}";
        }
        
    }
}