using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CVF.App.Models;
using Newtonsoft.Json;

namespace CVF.App.Manager
{
    public class PluginManager
    {
        private readonly ConcurrentDictionary<string, PluginClient> clients = new ConcurrentDictionary<string, PluginClient>();

        private readonly IReadOnlyList<Plugin> plugins;

        public PluginManager(string path)
        {
            this.plugins = this.GetPlugins(path);
        }

        public IReadOnlyList<Plugin> Plugins
        {
            get
            {
                return this.plugins;
            }
        }

        public Plugin GetPlugin(string name)
        {
            return this.Plugins.FirstOrDefault(p => p.Name == name);
        }

        public PluginItem GetPluginItem(string pluginName, string categoryName, string itemName)
        {
            var plugin = this.Plugins.Single(p => p.Name == pluginName);
            var category = plugin.Categories.Single(c => c.Name == categoryName);
            var item = category.Items.Single(i => i.Name == itemName);

            return item;
        }

        public PluginClient GetClient(string pluginName)
        {
            return this.clients.GetOrAdd(pluginName, name =>
            {
                var plugin = this.Plugins.First(v => v.Name == name);
                return new PluginClient(plugin);
            });
        }

        private IReadOnlyList<Plugin> GetPlugins(string path)
        {
            var files = Directory.GetFiles(path, "manifest.json", SearchOption.AllDirectories);
            var result = new List<Plugin>();
            foreach (var file in files)
            {
                var plugin = JsonConvert.DeserializeObject<Plugin>(File.ReadAllText(file));
                result.Add(plugin);
            }

            return result;
        }
    }
}
