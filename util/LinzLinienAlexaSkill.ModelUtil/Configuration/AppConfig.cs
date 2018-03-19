using System.Collections.Generic;

namespace LinzLinienAlexaSkill.ModelUtil.Configuration
{
    public class AppConfig : LinzLinienAlexaSkill.Web.Configuration.AppConfig, IAppConfig
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public IDictionary<string, ICollection<string>> StopNameSynonyms { get; set; }
    }
}