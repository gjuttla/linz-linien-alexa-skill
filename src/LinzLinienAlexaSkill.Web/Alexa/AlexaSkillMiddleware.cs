using System.IO;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LinzLinienAlexaSkill.Web.Alexa
{
    public class AlexaSkillMiddleware
    {
        private readonly RequestDelegate next;

        public AlexaSkillMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        
        public async Task InvokeAsync(HttpContext context, ILogger<AlexaSkillMiddleware> logger, SkillRequestHandler skillRequestHandler)
        {
            logger.LogTrace($"Converting HTTP request body to {nameof(SkillRequest)}");
            string bodyStr;
            using (var reader = new StreamReader(context.Request.Body))
            {
                bodyStr = reader.ReadToEnd();
            } 
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(bodyStr);
            logger.LogTrace($"Converted HTTP request body to {nameof(SkillRequest)}");
            
            logger.LogTrace($"Passing {nameof(SkillRequest)} to {nameof(SkillRequestHandler)}");
            var skillResponse = await skillRequestHandler.HandleRequestAsync(skillRequest);
            logger.LogTrace($"Got {nameof(SkillResponse)} from {nameof(SkillRequestHandler)}");
            
            logger.LogTrace($"Serializing {nameof(SkillResponse)} to JSON");
            await context.Response.WriteAsync(JsonConvert.SerializeObject(skillResponse));
            logger.LogTrace($"Wrote {nameof(SkillResponse)} as JSON in HTTP response");
        }
    }
}