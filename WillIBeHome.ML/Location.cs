namespace WillIBeHome.ML
{
    using System;

    public class Location
    {
        public Location(string user, string device, string geohash, DateTimeOffset date, bool isHome)
        {
            this.User = user;
            this.Device = device;
            this.Geohash = geohash;
            this.Date = date;
            this.IsHome = isHome;
        }

        public string User { get; }

        public string Device { get; }

        public DateTimeOffset Date { get; }

        public string Geohash { get; }

        public bool IsHome { get; }
    }
}
