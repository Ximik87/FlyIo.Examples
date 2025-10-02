using Moq;

namespace FlyIo.Examples.Tests;

public class MaelstromNodeTests
{
    [Fact]
    public async Task Init_Test()
    {
        var request =
            "{\"id\":4,\"src\":\"c4\",\"dest\":\"n2\",\"body\":{\"type\":\"init\",\"node_id\":\"n2\",\"node_ids\":[\"n1\",\"n2\",\"n3\",\"n4\",\"n5\"],\"msg_id\":1}}";
        var moq = new Mock<IStdIoProcessor>();
        moq.SetupSequence(m => m.ReadLineAsync())
            .ReturnsAsync(request)
            .ReturnsAsync((string?)null);
        string expected = "{\"src\":\"n2\",\"dest\":\"c4\",\"body\":{\"type\":\"init_ok\",\"in_reply_to\":1}}";
        var sut = new MaelstromNode(moq.Object, Mock.Of<IDebugLogger>());
        sut.On("init", Handlers.HandleInit(sut));

        // Act
        await sut.RunAsync();

        moq.Verify(x => x.WriteLine(expected));
    }

    [Fact]
    public async Task Echo_Test()
    {
        var a =
            "{\"id\":4,\"src\":\"c4\",\"dest\":\"n2\",\"body\":{\"type\":\"echo\",\"msg_id\":10,\"echo\":\"Please echo 82\"}}";
        var moq = new Mock<IStdIoProcessor>();
        moq.SetupSequence(m => m.ReadLineAsync())
            .ReturnsAsync(a)
            .ReturnsAsync((string?)null);
        string expected =
            "{\"src\":\"n2\",\"dest\":\"c4\",\"body\":{\"type\":\"echo_ok\",\"in_reply_to\":10,\"echo\":\"Please echo 82\"}}";
        var sut = new MaelstromNode(moq.Object, Mock.Of<IDebugLogger>());
        sut.On("echo", Handlers.HandleEcho(sut));

        // Act
        await sut.RunAsync();

        moq.Verify(x => x.WriteLine(expected));
    }

    [Fact]
    public async Task Topology_Test()
    {
        var request =
            "{\"id\":14,\"src\":\"c6\",\"dest\":\"n3\",\"body\":{\"type\":\"topology\",\"topology\":{\"n1\":[\"n4\",\"n2\"],\"n2\":[\"n5\",\"n3\",\"n1\"],\"n3\":[\"n2\"],\"n4\":[\"n1\",\"n5\"],\"n5\":[\"n2\",\"n4\"]},\"msg_id\":1}}";
        var moq = new Mock<IStdIoProcessor>();
        moq.SetupSequence(m => m.ReadLineAsync())
            .ReturnsAsync(request)
            .ReturnsAsync((string?)null);
        string expected = "{\"src\":\"n3\",\"dest\":\"c6\",\"body\":{\"type\":\"topology_ok\",\"in_reply_to\":1}}";
        var sut = new MaelstromNode(moq.Object, Mock.Of<IDebugLogger>());
        sut.On("topology", Handlers.HandleTopology(sut));

        // Act
        await sut.RunAsync();

        moq.Verify(x => x.WriteLine(expected));
    }

    [Fact]
    public async Task Broadcast_Test()
    {
        var request =
            "{\"id\":9,\"src\":\"c3\",\"dest\":\"n1\",\"body\":{\"type\":\"broadcast\",\"msg_id\":5,\"message\":42}}";
        var moq = new Mock<IStdIoProcessor>();
        moq.SetupSequence(m => m.ReadLineAsync())
            .ReturnsAsync(request)
            .ReturnsAsync((string?)null);
        string expected = "{\"src\":\"n1\",\"dest\":\"c3\",\"body\":{\"type\":\"broadcast_ok\",\"in_reply_to\":5}}";
        var sut = new MaelstromNode(moq.Object, Mock.Of<IDebugLogger>());
        sut.On("broadcast", Handlers.HandleBroadcast(sut));

        // Act
        await sut.RunAsync();

        moq.Verify(x => x.WriteLine(expected));
        Assert.Single(sut.Messages);
        Assert.Equal(42, sut.Messages[0]);
    }

    [Fact]
    public async Task Read_Test()
    {
        var request =
            "{\"id\":11,\"src\":\"c5\",\"dest\":\"n4\",\"body\":{\"type\":\"read\",\"msg_id\":7}}";
        var moq = new Mock<IStdIoProcessor>();
        moq.SetupSequence(m => m.ReadLineAsync())
            .ReturnsAsync(request)
            .ReturnsAsync((string?)null);
        string expected =
            "{\"src\":\"n4\",\"dest\":\"c5\",\"body\":{\"type\":\"read_ok\",\"in_reply_to\":7,\"messages\":[122]}}";
        var sut = new MaelstromNode(moq.Object, Mock.Of<IDebugLogger>());
        sut.AddMessage(122);
        sut.On("read", Handlers.HandleReadBroadcast(sut));

        // Act
        await sut.RunAsync();

        moq.Verify(x => x.WriteLine(expected));
        Assert.NotEmpty(sut.Messages);
    }

    [Fact]
    public async Task AddCounter_Test()
    {
        var request = "{\"id\":4,\"src\":\"c2\",\"dest\":\"n0\",\"body\":{\"delta\":4,\"type\":\"add\",\"msg_id\":2}}";
        var moq = new Mock<IStdIoProcessor>();
        moq.SetupSequence(m => m.ReadLineAsync())
            .ReturnsAsync(request)
            .ReturnsAsync((string?)null);
        var sut = new MaelstromNode(moq.Object, Mock.Of<IDebugLogger>());
        string expected = "{\"src\":\"n0\",\"dest\":\"c2\",\"body\":{\"type\":\"add_ok\",\"in_reply_to\":2}}";
        sut.On("add", Handlers.HandleAddCounter(sut));

        // Act
        await sut.RunAsync();

        moq.Verify(x => x.WriteLine(expected));
        Assert.Equal(4, sut.Counter);
    }

    [Fact]
    public async Task ReadCounter_Test()
    {
        var request = "{\"id\":6,\"src\":\"c2\",\"dest\":\"n0\",\"body\":{\"type\":\"read\",\"msg_id\":3}}";
        var moq = new Mock<IStdIoProcessor>();
        moq.SetupSequence(m => m.ReadLineAsync())
            .ReturnsAsync(request)
            .ReturnsAsync((string?)null);
        var sut = new MaelstromNode(moq.Object, Mock.Of<IDebugLogger>());
        string expected = "{\"src\":\"n0\",\"dest\":\"c2\",\"body\":{\"type\":\"read_ok\",\"in_reply_to\":3,\"value\":6}}";
        sut.SetCounter(6);
        sut.On("read", Handlers.HandleReadCounter(sut));

        // Act
        await sut.RunAsync();

        moq.Verify(x => x.WriteLine(expected));
    }
}