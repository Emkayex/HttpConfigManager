namespace HttpConfigManager.ConfigDiscovery;
public readonly record struct ConfigEntryKey(string AssemblyName, string SectionName, string OptionName);
