using System.Text;

namespace FlyIo.Examples;

internal interface IDebugLogger
{
    void Init();
    Task Log(string line);
    Task Flush();
}

internal class DebugLogger : IDebugLogger
{
    private readonly bool _isEnabled;
    private FileStream? _log;

    public DebugLogger(bool isEnabled)
    {
        _isEnabled = isEnabled;
    }

    public void Init()
    {
        if (!_isEnabled)
            return;

        _log = File.Create($@"D:\Sources\FlyIo.Examples\FlyIo.Examples\{Guid.NewGuid()}-log.txt");
    }

    public async Task Log(string line)
    {
        if (!_isEnabled || _log is null)
            return;

        await _log.WriteAsync(Encoding.UTF8.GetBytes(line + Environment.NewLine));
        await _log.FlushAsync();
    }

    public async Task Flush()
    {
        if (!_isEnabled || _log is null)
            return;

        await _log.FlushAsync();
    }
}