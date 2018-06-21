using System;
using System.IO;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LinzLinienAlexaSkill.Alexa
{
    public class AlexaSkillMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHostingEnvironment environment;
        private readonly SkillRequestHandler skillRequestHandler;

        public AlexaSkillMiddleware(RequestDelegate next, IHostingEnvironment environment, SkillRequestHandler skillRequestHandler)
        {
            this.next = next;
            this.environment = environment;
            this.skillRequestHandler = skillRequestHandler;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            string bodyStr;
            using (var reader = new StreamReader(context.Request.Body))
            {
                bodyStr = reader.ReadToEnd();
            }

            if (environment.IsProduction())
            {
                // Verify SignatureCertChainUrl is present
                context.Request.Headers.TryGetValue("SignatureCertChainUrl", out var signatureChainUrl);
                if (String.IsNullOrWhiteSpace(signatureChainUrl))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                Uri certUrl;
                try
                {
                    certUrl = new Uri(signatureChainUrl);
                }
                catch
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                // Verify SignatureCertChainUrl is Signature
                context.Request.Headers.TryGetValue("Signature", out var signature);
                if (String.IsNullOrWhiteSpace(signature))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                if (String.IsNullOrWhiteSpace(bodyStr))
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
                var valid = await RequestVerification.Verify(signature, certUrl, bodyStr);
                if (!valid)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
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