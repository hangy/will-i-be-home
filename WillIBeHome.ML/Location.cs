using System;

namespace WillIBeHome.ML
{
    public class Location
    {
        public Location(string user, string device, string geohash, DateTimeOffset date, bool isHome)
        {
            User = user;
            Device = device;
            Geohash = geohash;
            Date = date;
            IsHome = isHome;
        }

        public string User { get; }

        public string Device { get; }

        public DateTimeOffset Date { get; }

        public string Geohash { get; }

        public bool IsHome { get; }
    }
}
