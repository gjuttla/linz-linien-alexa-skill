using Alexa.NET.Response;

namespace LinzLinienAlexaSkill.Web.Alexa
{
    public static class SkillResponseUtil
    {
        public static SkillResponse CreatePlainTextResponse(string text, bool shouldEndSession = true)
        {
            return new SkillResponse()
            {
                Response = new ResponseBody()
                {
                    OutputSpeech = new PlainTextOutputSpeech()
                    {
                        Text = text
                    },
                    ShouldEndSession = shouldEndSession
                },
                Version = "1.0"
            };
        }
    }
}