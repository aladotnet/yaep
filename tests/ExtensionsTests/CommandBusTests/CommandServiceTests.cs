using FluentAssertions;
using NCommandBus.Core.Abstractions;

namespace ExtensionsTests.CommandBusTests;


public record UnregisteredCommand(string Id);


public record Result1(string Message);

public class CommandServiceTests
{
    [Fact]
    public async Task Handle_No_marker_interface_Executes_the_registered_handler()
    {
        var service = new DummyService();
        var id = Guid.NewGuid().ToString();
        var cmd = new DoStuff1(id, "Test1234");
        await service.Handle(cmd, CancellationToken.None).DetachedAwait();

        service.State[id].Should().NotBeNull();
        service.State[id].Should().Be(cmd.Message);
    }

    [Fact]
    public async Task Handle_Executes_the_registered_handler_and_returns_a_result()
    {
        var service = new DummyService();
        var id = Guid.NewGuid().ToString();
        var cmd = new DoStuff1(id, "Test1234");

        var result = await service.Handle<Result1>(cmd, CancellationToken.None).DetachedAwait();
        var expectedResult = new Result1($"{cmd.Id} - {cmd.Message}");

        service.State[id].Should().NotBeNull();
        service.State[id].Should().Be(cmd.Message);
        result.Should().Be(expectedResult);
    }

    [Fact]
    public async Task Handle_given_generic_cmd_Executes_the_registered_handler_and_returns_a_result()
    {
        var service = new GenericDummyService<string>();
        var id = Guid.NewGuid();
        var cmd = new GenericCmd<string>(id, "Test1234");

        var result = await service.Handle<Result1>(cmd, CancellationToken.None);
        var expectedResult = new Result1($"{cmd.Id} - {cmd.Message}");

        result.Should().Be(expectedResult);
    }


    [Fact]
    public async Task Handle_giving_an_unregistered_command_throws_an_exception()
    {

        var service = new DummyService();
        var id = Guid.NewGuid().ToString();
        var cmd = new UnregisteredCommand(id);

        await service.Invoking(s => s.Handle(cmd, CancellationToken.None).AsTask())
            .Should().ThrowAsync<InvalidOperationException>();
    }
}
