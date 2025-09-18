namespace FlyIo.Examples;
interface IStdIoProcessor
{
    Task<string?> ReadLineAsync();
    void WriteLine(string line);
}

internal class ConsoleProcessor : IStdIoProcessor
{
    public Task<string?> ReadLineAsync()
        => Console.In.ReadLineAsync();

    public void WriteLine(string line)
    {
        Console.Out.WriteLine(line);
        Console.Out.Flush();
    }
}