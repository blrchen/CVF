using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CVF.App.Models;
using Newtonsoft.Json;

namespace CVF.App.Manager
{
    public class PluginClient
    {
        private readonly Plugin plugin;
        private readonly HttpClient client;

        public PluginClient(Plugin plugin)
        {
            this.plugin = plugin;
            this.client = new HttpClient();
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(PluginItem item, TRequest request, CancellationToken token)
        {
            try
            {
                var uri = $"{this.plugin.Url}/{item.Route}";
                using (HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, uri))
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                    var response = await this.client.SendAsync(message, token);
                    response.EnsureSuccessStatusCode();

                    var responseString = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<TResponse>(responseString);
                }
            }
            catch (Exception ex)
            {
                // todo: trace exception.
                throw;
            }
        }
    }
}
