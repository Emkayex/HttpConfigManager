using System.Linq;
using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Actions;
using HttpConfigManager.ConfigDiscovery;

namespace HttpConfigManager.Server;
public class HttpServer
{
    private readonly WebServer Server;

    public HttpServer(ConfigCollection collection)
    {
        var server = new WebServer(o => {
            o.WithUrlPrefix("http://localhost:5551")
            .WithMode(HttpListenerMode.EmbedIO);
        });

        server.WithLocalSessionManager()
            .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => {
                return ctx.SendDataAsync(collection.Keys.ToList());
            }));

        Server = server;
    }

    public void Run()
    {
        Task.Run(async () => await Server.RunAsync());
    }
}
