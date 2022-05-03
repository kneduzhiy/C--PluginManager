using AppWithPlugins.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppWithPlugins
{
    public class PluginManager
    {
        private IList<IPlugin> RegisteredPlugins { get; } = new List<IPlugin>();

        public List<IPlugin> GetPlugins() => RegisteredPlugins.ToList();

        public async Task RegisterPlugin(IPlugin plugin)
        {
            if (plugin is null || plugin?.PluginInfo is null)
            {
                throw new("Cannot register null plugin or plugin with null info");
            }

            var existingPlugin = RegisteredPlugins.FirstOrDefault(_ => _.PluginInfo.Name.ToLower() == plugin.PluginInfo.Name.ToLower());
            if (existingPlugin is not null)
            {
                throw new($"ERROR: Plugin {plugin.PluginInfo.Name} is already registered.");
            }

            RegisteredPlugins.Add(plugin);

            plugin.Initialize();
            await plugin.InitializeAsync();

            Console.WriteLine("[PluginManager] Registered plugin " + plugin.PluginInfo.Name);
        }

        public async Task Unload(string pluginName)
        {
            var existingPlugin = RegisteredPlugins.FirstOrDefault(_ => _.PluginInfo.Name.ToLower() == pluginName.ToLower());
            if (existingPlugin is not null)
            {
                existingPlugin.Destroy();
                await existingPlugin.DestroyAsync();

                RegisteredPlugins.Remove(existingPlugin);
            }

            Console.WriteLine("[PluginManager] Unloaded " + pluginName);
        }

        public async Task UnloadAll()
        {
            foreach (var plugin in RegisteredPlugins)
            {
                plugin.Destroy();
                await plugin.DestroyAsync();
            }

            RegisteredPlugins.Clear();
            Console.WriteLine("[PluginManager] Unloaded all plugins");
        }
    }
}
