using System.Collections.Generic;

namespace LinzLinienAlexaSkill.ModelUtil.Configuration
{
    public interface IAppConfig : LinzLinienAlexaSkill.Web.Configuration.IAppConfig
    {
        string InputFile { get; }
        string OutputFile { get; }
        IDictionary<string, ICollection<string>> StopNameSynonyms { get; }
    }
}