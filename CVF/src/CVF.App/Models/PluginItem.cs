using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CVF.App.Models
{
    [DataContract]
    public class PluginItem : PluginBase
    {
        private string controller;

        [DataMember(Name = "route")]
        public string Route { get; set; }

        [DataMember(Name = "view")]
        public string View { get; set; }

        [DataMember(Name = "type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public PluginItemType Type { get; set; }

        [DataMember(Name = "refreshInterval")]
        public int RefreshInterval { get; set; }

        [DataMember(Name = "controller")]
        public string Controller
        {
            get
            {
                if (string.IsNullOrEmpty(this.controller))
                {
                    return this.Type.ToString().ToLowerInvariant() + "Controller";
                }

                return this.controller;
            }
            set
            {
                this.controller = value;
            }
        }
    }
}
