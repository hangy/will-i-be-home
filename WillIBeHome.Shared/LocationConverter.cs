namespace WillIBeHome.Shared
{
    using System;
    using System.Linq;

    public static class LocationConverter
    {
        public static ML.Location Convert(string user, string device, Owntracks.Location source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var geohash = source.Geohash ?? string.Empty;
            var isHome = source.InRegions.Contains("Home");

            DateTimeOffset date;
            if (source.UnixEpochTimestamp.HasValue)
            {
                date = DateTimeOffset.FromUnixTimeSeconds(source.UnixEpochTimestamp.Value);
            }
            else
            {
                date = default;
            }

            return new ML.Location(user, device, geohash, date, isHome);
        }
    }
}
