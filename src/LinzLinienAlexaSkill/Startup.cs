using LinzLinienAlexaSkill.Alexa;
using LinzLinienAlexaSkill.Configuration;
using LinzLinienAlexaSkill.Dao;
using LinzLinienEfa.Service.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LinzLinienAlexaSkill
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.Configure<AppConfig>(Configuration.GetSection("AppConfig"));
            
            // Add logging
            services.AddSingleton(new LoggerFactory()
                .AddConsole(Configuration.GetSection("Logging"))
                .AddDebug());
            services.AddLogging();
            
            // Add services
            services.AddSingleton<IDeparturesService, DeparturesDao>();
            services.AddSingleton<IStopsService, StopsDao>();
            services.AddSingleton<SkillRequestHandler>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAlexaSkillMiddleware("/alexa");
            app.Run(async context => await context.Response.WriteAsync("Hello there!"));
        }
    }
}