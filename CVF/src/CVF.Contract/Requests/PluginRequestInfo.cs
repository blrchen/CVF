using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CVF.Contract.Requests
{
    public class PluginRequestInfo
    {
        public PluginRequestInfo(PluginRequestMethod method, List<PluginRequestParameter> parameters)
        {
            this.Method = method;
            this.Parameters = parameters;
        }

        public PluginRequestMethod Method { get; private set; }

        public List<PluginRequestParameter> Parameters { get; private set; }

        public string GetParameterStringValue(string name)
        {
            if (this.Parameters.Any(p => p.Name == name))
            {
                return this.Parameters.First(p => p.Name == name).Value;
            }

            return null;
        }

        public T GetParameterValue<T>(string name)
        {
            return this.GetParameterValue<T>(name, default(T));
        }

        public T GetParameterValue<T>(string name, T defaultValue)
        {
            try
            {
                var type = typeof(T);

                var value = this.GetParameterStringValue(name);
                if (value == null)
                {
                    return defaultValue;
                }

                if (type.IsValueType)
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                else if (type == typeof(string))
                {
                    return (T)(object)value;
                }
                else
                {
                    // this is a self define type.
                    return (T)JsonConvert.DeserializeObject(value, type);
                }
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}
