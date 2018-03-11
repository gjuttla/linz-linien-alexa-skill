using LinzLinienAlexaSkill.Web.Alexa;
using LinzLinienAlexaSkill.Web.Configuration;
using LinzLinienAlexaSkill.Web.Dao;
using LinzLinienAlexaSkill.Web.Utility;
using LinzLinienEfa.Service.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LinzLinienAlexaSkill.Web
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
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.Run(async (context) =>
            {
                var logger = app.ApplicationServices.GetService<ILogger<LinzLinienEfaSpeechlet>>();
                var departuresService = app.ApplicationServices.GetService<IDeparturesService>();
                var stopsService = app.ApplicationServices.GetService<IStopsService>();
                var speechlet = new LinzLinienEfaSpeechlet(logger, departuresService, stopsService);
                var response = await speechlet.GetResponseAsync(context.Request.ToHttpRequestMessage());
                // TODO: Set correct values for non-200 reponses
                await context.Response.FromHttpResponseMessage(response);
            });
        }
    }
}