using System.Collections.Generic;
using System.Threading.Tasks;
using LinzLinienAlexaSkill.Configuration;
using LinzLinienEfa.Domain;
using LinzLinienEfa.Service.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinzLinienAlexaSkill.Dao
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
        
        public async Task<ICollection<Departure>> GetDeparturesForStopAsync(Stop stop, uint limit)
        {
            return await GetDeparturesForStopAsync(stop.Id, limit);
        }

        public async Task<ICollection<Departure>> GetDeparturesForStopAsync(string stopId, uint limit)
        {
            return await GetJsonListAsync<Departure>($"{appConfig.EfaApiBaseUrl}{appConfig.DeparturesEndpoint}{stopId}");
        }
    }
}