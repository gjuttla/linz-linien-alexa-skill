﻿using Microsoft.AspNetCore.Builder;

namespace LinzLinienAlexaSkill.Alexa
{
    public static class AlexaSkillMiddlewareExtensions
    {
        public static IApplicationBuilder UseAlexaSkillMiddleware(this IApplicationBuilder builder, string path)
        {
            return builder.Map(path, b => b.UseMiddleware<AlexaSkillMiddleware>());
        }
    }
}