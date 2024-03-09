using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;

namespace HttpConfigManager;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    public static ManualLogSource Logger { get => Log; }

    public override void Load()
    {
        // Create a global logger for use by other classes
        Log = base.Log;

        Task.Run(async () => {
            await Task.Delay(5000);
            foreach (var configEntryInfo in FindConfigEntryInfos())
            {
                Logger.LogWarning(configEntryInfo);
            }
        });
    }

    private static IEnumerable<ConfigEntryInfo> FindConfigEntryInfos()
    {
        var pluginInfos = IL2CPPChainloader.Instance.Plugins.Values.Where(x => x.Instance is BasePlugin).ToList();
        foreach (var pluginInfo in pluginInfos)
        {
            var basePlugin = (BasePlugin)pluginInfo.Instance;
            if (basePlugin is not null)
            {
                foreach (var (key, value) in basePlugin.Config)
                {
                    var info = new ConfigEntryInfo(key, value, pluginInfo);
                    yield return info;
                }
            }
        }
    }
}
