using System;
using System.Collections.ObjectModel;

namespace BTL.Services
{
    public class DateTimeService : IDateTimeService
    {
        public string Format(DateTime dateTime, string pattern = "dd/MM/yyyy HH:mm:ss")
        {
            if (dateTime == null)
                return string.Empty;

            return dateTime.ToString(pattern);
        }

        public string Format(DateTime? dateTime, string pattern = "dd/MM/yyyy HH:mm:ss")
        {
            if (dateTime == null)
                return string.Empty;

            return dateTime.Value.ToString(pattern);
        }
        public ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones();   
        }
    }
}