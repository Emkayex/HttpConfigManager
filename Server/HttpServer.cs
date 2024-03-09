using System.Threading.Tasks;
using EmbedIO;
using EmbedIO.Actions;

namespace HttpConfigManager.Server;
public class HttpServer
{
    private readonly WebServer Server;

    public HttpServer()
    {
        var server = new WebServer(o => {
            o.WithUrlPrefix("http://localhost:5551")
            .WithMode(HttpListenerMode.EmbedIO);
        });

        server.WithLocalSessionManager()
            .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Hello, World!"})));

        Server = server;
    }

    public void Run()
    {
        Task.Run(async () => await Server.RunAsync());
    }
}
