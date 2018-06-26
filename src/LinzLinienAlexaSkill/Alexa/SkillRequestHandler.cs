using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using LinzLinienAlexaSkill.Dao;
using LinzLinienEfa.Domain;
using LinzLinienEfa.Service.Common;
using Microsoft.Extensions.Logging;
using static LinzLinienAlexaSkill.Alexa.SkillResponseUtil;

namespace LinzLinienAlexaSkill.Alexa
{
    public class SkillRequestHandler
    {
        #region Private constants

        // Default limit according to EFA API documentation.
        private const uint GetDeparturesForStopDefaultLimit = 40u;
        // Default number of departures for Alexa speech response.
        private const uint NumberOfDepartures = 3u;

        #endregion

        #region Private readonly fields

        private readonly ILogger<SkillRequestHandler> logger;
        private readonly IDeparturesService departuresService;
        private readonly IStopsService stopsService;

        #endregion

        #region Constructor

        public SkillRequestHandler(ILogger<SkillRequestHandler> logger, IDeparturesService departuresService, IStopsService stopsService)
        {
            this.logger = logger;
            this.departuresService = departuresService;
            this.stopsService = stopsService;
        }

        #endregion
       
        #region Request handler

        public async Task<SkillResponse> HandleRequestAsync(SkillRequest skillRequest)
        {
            if (skillRequest.GetRequestType() == typeof(LaunchRequest))
            {
                logger.LogDebug($"Handling {nameof(LaunchRequest)}");
                return CreateLaunchRequestResponse();
            }
            if (skillRequest.GetRequestType() == typeof(IntentRequest))
            {
                logger.LogDebug($"Handling {nameof(IntentRequest)}");
                return await CreateIntentRequestResponseAsync(skillRequest.Request as IntentRequest);
            }
            logger.LogWarning($"Encountered unknown {nameof(SkillRequest)}");
            return CreateErrorResponse();
        }

        #region LaunchRequest

        private SkillResponse CreateLaunchRequestResponse()
        {
            return CreatePlainTextResponse(Responses.WelcomeText);
        }

        #endregion
        
        #region IntentRequest

        private async Task<SkillResponse> CreateIntentRequestResponseAsync(IntentRequest intentRequest)
        {
            switch (intentRequest.Intent.Name)
            {
                case "NextLineDepartureFromStop":
                    return await CreateResponseForDepartureByLineRequestAsync(intentRequest);

                case "NextTramDepartureFromStop":
                    return await CreateResponseForDepartureByTypeRequestAsync(intentRequest, TransportationMean.Tram);

                case "NextBusDepartureFromStop":
                    return await CreateResponseForDepartureByTypeRequestAsync(intentRequest, TransportationMean.Bus);

                case "NextDeparturesFromStop":
                    return await CreateResponseForDeparturesFromStopRequestAsync(intentRequest, NumberOfDepartures);
            }
            logger.LogWarning($"Encountered unknown {nameof(IntentRequest)}");
            return CreateErrorResponse();
        }

        private async Task<SkillResponse> CreateResponseForDepartureByLineRequestAsync(IntentRequest intentRequest)
        {
            logger.LogTrace($"Enter {nameof(CreateResponseForDepartureByLineRequestAsync)}");
            
            var originStopName = intentRequest.TryGetSlotValue("originStopName");
            if (originStopName == null)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByLineRequestAsync)} (no slot value for {nameof(originStopName)})");
                return CreatePlainTextResponse(Responses.NoSlotValueFor("die Abfahrtshaltestelle"));
            }
            
            var originStop = await FindStopByNameAsync(originStopName);
            if (originStop == null)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByLineRequestAsync)} (stop not found)");
                return CreateStopNotFoundResponse(originStopName);
            }
                       
            var lineNr = intentRequest.TryGetSlotValue("lineNr");
            if (lineNr == null)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByLineRequestAsync)} (no slot value for {nameof(lineNr)})");
                return CreatePlainTextResponse(Responses.NoSlotValueFor("die Liniennummer"));
            }
            
            var finalDestinationStopName = intentRequest.TryGetSlotValue("finalDestinationStopName");
            if (finalDestinationStopName == null)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByLineRequestAsync)} (no slot value for {nameof(finalDestinationStopName)})");
                return CreatePlainTextResponse(Responses.NoSlotValueFor("die Endhaltestelle"));
            }
            
            logger.LogDebug($"NextLineDepartureFromStop: lineNr={lineNr}, originStopName={originStopName}, finalDestinationStopName={finalDestinationStopName}");
            
            var departures = await departuresService.GetDeparturesForStopAsync(originStop, GetDeparturesForStopDefaultLimit);
            if (departures.Count <= 0)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByLineRequestAsync)} (departures.Count <= 0)");
                return CreatePlainTextResponse(Responses.NoDepartuesFoundForCriteriaText);
            }
            
            var filteredDepartures = departures
                .FilterByFinalDestination(finalDestinationStopName)
                .FilterByLine(lineNr)
                .ToList();
            if (filteredDepartures.Count <= 0)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByLineRequestAsync)} (filteredDepartures.Count <= 0)");
                return CreatePlainTextResponse(Responses.NoDepartuesFoundForCriteriaText);
            }
            logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByLineRequestAsync)} [ok]");
            return CreatePlainTextResponse(Responses.NextDepartureText(originStop, filteredDepartures[0]));
        }
        
        private async Task<SkillResponse> CreateResponseForDepartureByTypeRequestAsync(IntentRequest intentRequest, TransportationMean type)
        {
            logger.LogTrace($"Enter {nameof(CreateResponseForDepartureByTypeRequestAsync)}");
            
            var originStopName = intentRequest.TryGetSlotValue("originStopName");
            if (originStopName == null)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByTypeRequestAsync)} (no slot value for {nameof(originStopName)})");
                return CreatePlainTextResponse(Responses.NoSlotValueFor("die Abfahrtshaltestelle"));
            }
            
            var originStop = await FindStopByNameAsync(originStopName);
            if (originStop == null)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByTypeRequestAsync)} (stop not found)");
                return CreateStopNotFoundResponse(originStopName);
            }
            
            var finalDestinationStopName = intentRequest.TryGetSlotValue("finalDestinationStopName");
            if (finalDestinationStopName == null)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByTypeRequestAsync)} (no slot value for {nameof(finalDestinationStopName)})");
                return CreatePlainTextResponse(Responses.NoSlotValueFor("die Endhaltestelle"));
            }
            
            logger.LogDebug($"Next{type}DepartureFromStop: originStopName={originStopName}, finalDestinationStopName={finalDestinationStopName}");
            
            var departures = await departuresService.GetDeparturesForStopAsync(originStop, GetDeparturesForStopDefaultLimit);
            if (departures.Count <= 0)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByTypeRequestAsync)} (departures.Count <= 0)");
                return CreatePlainTextResponse(Responses.NoDepartuesFoundForCriteriaText);
            }
            
            var filteredDepartures = departures
                .FilterByFinalDestination(finalDestinationStopName)
                .FilterByType(type)
                .ToList();
            if (filteredDepartures.Count <= 0)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByTypeRequestAsync)} (filteredDepartures.Count <= 0)");
                return CreatePlainTextResponse(Responses.NoDepartuesFoundForCriteriaText);
            }
            logger.LogTrace($"Exit {nameof(CreateResponseForDepartureByTypeRequestAsync)} [ok]");
            return CreatePlainTextResponse(Responses.NextDepartureText(originStop, filteredDepartures[0]));
        }
        
        private async Task<SkillResponse> CreateResponseForDeparturesFromStopRequestAsync(IntentRequest intentRequest, uint count)
        {
            logger.LogTrace($"Enter {nameof(CreateResponseForDeparturesFromStopRequestAsync)}");
            var originStopName = intentRequest.TryGetSlotValue("originStopName");
            if (originStopName == null)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDeparturesFromStopRequestAsync)} (no slot value for {nameof(originStopName)})");
                return CreatePlainTextResponse(Responses.NoSlotValueFor("die Abfahrtshaltestelle"));
            }
            
            var originStop = await FindStopByNameAsync(originStopName);
            if (originStop == null)
            {
                logger.LogTrace($"Exit {nameof(CreateResponseForDeparturesFromStopRequestAsync)} (stop not found)");
                return CreateStopNotFoundResponse(originStopName);
            }
            logger.LogDebug($"NextDeparturesFromStop: originStopName={originStopName}");
            
            var departures = (await departuresService.GetDeparturesForStopAsync(originStop, GetDeparturesForStopDefaultLimit)) as List<Departure>;
            if (departures.Count > 0 && departures.Count >= count)
            {
                var response = $"Hier sind die nächsten {count} Abfahrten von {originStop.Name}.";
                for (var i = 0; i < count; ++i)
                {
                    response = $"{response} {Responses.DepartureText(departures[i])}";
                }
                logger.LogTrace($"Exit {nameof(CreateResponseForDeparturesFromStopRequestAsync)} [ok]");
                return CreatePlainTextResponse(response);
            }
            logger.LogTrace($"Exit {nameof(CreateResponseForDeparturesFromStopRequestAsync)} (no departures)");
            return CreatePlainTextResponse(Responses.NoDepartuesFoundForCriteriaText);
        }

        #endregion

        #endregion

        #region Error Responses

        private SkillResponse CreateErrorResponse()
        {
            return CreatePlainTextResponse(Responses.ErrorText);
        }

        private SkillResponse CreateStopNotFoundResponse(string stopName)
        {
            return CreatePlainTextResponse(Responses.StopNotFoundText(stopName));
        }
        
        #endregion

        #region Helper method FindStopByNameAsync

        private async Task<Stop> FindStopByNameAsync(string name)
        {
            var stops = await stopsService.FindStopsByNameAsync(name);
            return (stops != null && stops.Count > 0) ? stops.First() : null;
        }

        #endregion
    }
}