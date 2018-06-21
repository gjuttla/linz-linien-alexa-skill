using System.IO;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LinzLinienAlexaSkill.Alexa
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
            string bodyStr;
            using (var reader = new StreamReader(context.Request.Body))
            {
                bodyStr = reader.ReadToEnd();
            } 
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(bodyStr);
            if (skillRequest == null)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            var skillResponse = await skillRequestHandler.HandleRequestAsync(skillRequest);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(skillResponse));
        }
    }
}