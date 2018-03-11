using System.IO;
using System.Threading.Tasks;
using LinzLinienAlexaSkill.ModelUtil.Configuration;
using LinzLinienAlexaSkill.ModelUtil.Services;
using LinzLinienAlexaSkill.Web.Dao;
using LinzLinienEfa.Service.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinzLinienAlexaSkill.ModelUtil
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var app = serviceProvider.GetService<App>();
            await app.RunAsync();
        }
        
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();
            serviceCollection.AddOptions();
            serviceCollection.Configure<AppConfig>(configuration.GetSection("AppConfig"));
            
            // Add logging
            serviceCollection.AddSingleton(new LoggerFactory()
                .AddConsole(configuration.GetSection("Logging"))
                .AddDebug());
            serviceCollection.AddLogging();

            // Get conf. for IStopsService impl. which requires a base AppConfig.
            var sp = serviceCollection.BuildServiceProvider();
            var options = sp.GetService<IOptions<AppConfig>>();
            
            // Add services
            serviceCollection.AddSingleton<IOptions<LinzLinienAlexaSkill.Web.Configuration.AppConfig>>(o => options);
            serviceCollection.AddTransient<IStopsService, StopsDao>();
            serviceCollection.AddTransient<IAlexaModelCreationService, AlexaModelCreationService>();
            
            // Add main app
            serviceCollection.AddSingleton<App>();
        }
    }
}