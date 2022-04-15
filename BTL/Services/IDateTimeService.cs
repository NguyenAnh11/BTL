using System;
using System.Collections.ObjectModel;

namespace BTL.Services
{
    public interface IDateTimeService
    {
        string Format(DateTime dateTime, string pattern = "dd/MM/yyyy HH:mm:ss");
        string Format(DateTime? dateTime, string pattern = "dd/MM/yyyy HH:mm:ss");
        ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones();
    }
}
