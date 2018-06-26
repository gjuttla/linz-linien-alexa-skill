using LinzLinienEfa.Domain;

namespace LinzLinienAlexaSkill.Alexa
{
    public static class Responses
    {
        public static readonly string WelcomeText = "Hallo, frag mich nach den nächsten Abfahrten.";
        public static readonly string ErrorText = "Bei der Anfrage ist ein Fehler aufgetreten.";
        public static readonly string NoDepartuesFoundForCriteriaText = "Ich habe zu diesen Kriterien leider keine Abfahrten gefunden.";

        public static string NoSlotValueFor(string slotUsage)
        {
            return $"Ungültiger Wert für {slotUsage}.";
        }
        
        public static string StopNotFoundText(string stopName)
        {
            return $"Ich habe die Haltestelle {stopName} leider nicht gefunden.";
        }

        public static string NextDepartureText(Stop originStop, Departure departure)
        {
            var ending = $" Linie {departure.Line.Number} von {originStop.Name} nach {departure.Line.Direction} fährt in {departure.CountdownInMinutes} Minuten ab.";
            switch (departure.Line.Type)
            {
                case TransportationMean.Bus:
                    return $"Der nächste Bus{ending}";
                case TransportationMean.Tram:
                    return $"Die nächste Straßenbahn{ending}";
                default:
                    return Responses.ErrorText;
            }
        }

        public static string DepartureText(Departure departure)
        {
            var ending = $" Linie {departure.Line.Number} nach {departure.Line.Direction} fährt in {departure.CountdownInMinutes} Minuten ab.";
            switch (departure.Line.Type)
            {
                case TransportationMean.Bus:
                    return $"Der Bus{ending}";
                case TransportationMean.Tram:
                    return $"Die Straßenbahn{ending}";
                default:
                    return Responses.ErrorText;
            }
        }
    }
}