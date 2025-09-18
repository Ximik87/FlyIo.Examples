namespace FlyIo.Examples;

internal static class Handlers
{
    // init -> init_ok
    public static Func<Envelope, Task> HandleInit(MaelstromNode node) => async req =>
    {
        node.NodeId = req.Body.NodeId;
        node.Peers = req.Body.NodeIds.Select(e => e).ToArray();

        await node.ReplyAsync(req, new { type = "init_ok" });
    };

    // echo -> echo_ok (echo field must be mirrored back)
    public static Func<Envelope, Task> HandleEcho(MaelstromNode node) => async req =>
    {
        await node.ReplyAsync(req, new { type = "echo_ok", echo = req.Body.Echo });
    };

    public static Func<Envelope, Task> HandleBroadcast(MaelstromNode node) => async req =>
    {
        var msg = req.Body.Message;
        if (msg is not null)
        {
            node.AddMessage(msg.Value);
        }

        foreach (var peer in node.ForBroadcast)
        {
            await node.SendAsync(peer, new { type = "broadcast", message = msg, msg_id = req.Body.MsgId });
        }

        await node.ReplyAsync(req, new { type = "broadcast_ok" });
    };

    public static Func<Envelope, Task> HandleTopology(MaelstromNode node) => async req =>
    {
        node.ForBroadcast = GetNodes(req, node.NodeId);

        await node.ReplyAsync(req, new { type = "topology_ok" });
    };

    public static Func<Envelope, Task> HandleRead(MaelstromNode node) => async req =>
    {
        await node.ReplyAsync(req, new { type = "read_ok", messages = node.Messages.ToArray() });
    };

    public static Func<Envelope, Task> HandleError(MaelstromNode node) => async req =>
    {
        await node.ReplyAsync(req, new { type = "error", messages = node.Messages });
    };

    public static Func<Envelope, Task> HandleGenerate(MaelstromNode node) => async req =>
    {
        await node.ReplyAsync(req, new { type = "generate_ok", id = Guid.NewGuid().ToString() });
    };

    private static string[] GetNodes(Envelope req, string name)
    {
        var topology = req.Body.Topology.Value;
        var topologyType = typeof(Topology);
        var property = topologyType.GetProperty(name);
    
        return property?.GetValue(topology) as string[] ?? Array.Empty<string>();
    }
}