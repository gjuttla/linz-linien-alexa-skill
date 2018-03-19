using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinzLinienAlexaSkill.Web.Dao
{
    public abstract class AbstractDao
    {
        protected static readonly HttpClient HttpClient;
        protected static readonly JsonSerializerSettings JsonSerializerSettings;

        static AbstractDao()
        {
            HttpClient = new HttpClient();
            HttpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
    }
}