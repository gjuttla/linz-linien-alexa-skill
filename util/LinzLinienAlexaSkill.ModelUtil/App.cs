using System.Threading.Tasks;
using LinzLinienAlexaSkill.ModelUtil.Services;
using Microsoft.Extensions.Logging;

namespace LinzLinienAlexaSkill.ModelUtil
{
    public class App
    {
        private readonly ILogger<App> logger;
        private readonly IAlexaModelCreationService alexaModelCreationService;

        public App(ILogger<App> logger, IAlexaModelCreationService alexaModelCreationService)
        {
            this.logger = logger;
            this.alexaModelCreationService = alexaModelCreationService;
        }
        
        public async Task RunAsync()
        {
            logger.LogTrace("Executing App.RunAsync");
            await alexaModelCreationService.CreateAlexaInvocationModelAsync();
            logger.LogTrace("Executed App.RunAsync");
        }
    }
}