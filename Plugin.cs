using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HttpConfigManager.ConfigDiscovery;
using HttpConfigManager.Server;

namespace HttpConfigManager;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    public static ManualLogSource Logger { get => Log; }

    private readonly static ConfigCollection Configs = new();

    public override void Load()
    {
        // Create a global logger for use by other classes
        Log = base.Log;

        Task.Run(async () => {
            await Task.Delay(5000);
            Configs.RefreshConfigEntryInfos();

            foreach (var key in Configs.Keys)
            {
                Logger.LogError(key);
            }

            var server = new HttpServer(Configs);
            server.Run();
        });
    }
}
