using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AlexaSkillsKit.Authentication;
using AlexaSkillsKit.Json;
using AlexaSkillsKit.Slu;
using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;
using LinzLinienAlexaSkill.Web.Extensions;
using LinzLinienEfa.Domain;
using LinzLinienEfa.Service.Common;
using Microsoft.Extensions.Logging;

namespace LinzLinienAlexaSkill.Web.Alexa
{
    public class LinzLinienEfaSpeechlet : SpeechletBase, ISpeechletWithContextAsync
    {
        // Default limit according to EFA API documentation.
        private const uint GetDeparturesForStopDefaultLimit = 40u;
        // Default number of departures for Alexa speech response.
        private const uint NumberOfDepartures = 3u;
        
        private readonly ILogger<LinzLinienEfaSpeechlet> logger;
        private readonly IDeparturesService departuresService;
        private readonly IStopsService stopsService;

        public LinzLinienEfaSpeechlet(ILogger<LinzLinienEfaSpeechlet> logger, IDeparturesService departuresService, IStopsService stopsService)
        {
            this.logger = logger;
            this.departuresService = departuresService;
            this.stopsService = stopsService;
        }

        public async Task<SpeechletResponse> OnIntentAsync(IntentRequest intentRequest, Session session, Context context)
        {
            logger.LogDebug($"OnIntentAsync requestId={intentRequest.RequestId}, sessionId={session.SessionId}");
            var intent = intentRequest.Intent;
            switch (intent.Name)
            {
                case "NextLineDepartureFromStop":
                    return await GetNextDepartureByLineAsync(intent);

                case "NextTramDepartureFromStop":
                    return await GetNextDepartureByTypeAsync(intent, TransportationMean.Tram);

                case "NextBusDepartureFromStop":
                    return await GetNextDepartureByTypeAsync(intent, TransportationMean.Bus);

                case "NextDeparturesFromStop":
                    return await GetNextDeparturesFromStopAsync(intent, NumberOfDepartures);

                default:
                    logger.LogError("OnIntentAsync: Invalid Intent");
                    throw new SpeechletException("Invalid Intent");
            }
        }

        public Task<SpeechletResponse> OnLaunchAsync(LaunchRequest launchRequest, Session session, Context context)
        {
            return Task.Run(() =>
            {
                logger.LogDebug($"OnLaunchAsync requestId={launchRequest.RequestId}, sessionId={session.SessionId}");
                return CreateSpeechletResponse(Responses.WelcomeText, shouldEndSession: false);
            });
        }

        public Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session, Context context)
        {
            return Task.Run(() => logger.LogDebug($"OnSessionStartedAsync requestId={sessionStartedRequest.RequestId}, sessionId={session.SessionId}"));
        }

        public Task OnSessionEndedAsync(SessionEndedRequest sessionEndedRequest, Session session, Context context)
        {
            return Task.Run(() => logger.LogDebug($"OnSessionEndedAsync requestId={sessionEndedRequest.RequestId}, sessionId={session.SessionId}"));
        }

        public override bool OnRequestValidation(SpeechletRequestValidationResult result,
            DateTime referenceTimeUtc, SpeechletRequestEnvelope requestEnvelope)
        {
            if (result != SpeechletRequestValidationResult.OK)
            {
                if (result.HasFlag(SpeechletRequestValidationResult.NoSignatureHeader))
                {
                    logger.LogWarning("Alexa request is missing signature header, but continue processing.");
                    return true;
                }
                if (result.HasFlag(SpeechletRequestValidationResult.NoCertHeader))
                {
                    logger.LogWarning("Alexa request is missing certificate header, but continue processing.");
                    return true;
                }
                if (result.HasFlag(SpeechletRequestValidationResult.InvalidSignature))
                {
                    logger.LogWarning("Alexa request signature is invalid, but continue processing.");
                    return true;
                }
                if (result.HasFlag(SpeechletRequestValidationResult.InvalidTimestamp))
                {
                    var diff = referenceTimeUtc - requestEnvelope.Request.Timestamp;
                    logger.LogWarning($"Alexa request timestamped '{diff.TotalSeconds:0:0.00}' seconds ago making timestamp invalid, but continue processing.");
                }
                return true;
            }
            else
            {
                var diff = referenceTimeUtc - requestEnvelope.Request.Timestamp;
                logger.LogDebug("Alexa request timestamped '{0:0.00}' seconds ago.", diff.TotalSeconds);
                return true;
            }
        }

        private async Task<SpeechletResponse> GetNextDepartureByLineAsync(Intent intent)
        {
            logger.LogTrace($"Enter {nameof(GetNextDepartureByLineAsync)}");
            var lineNr = intent.Slots["lineNr"].Value;
            var originStopName = intent.Slots["originStopName"].Value.ToLower();
            var finalDestinationStopName = intent.Slots["finalDestinationStopName"].Value.ToLower();
            logger.LogDebug($"NextLineDepartureFromStop: lineNr={lineNr}, originStopName={originStopName}, finalDestinationStopName={finalDestinationStopName}");
            var originStops = await stopsService.FindStopsByNameAsync(originStopName);
            if (originStops.Count > 0)
            {
                var originStop = originStops.First();
                var departures = await departuresService.GetDeparturesForStopAsync(
                                                            originStop.Id, 
                                                            GetDeparturesForStopDefaultLimit);
                if (departures.Count > 0)
                {
                    var filteredDepartures = departures
                        .FilterByFinalDestination(finalDestinationStopName)
                        .FilterByLine(lineNr)
                        .ToList();
                    if (filteredDepartures.Count > 0)
                    {
                        logger.LogTrace($"Exit {nameof(GetNextDepartureByLineAsync)} (departure found)");
                        return CreateSpeechletResponse(Responses.NextDepartureText(originStop, filteredDepartures.First()));
                    }
                    logger.LogTrace($"Exit {nameof(GetNextDepartureByLineAsync)} (no departures found)");
                    return CreateSpeechletResponse(Responses.NoDepartuesFoundForCriteriaText);
                }
            }
            logger.LogTrace($"Exit {nameof(GetNextDepartureByLineAsync)} (stop not found)");
            return CreateSpeechletResponse(Responses.StopNotFoundText(originStopName));
        }

        private async Task<SpeechletResponse> GetNextDepartureByTypeAsync(Intent intent, TransportationMean type)
        {
            logger.LogTrace($"Enter {nameof(GetNextDepartureByTypeAsync)}");
            var originStopName = intent.Slots["originStopName"].Value.ToLower();
            var finalDestinationStopName = intent.Slots["finalDestinationStopName"].Value.ToLower();
            logger.LogDebug($"Next{type}DepartureFromStop: originStopName={originStopName}, finalDestinationStopName={finalDestinationStopName}");
            var originStops = await stopsService.FindStopsByNameAsync(originStopName);
            if (originStops.Count > 0)
            {
                var originStop = originStops.First();
                var departures = await departuresService.GetDeparturesForStopAsync(
                                                            originStop.Id, 
                                                            GetDeparturesForStopDefaultLimit);
                if (departures.Count > 0)
                {
                    var filteredDepartures = departures
                        .FilterByFinalDestination(finalDestinationStopName)
                        .FilterByType(type)
                        .ToList();
                    if (filteredDepartures.Count > 0)
                    {
                        logger.LogTrace($"Exit {nameof(GetNextDepartureByTypeAsync)} (departure found)");
                        return CreateSpeechletResponse(Responses.NextDepartureText(originStop, filteredDepartures.First()));
                    }
                    logger.LogTrace($"Exit {nameof(GetNextDepartureByTypeAsync)} (no departures found)");
                    return CreateSpeechletResponse(Responses.NoDepartuesFoundForCriteriaText);
                }
            }
            logger.LogTrace($"Exit {nameof(GetNextDepartureByTypeAsync)} (stop not found)");
            return CreateSpeechletResponse(Responses.StopNotFoundText(originStopName));
        }

        private async Task<SpeechletResponse> GetNextDeparturesFromStopAsync(Intent intent, uint count)
        {
            logger.LogTrace($"Enter {nameof(GetNextDeparturesFromStopAsync)}");
            var originStopName = intent.Slots["originStopName"].Value.ToLower();
            logger.LogDebug($"NextDeparturesFromStop: originStopName={originStopName}");
            var originStops = await stopsService.FindStopsByNameAsync(originStopName);
            if (originStops.Count > 0)
            {
                var originStop = originStops.First();
                var departures = (await departuresService.GetDeparturesForStopAsync(
                                                            originStop, 
                                                            GetDeparturesForStopDefaultLimit)) as List<Departure>;
                if (departures.Count > 0 && departures.Count >= count)
                {
                    var response = $"Hier sind die nächsten {count} Abfahrten von {originStop.Name}.";
                    for (var i = 0; i < count; ++i)
                    {
                        response = $"{response} {Responses.DepartureText(departures[i])}";
                    }
                    logger.LogTrace($"Exit {nameof(GetNextDeparturesFromStopAsync)} (departures found)");
                    return CreateSpeechletResponse(response);
                }
                logger.LogTrace($"Exit {nameof(GetNextDeparturesFromStopAsync)} (no departures found)");
                return CreateSpeechletResponse(Responses.NoDepartuesFoundForCriteriaText);
            }
            logger.LogTrace($"Exit {nameof(GetNextDeparturesFromStopAsync)} (stop not found)");
            return CreateSpeechletResponse(Responses.StopNotFoundText(originStopName));
        }

        private SpeechletResponse CreateSpeechletResponse(string output, bool shouldEndSession = true)
        {
            return new SpeechletResponse
            {
                ShouldEndSession = shouldEndSession,
                OutputSpeech = new PlainTextOutputSpeech { Text = output }
            };
        }
    }
}