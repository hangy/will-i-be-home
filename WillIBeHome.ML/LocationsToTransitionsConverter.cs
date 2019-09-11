namespace WillIBeHome.ML
{
    using System.Collections.Generic;
    using System.Linq;

    public static class LocationsToTransitionsConverter
    {
        public static IEnumerable<Transition> Convert(IEnumerable<Location> locations)
        {
            if (locations == null)
            {
                return Enumerable.Empty<Transition>();
            }

            var result = new List<Transition>();
            foreach (var g in locations.GroupBy(l => new { l.User, l.Device }))
            {
                result.AddRange(ConvertForSingleDevice(g.ToList()));
            }

            return result;
        }

        private static IEnumerable<Transition> ConvertForSingleDevice(IReadOnlyList<Location> locations)
        {
            var result = new List<Transition>(locations.Count * 3);
            const double maxMinutesBeforeArrivalAtHome = 5.0;
            var sequence = new List<Location>();
            for (var i = 0; i < locations.Count; ++i)
            {
                var location = locations[i];
                if (location.IsHome)
                {
                    var sequenceItemsInRange = sequence.Where(l => (location.Date - l.Date).TotalMinutes <= maxMinutesBeforeArrivalAtHome);
                    var sequenceItemsOutOfRange = sequence.Except(sequenceItemsInRange);
                    result.AddRange(ConvertSequenceToTransitions(sequenceItemsOutOfRange));
                    result.AddRange(ConvertSequenceToTransitions(sequenceItemsInRange, location));
                    sequence.Clear();
                }
                else
                {
                    sequence.Add(location);
                }
            }

            result.AddRange(ConvertSequenceToTransitions(sequence));
            sequence.Clear();

            return result;
        }

        private static IEnumerable<Transition> ConvertSequenceToTransitions(IEnumerable<Location> sequenceItemsInRange, Location? homeLocation = default)
        {
            var result = new List<Transition>();

            var willBeHome = homeLocation != null;

            Location? lastLocation = default;
            foreach (var location in sequenceItemsInRange)
            {
                if (lastLocation != null)
                {
                    result.Add(new Transition(lastLocation, location, willBeHome));
                }

                lastLocation = location;
            }

            if (lastLocation != null && homeLocation != null)
            {
                result.Add(new Transition(lastLocation, homeLocation, willBeHome));
            }
            else if (homeLocation != null)
            {
                result.Add(new Transition(homeLocation, homeLocation, willBeHome));
            }

            return result;
        }
    }
}
