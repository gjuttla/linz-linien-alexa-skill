using System.Threading.Tasks;
using LinzLinienAlexaSkill.Web.Alexa;
using LinzLinienAlexaSkill.Web.Utility;
using LinzLinienEfa.Service.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace LinzLinienAlexaSkill.Web
{
    public class AlexaSkillMiddleware
    {
        private readonly RequestDelegate next;

        public AlexaSkillMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        
        public async Task InvokeAsync(HttpContext context, ILogger<AlexaSkillMiddleware> logger, IDeparturesService departuresService, IStopsService stopsService, ILogger<LinzLinienEfaSpeechlet> speechletLogger)
        {
            var speechlet = new LinzLinienEfaSpeechlet(speechletLogger, departuresService, stopsService);
            var response = await speechlet.GetResponseAsync(context.Request.ToHttpRequestMessage());
            // TODO: Set correct values for non-200 reponses
            await context.Response.FromHttpResponseMessage(response);
        }
    }
}