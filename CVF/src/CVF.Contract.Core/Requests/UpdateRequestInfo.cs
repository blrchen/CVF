using System.Collections.Generic;

namespace CVF.Contract.Requests
{
    public class UpdateRequestInfo<TBody> : PluginRequestInfo
    {
        public UpdateRequestInfo(PluginRequestMethod method, List<PluginRequestParameter> parameters)
            : base(method, parameters)
        {
        }

        public TBody Body
        {
            get
            {
                return this.GetParameterValue<TBody>("data");
            }
        }
    }
}
