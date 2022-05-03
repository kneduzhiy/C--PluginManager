using AppWithPlugins.SDK;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AppWithPlugins
{
    public class App
    {
        private PluginManager PluginManager { get; }

        public App()
        {
            PluginManager = new();
        }

        private void DiscoverPlugins()
        {
            int registeredPluginsCount = 0;

            var pluginDirectory = Directory.CreateDirectory("plugins");
            var pluginDirectoryAssemblyFiles = Directory.GetFiles("plugins")
                .Where(_ => _.EndsWith(".dll"))
                .ToList();

            pluginDirectoryAssemblyFiles.ForEach(async _ =>
            {
                bool success = await TryRegisterPluginFromFile(_);

                if (success)
                {
                    registeredPluginsCount++;
                }
            });

            Console.WriteLine($"[App] Discovered and loaded {registeredPluginsCount} plugins successfully");
        }

        private async Task<bool> TryRegisterPluginFromFile(string dllFile)
        {
            try
            {
                var pluginAssembly = Assembly.LoadFrom(dllFile);
                var pluginAssemblyEntrypoint = pluginAssembly.GetTypes().FirstOrDefault(_ => typeof(IPlugin).IsAssignableFrom(_));

                var pluginEntrypointInstance = Activator.CreateInstance(pluginAssemblyEntrypoint) as IPlugin;
                await PluginManager.RegisterPlugin(pluginEntrypointInstance);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[App::TryRegisterPlugin] Failed to register plugin " + dllFile + " because of: " + e.Message);
                return false;
            }
        }

        public async void Run()
        {
            DiscoverPlugins();

            string command;
            while (true)
            {
                command = Console.ReadLine();
                if (command.ToLower() == "showplugins")
                {
                    var plugins = PluginManager.GetPlugins();
                    Console.WriteLine($"-- Installed plugins ({plugins.Count}) --");

                    foreach (IPlugin plugin in plugins)
                    {
                        Console.WriteLine($">> {plugin.PluginInfo.Name} v{plugin.PluginInfo.Version} by {plugin.PluginInfo.Author}");
                    }

                    Console.WriteLine();
                    Console.WriteLine();

                    continue;
                }

                if (command.ToLower().StartsWith("runplugin"))
                {
                    var requestedPlugin = command.Split(" ")[1];
                    var plugin = PluginManager.GetPlugins().FirstOrDefault(_ => _.PluginInfo.Name == requestedPlugin);
                    if (plugin is null)
                    {
                        Console.WriteLine($"Plugin {requestedPlugin} is not registered, not found.");
                        continue;
                    }

                    var args = command.Split(" ")[2];
                    plugin.Run(args);
                    await plugin.RunAsync(args);

                    continue;
                }

                if (command.ToLower() == "reloadplugins")
                {
                    await PluginManager.UnloadAll();
                    DiscoverPlugins();

                    continue;
                }

                if (command.ToLower() == "exit")
                {
                    break;
                }

                Console.WriteLine("The command has not been found");
            }
        }
    }
}
