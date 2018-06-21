using System.Collections.Generic;
using System.Linq;
using LinzLinienEfa.Domain;

namespace LinzLinienAlexaSkill.Dao
{
    internal static class DeparturesFiltering
    {
        internal static IEnumerable<Departure> FilterByLine(this IEnumerable<Departure> departures, string line)
        {
            return departures.Where(d => d.Line.Number.Contains(line));
        }

        internal static IEnumerable<Departure> FilterByType(this IEnumerable<Departure> departures, TransportationMean type)
        {
            return departures.Where(d => d.Line.Type == type);
        }

        internal static IEnumerable<Departure> FilterByFinalDestination(this IEnumerable<Departure> departures, string stopName)
        {
            return departures.Where(d => d.Line.Direction.ToLower().Contains(stopName));
        }
    }
}