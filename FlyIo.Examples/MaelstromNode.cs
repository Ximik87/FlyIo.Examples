using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using FlyIo.Examples;
using FlyIo.Examples.Models;

internal sealed class MaelstromNode
{
    private readonly Dictionary<string, Func<Envelope, Task>> _handlers = new(StringComparer.Ordinal);
    private readonly IStdIoProcessor _processor;
    private readonly IDebugLogger _logger;
    private readonly object _stdoutLock = new();
    private readonly List<int> messages = new();
    private int _nextMsgId = 1;
    private int _counter;
    public string NodeId { get; set; } = "";
    public IReadOnlyList<string> Peers { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> ForBroadcast { get; set; } = Array.Empty<string>();
    public IReadOnlyList<int> Messages => messages.AsReadOnly();
    public int Counter => _counter;

    public void On(string type, Func<Envelope, Task> handler)
        => _handlers[type] = handler;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        AllowTrailingCommas = false,
        ReadCommentHandling = JsonCommentHandling.Disallow
    };

    public MaelstromNode(IStdIoProcessor processor, IDebugLogger logger)
    {
        _processor = processor;
        _logger = logger;
    }

    public void AddMessage(int message)
    {
        messages.Add(message);
    }

    public void SetCounter(int value)
    {
        int initial, computed;
        do
        {
            initial = _counter;
            computed = initial + value;
        } while (initial != Interlocked.CompareExchange(ref _counter, computed, initial));
    }

    public async Task RunAsync()
    {
        _logger.Init();
        string? line;
        while ((line = await _processor.ReadLineAsync()) is not null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;


            await _logger.Log(line);

            Envelope env;
            try
            {
                env = JsonSerializer.Deserialize<Envelope?>(line, _jsonOptions) ??
                      throw new InvalidOperationException("null envelope");
            }
            catch (Exception ex)
            {
                WriteTechError($"failed to parse json: {ex.Message}");
                continue;
            }

            var type = env.Body.Type;
            if (type is null)
            {
                await ReplyAsync(env, new { type = "error" });
                continue;
            }

            if (_handlers.TryGetValue(type, out var handler))
            {
                try
                {
                    await handler(env);
                }
                catch (Exception ex)
                {
                    WriteTechError($"handler error for '{type}': {ex.Message}");
                }
            }
            else
            {
                WriteTechError($"no handler for type '{type}'");
            }
        }

        await _logger.Flush();
    }

    public async Task ReplyAsync(Envelope req, object body)
    {
        int? inReplyTo = req.Body.MsgId;
        var replyBody = JsonSerializer.SerializeToElement(body, _jsonOptions);
        var bodyObj = replyBody.ToMutableDictionary();
        if (inReplyTo.HasValue)
            bodyObj["in_reply_to"] = JsonSerializer.SerializeToElement(inReplyTo.Value, _jsonOptions);

        var bodyElement = JsonSerializer.SerializeToElement(bodyObj, _jsonOptions);

        var reply = new Envelope
        {
            Src = req.Dest,
            Dest = req.Src,
            Body = bodyElement.Deserialize<Body>(_jsonOptions),
        };

        await WriteAsync(reply);
    }

    public async Task SendAsync(string dest, object body)
    {
        // For non-reply messages: add a fresh msg_id
        var bodyEl = JsonSerializer.SerializeToElement(body, _jsonOptions);
        var bodyObj = bodyEl.ToMutableDictionary();
        var id = Interlocked.Increment(ref _nextMsgId);
        bodyObj["msg_id"] = JsonSerializer.SerializeToElement(id, _jsonOptions);

        var bodyElement = JsonSerializer.SerializeToElement(bodyObj, _jsonOptions);

        var env = new Envelope
        {
            Src = NodeId,
            Dest = dest,
            Body = bodyElement.Deserialize<Body>(_jsonOptions),
        };

        await WriteAsync(env);
    }

    private Task WriteAsync(Envelope env)
    {
        var json = JsonSerializer.Serialize(env, _jsonOptions);

        lock (_stdoutLock)
        {
            _processor.WriteLine(json);
        }

        return Task.CompletedTask;
    }

    public void WriteTechError(string error)
    {
        var json = JsonSerializer.Serialize(new { error }, _jsonOptions);

        lock (_stdoutLock)
        {
            _processor.WriteLine(json);
        }
    }
}