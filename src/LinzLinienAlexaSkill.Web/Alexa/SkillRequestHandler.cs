using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using LinzLinienAlexaSkill.Web.Extensions;
using LinzLinienEfa.Domain;
using LinzLinienEfa.Service.Common;
using Microsoft.Extensions.Logging;

namespace LinzLinienAlexaSkill.Web.Alexa
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
                return CreateLaunchRequestResponse();
            }
            if (skillRequest.GetRequestType() == typeof(IntentRequest))
            {
                return await CreateIntentRequestResponseAsync(skillRequest.Request as IntentRequest);
            }
            return CreateErrorResponse();
        }

        #region LaunchRequest

        private SkillResponse CreateLaunchRequestResponse()
        {
            return SkillResponseUtil.CreatePlainTextResponse(Responses.WelcomeText);
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
            return CreateErrorResponse();
        }

        private async Task<SkillResponse> CreateResponseForDepartureByLineRequestAsync(IntentRequest intentRequest)
        {
            var originStopName = intentRequest.Intent.Slots["originStopName"].Value.ToLower();
            var originStop = await FindStopByNameAsync(originStopName);
            if (originStop == null)
            {
                return CreateStopNotFoundResponse(originStopName);
            }
            
            var lineNr = intentRequest.Intent.Slots["lineNr"].Value;
            var finalDestinationStopName = intentRequest.Intent.Slots["finalDestinationStopName"].Value.ToLower();
            
            var departures = await departuresService.GetDeparturesForStopAsync(originStop, GetDeparturesForStopDefaultLimit);
            if (departures.Count <= 0)
            {
                return SkillResponseUtil.CreatePlainTextResponse(Responses.NoDepartuesFoundForCriteriaText);
            }
            
            var filteredDepartures = departures
                .FilterByFinalDestination(finalDestinationStopName)
                .FilterByLine(lineNr)
                .ToList();
            if (filteredDepartures.Count > 0)
            {
                return SkillResponseUtil.CreatePlainTextResponse(Responses.NextDepartureText(originStop, filteredDepartures[0]));
            }
            return SkillResponseUtil.CreatePlainTextResponse(Responses.NoDepartuesFoundForCriteriaText);
        }
        
        private async Task<SkillResponse> CreateResponseForDepartureByTypeRequestAsync(IntentRequest intentRequest, TransportationMean type)
        {
            var originStopName = intentRequest.Intent.Slots["originStopName"].Value.ToLower();
            var originStop = await FindStopByNameAsync(originStopName);
            if (originStop == null)
            {
                return CreateStopNotFoundResponse(originStopName);
            }
            
            var finalDestinationStopName = intentRequest.Intent.Slots["finalDestinationStopName"].Value.ToLower();
            
            var departures = await departuresService.GetDeparturesForStopAsync(originStop, GetDeparturesForStopDefaultLimit);
            if (departures.Count <= 0)
            {
                return SkillResponseUtil.CreatePlainTextResponse(Responses.NoDepartuesFoundForCriteriaText);
            }
            
            var filteredDepartures = departures
                .FilterByFinalDestination(finalDestinationStopName)
                .FilterByType(type)
                .ToList();
            if (filteredDepartures.Count > 0)
            {
                return SkillResponseUtil.CreatePlainTextResponse(Responses.NextDepartureText(originStop, filteredDepartures[0]));
            }
            return SkillResponseUtil.CreatePlainTextResponse(Responses.NoDepartuesFoundForCriteriaText);
        }
        
        private async Task<SkillResponse> CreateResponseForDeparturesFromStopRequestAsync(IntentRequest intentRequest, uint count)
        {
            var originStopName = intentRequest.Intent.Slots["originStopName"].Value.ToLower();
            var originStop = await FindStopByNameAsync(originStopName);
            if (originStop == null)
            {
                return CreateStopNotFoundResponse(originStopName);
            }
            
            var departures = (await departuresService.GetDeparturesForStopAsync(originStop, GetDeparturesForStopDefaultLimit)) as List<Departure>;
            if (departures.Count > 0 && departures.Count >= count)
            {
                var response = $"Hier sind die nächsten {count} Abfahrten von {originStop.Name}.";
                for (var i = 0; i < count; ++i)
                {
                    response = $"{response} {Responses.DepartureText(departures[i])}";
                }
                return SkillResponseUtil.CreatePlainTextResponse(response);
            }
            return SkillResponseUtil.CreatePlainTextResponse(Responses.NoDepartuesFoundForCriteriaText);
        }

        #endregion

        #endregion

        #region Error Responses

        private SkillResponse CreateErrorResponse()
        {
            return SkillResponseUtil.CreatePlainTextResponse(Responses.ErrorText);
        }

        private SkillResponse CreateStopNotFoundResponse(string stopName)
        {
            return SkillResponseUtil.CreatePlainTextResponse(Responses.StopNotFoundText(stopName));
        }
        
        #endregion

        #region Helper method FindStopByNameAsync

        private async Task<Stop> FindStopByNameAsync(string name)
        {
            var stops = await stopsService.FindStopsByNameAsync(name);
            return stops.Count > 0 ? stops.First() : null;
        }

        #endregion
    }
}