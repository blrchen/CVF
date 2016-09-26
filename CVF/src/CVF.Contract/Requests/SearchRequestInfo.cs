using System.Collections.Generic;

namespace CVF.Contract.Requests
{
    public class SearchRequestInfo : PluginRequestInfo
    {
        public SearchRequestInfo(PluginRequestMethod method, List<PluginRequestParameter> parameters)
            : base(method, parameters)
        {
        }

        public int PageSize
        {
            get
            {
                return this.GetParameterValue<int>("pageSize", 10);
            }
        }

        public int Page
        {
            get
            {
                return this.GetParameterValue<int>("page", 1);
            }
        }

        public string Keyword
        {
            get
            {
                return this.GetParameterValue<string>("keyword", string.Empty);
            }
        }

        public T GetRawData<T>()
        {
            return this.GetParameterValue<T>("raw");
        }
    }

    public class SearchRequestInfo<TRaw> : SearchRequestInfo
    {
        public SearchRequestInfo(PluginRequestMethod method, List<PluginRequestParameter> parameters)
            : base(method, parameters)
        {
        }

        public TRaw RawData
        {
            get
            {
                return this.GetRawData<TRaw>();
            }
        }
    }
}
