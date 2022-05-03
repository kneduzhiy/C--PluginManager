using AppWithPlugins;
using AppWithPlugins.SDK;
using System;
using System.Threading.Tasks;

namespace PluginDemo
{
    public class DemoPlugin : IPlugin
    {
        public Plugin PluginInfo => new()
        {
            Name = "DemoPlugin",
            Description = "This plugin is an example of a plugin",
            Version = "1.0",
            Author = "Kirill Neduzhiy",
            Copyright = "None"
        };

        public void Initialize()
        {
            Console.WriteLine("DemoPlugin::Initialize");
        }

        public Task InitializeAsync()
        {
            Console.WriteLine("DemoPlugin::InitializeAsync");
            return Task.CompletedTask;
        }

        public void Run(object parameters)
        {
            Console.WriteLine("DemoPlugin::Run");
            if (parameters is not string)
            {
                throw new("DemoPlugin: This plugin only accepts a string as an argument.");
            }

            string arg = parameters as string;
            Console.WriteLine("DemoPlugin: You passed me the following message: " + arg);
        }

        public Task RunAsync(object parameters)
        {
            Console.WriteLine("DemoPlugin::RunAsync");
            return Task.CompletedTask;
        }

        public void Destroy()
        {
            Console.WriteLine("DemoPlugin::Destroy");
        }

        public Task DestroyAsync()
        {
            Console.WriteLine("DemoPlugin::DestroyAsync");
            return Task.CompletedTask;
        }
    }
}
