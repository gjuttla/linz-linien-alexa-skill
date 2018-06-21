namespace LinzLinienAlexaSkill.Configuration
{
    public class AppConfig : IAppConfig
    {
        public string EfaApiBaseUrl { get; set; }
        public string StopsEndpoint { get; set; }
        public string DeparturesEndpoint { get; set; }
        public string CacheConnectionStr { get; set; }
    }
}