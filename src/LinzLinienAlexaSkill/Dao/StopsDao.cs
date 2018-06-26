using System.Collections.Generic;
using System.Threading.Tasks;
using LinzLinienAlexaSkill.Configuration;
using LinzLinienEfa.Domain;
using LinzLinienEfa.Service.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LinzLinienAlexaSkill.Dao
{
    public class StopsDao : AbstractDao, IStopsService
    {
        private readonly ILogger<StopsDao> logger;
        private readonly IAppConfig appConfig;

        public StopsDao(ILogger<StopsDao> logger, IOptions<AppConfig> options)
        {
            this.logger = logger;
            this.appConfig = options.Value;
        }
        
        public async Task<ICollection<Stop>> FindStopsByNameAsync(string name)
        {
            return await GetJsonListAsync<Stop>($"{appConfig.EfaApiBaseUrl}{appConfig.StopsEndpoint}{name}");
        }
    }
}