using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Actions;
using HttpConfigManager.ConfigDiscovery;

namespace HttpConfigManager.Server;
public class HttpServer
{
    private const string HtmlContentType = "text/html";

    private readonly WebServer Server;

    public HttpServer(ConfigCollection cfgs)
    {
        var server = new WebServer(o => {
            o.WithUrlPrefix("http://localhost:5551")
            .WithMode(HttpListenerMode.EmbedIO);
        });

        server.WithLocalSessionManager();

        // Setup the main route to return the index page
        server.OnGet("/", async ctx => {
            await ctx.SendStringAsync(HtmlGenerator.GetIndex(cfgs), HtmlContentType, Encoding.UTF8);
        });

        // Set the API route to update an option
        server.OnPost("/api/update-option", async ctx => {
            // Go through each key and value in the request data to be able to set the correct option
            var data = await ctx.GetRequestFormDataAsync();
            var success = true;
            foreach (var requestKey in data.AllKeys)
            {
                var value = data.Get(requestKey);

                // Get the parts from the key that can be used to find the correct mod/section/option to set from the ConfigCollection
                var keyParts = requestKey!.Split(HtmlGenerator.ModSectionOptionSeparator);
                var mod = keyParts[0];
                var section = keyParts[1];
                var option = keyParts[2];

                var collectionKey = new ConfigEntryKey(mod, section, option);
                if (cfgs.TryGetValue(collectionKey, out var entryInfo))
                {
                    entryInfo.Entry.SetSerializedValue(value);
                }
                else
                {
                    success = false;
                }
            }

            await ctx.SendDataAsync(new { success });
        });

        Server = server;
    }

    public void Run()
    {
        Task.Run(async () => await Server.RunAsync());
    }
}
