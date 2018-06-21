using System.IO;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LinzLinienAlexaSkill.Alexa
{
    public class AlexaSkillMiddleware
    {
        private readonly RequestDelegate next;
        private readonly SkillRequestHandler skillRequestHandler;

        public AlexaSkillMiddleware(RequestDelegate next, SkillRequestHandler skillRequestHandler)
        {
            this.next = next;
            this.skillRequestHandler = skillRequestHandler;
        }
        
        public async Task InvokeAsync(HttpContext context)
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