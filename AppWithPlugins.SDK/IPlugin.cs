using System.Threading.Tasks;

namespace AppWithPlugins.SDK
{
    public interface IPlugin
    {
        Plugin PluginInfo { get; }

        void Initialize();
        Task InitializeAsync();

        void Run(object parameters);
        Task RunAsync(object parameters);

        void Destroy();
        Task DestroyAsync();
    }
}
