using BepInEx;
using BepInEx.Configuration;

namespace HttpConfigManager;
public record class ConfigEntryInfo(ConfigDefinition Definition, ConfigEntryBase Entry, PluginInfo PluginInfo);
