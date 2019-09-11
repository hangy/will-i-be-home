namespace WillIBeHome.ML
{
    using System;

    public class Transition
    {
        private readonly Location from;

        private readonly Location to;

        public Transition(Location from, Location to, bool willBeHome)
        {
            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            this.from = from;
            this.to = to;

            this.Label = willBeHome;
        }

        public string User => from.User;

        public string Device => from.Device;

        public string FromGeohash => from.Geohash;

        public int FromDayOfWeek => (int)from.Date.DayOfWeek;

        public int FromHours => from.Date.Hour;

        public string ToGeohash => to.Geohash;

        public int ToDayOfWeek => (int)to.Date.DayOfWeek;

        public int ToHours => to.Date.Hour;

        public bool Label { get; }
    }
}
