using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LinzLinienAlexaSkill.Dao
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

        protected static async Task<List<T>> GetJsonListAsync<T>(string url)
        {
            try
            {
                return JsonConvert.DeserializeObject<List<T>>(await HttpClient.GetStringAsync(url), JsonSerializerSettings);
            }
            catch (HttpRequestException e)
            {
                return null;
            }
        }
    }
}