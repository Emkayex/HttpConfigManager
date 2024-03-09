using System.Collections.Generic;
using System.Linq;
using BepInEx.Unity.IL2CPP;

namespace HttpConfigManager.ConfigDiscovery;
public static class Searcher
{
    public static IEnumerable<ConfigEntryInfo> FindConfigEntryInfos()
    {
        // Find all loaded plugins (they must inherit from BasePlugin)
        var pluginInfos = IL2CPPChainloader.Instance.Plugins.Values.Where(x => x.Instance is BasePlugin).ToList();
        foreach (var pluginInfo in pluginInfos)
        {
            // Get the instance of the plugin so the bound ConfigEntry objects can be discovered
            var basePlugin = (BasePlugin)pluginInfo.Instance;
            if (basePlugin is not null)
            {
                // For each ConfigEntry, create a ConfigEntryInfo object for easy reference elsewhere
                foreach (var (key, value) in basePlugin.Config)
                {
                    var info = new ConfigEntryInfo(key, value, basePlugin);
                    yield return info;
                }
            }
        }
    }
}
