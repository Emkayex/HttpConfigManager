using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;

namespace HttpConfigManager.ConfigDiscovery;
public record class ConfigEntryInfo(ConfigDefinition Definition, ConfigEntryBase Entry, BasePlugin BasePlugin);
