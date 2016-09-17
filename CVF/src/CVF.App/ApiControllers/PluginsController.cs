using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CVF.App.Manager;
using CVF.App.Models;
using CVF.Contract.Requests;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CVF.App.ApiControllers
{
    [Route("api/[controller]")]
    public class PluginsController : Controller
    {
        private readonly PluginManager manager;

        public PluginsController(PluginManager manager)
        {
            this.manager = manager;
        }

        [HttpGet]
        public IReadOnlyList<Plugin> Get()
        {
            return this.manager.Plugins;
        }

        // POST api/values
        [HttpPost]
        public async Task<object> Post([FromBody]PluginRequest request, CancellationToken token)
        {
            var client = this.manager.GetClient(request.PluginName);
            var item = this.manager.GetPluginItem(request.PluginName, request.CategoryName, request.ItemName);
            var parameters = request.Content
                .Select(kv => new PluginRequestParameter() { Name = kv.Key, Value = kv.Value })
                .ToList();
            var requestInfo = new PluginRequestInfo(request.Method, parameters);

            return await client.SendAsync<PluginRequestInfo, object>(item, requestInfo, token);
        }
    }
}
