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

    public HttpServer(ConfigCollection collection)
    {
        var server = new WebServer(o => {
            o.WithUrlPrefix("http://localhost:5551")
            .WithMode(HttpListenerMode.EmbedIO);
        });

        server.WithLocalSessionManager()
            .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => {
                return ctx.SendStringAsync(HtmlGenerator.GetIndex(collection), HtmlContentType, Encoding.UTF8);
            }));

        Server = server;
    }

    public void Run()
    {
        Task.Run(async () => await Server.RunAsync());
    }
}
