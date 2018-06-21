namespace LinzLinienAlexaSkill.Configuration
{
    public interface IAppConfig
    {
        string EfaApiBaseUrl { get; }
        string StopsEndpoint { get; }
        string DeparturesEndpoint { get; }
        string CacheConnectionStr { get; }
    }
}