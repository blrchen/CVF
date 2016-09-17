using System.Runtime.Serialization;

namespace CVF.App.Models
{
    [DataContract]
    public class PluginCertificate
    {
        [DataMember]
        public string Thumbnail { get; set; }
    }
}
