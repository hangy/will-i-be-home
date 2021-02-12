using System.Collections.Generic;
using System.Linq;

namespace WillIBeHome.ML
{
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
            for (int i = 0; i < locations.Count; ++i)
            {
                Location? location = locations[i];
                if (location.IsHome)
                {
                    IEnumerable<Location>? sequenceItemsInRange = sequence.Where(l => (location.Date - l.Date).TotalMinutes <= maxMinutesBeforeArrivalAtHome);
                    IEnumerable<Location>? sequenceItemsOutOfRange = sequence.Except(sequenceItemsInRange);
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

        private static IEnumerable<Transition> ConvertSequenceToTransitions(IEnumerable<Location> locationSequence, Location? homeLocation = default)
        {
            var result = new List<Transition>();

            bool willBeHome = homeLocation != null;

            Location? lastLocation = default;
            foreach (Location? location in locationSequence)
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
            else
            {
                if (homeLocation != null)
                {
                    result.Add(new Transition(homeLocation, homeLocation, willBeHome));
                }

                if (lastLocation != null)
                {
                    result.Add(new Transition(lastLocation, lastLocation, willBeHome));
                }
            }

            return result;
        }
    }
}
