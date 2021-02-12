using System;

namespace WillIBeHome.ML
{
    public class Transition
    {
        private readonly Location _from;

        private readonly Location _to;

        public Transition(Location from, Location to, bool willBeHome)
        {
            _from = from ?? throw new ArgumentNullException(nameof(from));
            _to = to ?? throw new ArgumentNullException(nameof(to));

            Label = willBeHome;
        }

        public string User => _from.User;

        public string Device => _from.Device;

        public string FromGeohash => _from.Geohash;

        public int FromDayOfWeek => (int)_from.Date.DayOfWeek;

        public int FromHours => _from.Date.Hour;

        public string ToGeohash => _to.Geohash;

        public int ToDayOfWeek => (int)_to.Date.DayOfWeek;

        public int ToHours => _to.Date.Hour;

        public bool Label { get; }
    }
}
