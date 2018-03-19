using System.Collections.Generic;
using System.Linq;
using LinzLinienEfa.Domain;

namespace LinzLinienAlexaSkill.Web.Extensions
{
    internal static class DepartureFiltering
    {
        internal static IEnumerable<Departure> FilterByLine(this IEnumerable<Departure> departures, string line)
        {
            return from departure in departures where departure.Line.Number.Contains(line) select departure;
        }

        internal static IEnumerable<Departure> FilterByType(this IEnumerable<Departure> departures, TransportationMean type)
        {
            return from departure in departures where departure.Line.Type == type select departure;
        }

        internal static IEnumerable<Departure> FilterByFinalDestination(this IEnumerable<Departure> departures, string stopName)
        {
            return from departure in departures where departure.Line.Direction.ToLower().Contains(stopName) select departure;
        }
    }
}