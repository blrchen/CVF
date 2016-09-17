using System.Runtime.Serialization;

namespace CVF.App.Models
{
    [DataContract]
    public class PluginBase
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
}
