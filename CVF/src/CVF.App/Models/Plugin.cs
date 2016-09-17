using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CVF.App.Models
{
    [DataContract]
    public class Plugin : PluginBase
    {
        [DataMember(Name = "url")]
        public string Url { get; set; }

        [IgnoreDataMember]
        public List<string> Scripts { get; set; }

        [IgnoreDataMember]
        public PluginCertificate Certificate { get; set; }

        [DataMember(Name = "categories")]
        public List<PluginCategory> Categories { get; set; }
    }
}
