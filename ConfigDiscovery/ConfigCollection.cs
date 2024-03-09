using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HttpConfigManager.ConfigDiscovery;
public class ConfigCollection : IDictionary<ConfigEntryKey, ConfigEntryInfo>
{
    private readonly Dictionary<ConfigEntryKey, ConfigEntryInfo> BackingDict;

    public ConfigCollection()
    {
        BackingDict = new();
    }

    public void RefreshConfigEntryInfos()
    {
        // Clear the dictionary to ensure a key isn't added twice (which would incorrectly log a warning)
        BackingDict.Clear();

        // Convert each ConfigEntryInfo object into a simple key of strings and then store the info into the dictionary with the key
        // The web interface will make requests using the key, so simple strings can be sent back and forth
        foreach (var entryInfo in Searcher.FindConfigEntryInfos())
        {
            var key = new ConfigEntryKey(entryInfo.BasePlugin.GetType().Assembly.GetName().Name!, entryInfo.Definition.Section, entryInfo.Definition.Key);
            if (!this.TryAdd(key, entryInfo))
            {
                Plugin.Logger.LogWarning($"Skipping duplicate key: {key}");
            }
        }
    }

    public ConfigEntryInfo this[ConfigEntryKey key] { get => BackingDict[key]; set => BackingDict[key] = value; }
    public ICollection<ConfigEntryKey> Keys => BackingDict.Keys;
    public ICollection<ConfigEntryInfo> Values => BackingDict.Values;
    public int Count => BackingDict.Count;
    public bool IsReadOnly => false;
    public void Add(ConfigEntryKey key, ConfigEntryInfo value) => BackingDict.Add(key, value);
    public void Add(KeyValuePair<ConfigEntryKey, ConfigEntryInfo> item) => BackingDict.Add(item.Key, item.Value);
    public void Clear() => BackingDict.Clear();
    public bool Contains(KeyValuePair<ConfigEntryKey, ConfigEntryInfo> item) => BackingDict.ContainsKey(item.Key) && (this[item.Key] == item.Value);
    public bool ContainsKey(ConfigEntryKey key) => BackingDict.ContainsKey(key);

    public void CopyTo(KeyValuePair<ConfigEntryKey, ConfigEntryInfo>[] array, int arrayIndex)
    {
        foreach (var (i, kv) in this.Select((x, i) => (i, x)))
        {
            array[i + arrayIndex] = kv;
        }
    }

    public IEnumerator<KeyValuePair<ConfigEntryKey, ConfigEntryInfo>> GetEnumerator() => BackingDict.GetEnumerator();
    public bool Remove(ConfigEntryKey key) => BackingDict.Remove(key);
    public bool Remove(KeyValuePair<ConfigEntryKey, ConfigEntryInfo> item) => Remove(item);
    public bool TryGetValue(ConfigEntryKey key, [MaybeNullWhen(false)] out ConfigEntryInfo value) => BackingDict.TryGetValue(key, out value);
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
