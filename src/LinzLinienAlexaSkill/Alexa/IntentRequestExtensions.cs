using Alexa.NET.Request.Type;

namespace LinzLinienAlexaSkill.Alexa
{
    public static class IntentRequestExtensions
    {
        public static string TryGetSlotValue(this IntentRequest intentRequest, string slotName)
        {
            return intentRequest.Intent.Slots[slotName].Value?.ToLower();
        }
    }
}