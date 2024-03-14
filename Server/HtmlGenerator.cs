using System;
using System.Collections.Generic;
using System.Linq;
using HttpConfigManager.ConfigDiscovery;

namespace HttpConfigManager.Server;
public static class HtmlGenerator
{
    public static string GetIndex(ConfigCollection cfgs)
    {
        var htmlSnippets = new List<string>();

        // Get a list of the all the mods that can be configured and then generate HTML for each
        var mods = cfgs.Select(kv => kv.Key.AssemblyName).Distinct().OrderBy(x => x);
        foreach (var mod in mods)
        {
            // Add a header for this mod
            var modHeader = $"<h1>{mod}</h1>";
            htmlSnippets.Add(modHeader);

            // Get the key-value pairs with ConfigEntryInfo objects in this assembly and then sort by section within the mod
            var cfgsInMod = cfgs.Where(kv => kv.Key.AssemblyName == mod);
            var sections = cfgsInMod.Select(kv => kv.Key.SectionName).Distinct().OrderBy(x => x);

            foreach (var section in sections)
            {
                // Generate a header for this section's options
                var sectionHeader = $"<h2>{section}</h2>";
                htmlSnippets.Add(sectionHeader);

                // Get all of the ConfigEntryInfo objects in this section
                var cfgsInSection = cfgsInMod.Where(kv => kv.Key.SectionName == section).OrderBy(kv => kv.Key.OptionName);
                foreach (var (configEntryKey, entryInfo) in cfgsInSection)
                {
                    // Generate an HTML snippet for this option
                    var optionHeader = $"<h3>{configEntryKey.OptionName}</h3>";
                    htmlSnippets.Add(optionHeader);

                    var description = $"<p>{entryInfo.Entry.Description.Description}</p>";
                    htmlSnippets.Add(description);

                    var value = $"<p>{entryInfo.Entry.BoxedValue}</p>";
                    htmlSnippets.Add(value);
                }
            }
        }

        var html = GetPageHtml("Configure", Array.Empty<string>(), htmlSnippets);
        return html;
    }

    private static string GetPageHtml(string title, IEnumerable<string> scripts, IEnumerable<string> bodySnippets)
    {
        var html = @$"
{GetHtmlPre()}
    {string.Join("\n    ", GetHtmlHead(title, scripts).SplitLines())}
    {string.Join("\n    ", GetHtmlBody(bodySnippets))}
{GetHtmlPost()}
";

        return html;
    }

    private static string GetHtmlHead(string title, IEnumerable<string> scripts)
    {
        IEnumerable<string> IterateScriptTags()
        {
            // Generate the script tags with the script texts inserted between them
            foreach (var script in scripts)
            {
                var scriptTagSet = $"<script type=\"application/javascript\">\n{script}\n</script>";
                yield return scriptTagSet;
            }
        }

        // TODO: Insert CSS
        var src = @$"
<head>
    <title>{title}</title>

    {string.Join("\n    ", IterateScriptTags())}
</head>
";

        return src;
    }

    private static string GetHtmlBody(IEnumerable<string> snippets)
    {
        var src = @$"
<body>
    {string.Join("\n    ", snippets)}
</body>
";

        return src;
    }

    private static string GetHtmlPre() => @"
<!DOCTYPE html>
<html lang=""en"">
";

    private static string GetHtmlPost() => @"
</html>
";

    private static IEnumerable<string> SplitLines(this string s)
    {
        var splitChars = new string[] { "\r", "\n", "\r\n" };
        foreach (var line in s.Split(splitChars, StringSplitOptions.None))
        {
            yield return line;
        }
    }
}
