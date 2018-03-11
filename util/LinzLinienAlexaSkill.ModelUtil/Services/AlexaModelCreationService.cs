using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using LinzLinienAlexaSkill.ModelUtil.Configuration;
using LinzLinienEfa.Domain;
using LinzLinienEfa.Service.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace LinzLinienAlexaSkill.ModelUtil.Services
{
    public class AlexaModelCreationService : IAlexaModelCreationService
    {
        private readonly ILogger<AlexaModelCreationService> logger;
        private readonly IAppConfig appConfig;
        private readonly IStopsService stopsService;

        public AlexaModelCreationService(ILogger<AlexaModelCreationService> logger, IOptions<AppConfig> options, IStopsService stopsService)
        {
            this.logger = logger;
            this.appConfig = options.Value;
            this.stopsService = stopsService;
        }
        
        public async Task CreateAlexaInvocationModelAsync()
        {
            logger.LogInformation("Creating Alexa invocation model");
            var allStops = new Dictionary<string, Stop>();
            for (var c = 'a'; c <= 'z'; ++c)
            {
                try
                {
                    var stops = await stopsService.FindStopsByNameAsync(c.ToString());
                    foreach (var stop in stops)
                    {
                        allStops[stop.Id] = stop;
                    }
                }
                catch (HttpRequestException e)
                {
                    logger.LogWarning($"Caught {nameof(HttpRequestException)}. No stops found for '{c}'.");
                }
            }
            var slotValues = new JArray();
            foreach (var stop in allStops.Values)
            {
                slotValues.Add(CreateSlotValue(stop));
            }
            var inputModel = JObject.Parse(File.ReadAllText(appConfig.InputFile));
            inputModel["languageModel"]["types"] = new JObject()
            {
                {"name", "EFA_STOP"},
                {"values", slotValues}
            };
            File.WriteAllText(appConfig.OutputFile, inputModel.ToString());
            File.WriteAllText($"valuesArray_{appConfig.OutputFile}", slotValues.ToString());
        }

        private JObject CreateSlotValue(Stop stop)
        {
            return new JObject()
            {
                {"id", stop.Id},
                {"name", new JObject()
                {
                    {"value", stop.Name},
                    {"synonyms", GetSynonyms(stop)}
                }}
            };
        }

        private JArray GetSynonyms(Stop stop)
        {
            var synonyms = new JArray();
            if (appConfig.StopNameSynonyms.ContainsKey(stop.Name))
            {
                foreach (var synonym in appConfig.StopNameSynonyms[stop.Name])
                {
                    synonyms.Add(synonym);
                }
            }
            return synonyms;
        }
    }
}