using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CVF.App.Models;
using Newtonsoft.Json;
using System.ComponentModel.Composition.Hosting;
using System;

namespace CVF.App.Manager
{
    public class PluginManager
    {
        private const string ManifestFileName = "manifest.json";
        private readonly ConcurrentDictionary<string, PluginClient> clients = new ConcurrentDictionary<string, PluginClient>();
        private IReadOnlyList<Plugin> plugins;
        private readonly TimeSpan syncInterval = TimeSpan.FromSeconds(5);
        private readonly FileSystemWatcher watcher;
        private readonly string path;

        public PluginManager(string path)
        {
            this.path = path;
            this.plugins = this.GetPlugins(path);
            this.watcher = new FileSystemWatcher(path, "*" + ManifestFileName);
            this.watcher.IncludeSubdirectories = true;
            this.watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;
            this.watcher.Changed += Watcher_Changed;
            this.watcher.Deleted += Watcher_Changed;
            this.watcher.Created += Watcher_Changed;
            this.watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("Find file change: {0}", e.FullPath);
            this.SyncPlugins();
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

        private void SyncPlugins()
        {
            try
            {
                Console.WriteLine("Start to sync plugins.");
                var plugins = this.GetPlugins(this.path);
                this.plugins = plugins;
                Console.WriteLine("Finish sync pluings");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sync plugins error. {0}", ex);
            }
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
