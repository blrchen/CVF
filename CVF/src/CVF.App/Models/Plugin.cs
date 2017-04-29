using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CVF.App.Models
{
    [DataContract]
    public class Plugin : PluginBase
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "DebugUrl")]
        public string DebugUrl { get; set; }

        [DataMember(Name = "scripts")]
        public List<string> Scripts { get; set; }

        [IgnoreDataMember]
        public PluginCertificate Certificate { get; set; }

        [DataMember(Name = "categories")]
        public List<PluginCategory> Categories { get; set; }

        [DataMember(Name = "refreshInterval")]
        public int RefreshInterval { get; set; }
    }
}
