using Alexa.NET.Security.Middleware;
using Microsoft.AspNetCore.Builder;

namespace LinzLinienAlexaSkill.Alexa
{
    public static class AlexaSkillRequestVerificationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAlexaSkillRequestVerificationMiddleware(this IApplicationBuilder builder, string path)
        {
            return builder.Map(path, b => b.UseMiddleware<AlexaRequestValidationMiddleware>());
        }
    }
}