using System.Collections.Generic;
using System.Threading.Tasks;
using LinzLinienAlexaSkill.Web.Configuration;
using LinzLinienEfa.Domain;
using LinzLinienEfa.Service.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LinzLinienAlexaSkill.Web.Dao
{
    public class DeparturesDao : AbstractDao, IDeparturesService
    {
        private readonly ILogger<DeparturesDao> logger;
        private readonly IAppConfig appConfig;
        
        public DeparturesDao(ILogger<DeparturesDao> logger, IOptions<AppConfig> options)
        {
            this.logger = logger;
            this.appConfig = options.Value;
        }
        
        public Task<ICollection<Departure>> GetDeparturesForStopAsync(Stop stop, uint limit)
        {
            return GetDeparturesForStopAsync(stop.Id, limit);
        }

        public async Task<ICollection<Departure>> GetDeparturesForStopAsync(string stopId, uint limit)
        {
            return JsonConvert.DeserializeObject<List<Departure>>(
                await HttpClient.GetStringAsync($"{appConfig.EfaApiBaseUrl}{appConfig.DeparturesEndpoint}{stopId}"), 
                JsonSerializerSettings);
        }
    }
}