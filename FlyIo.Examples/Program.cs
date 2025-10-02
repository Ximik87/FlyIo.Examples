using System.Text;
using FlyIo.Examples;

internal static class Program
{
    private static async Task Main()
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        var node = new MaelstromNode(new ConsoleProcessor(), new DebugLogger(false));
        node.On("init", Handlers.HandleInit(node));
        node.On("echo", Handlers.HandleEcho(node));
        node.On("broadcast", Handlers.HandleBroadcast(node));
        //node.On("broadcast_ok", Handlers.HandleBroadcastOk(node));
        node.On("topology", Handlers.HandleTopology(node));
        //node.On("read", Handlers.HandleReadBroadcast(node));
        node.On("error", Handlers.HandleError(node));
        node.On("generate", Handlers.HandleGenerate(node));
        node.On("read", Handlers.HandleReadCounter(node));
        node.On("add", Handlers.HandleAddCounter(node));

        await node.RunAsync();
    }
}