using NCommandBus.Core.Abstractions;
using System.Collections.Immutable;
using YAEP.Utils;

namespace ExtensionsTests.CommandBusTests;

public record DoStuff1(string Id,string Message);

public class DummyService : CommandBusBase
{
    private readonly Dictionary<string, string> _messages;
    public ImmutableDictionary<string, string> State => _messages.ToImmutableDictionary();

    public DummyService()
    {
        _messages = new Dictionary<string, string>();
        On<DoStuff1>((cmd, token) =>
        {
            _messages[cmd.Id] = cmd.Message;
            return ValueTask.CompletedTask;
        });

        On<DoStuff1, Result1>((cmd, token) =>
        {
            _messages[cmd.Id] = cmd.Message;
            return new ValueTask<Result1>(new Result1($"{cmd.Id} - {cmd.Message}").AsTaskFromResult());
        });
    }
}


public record GenericCmd<T>(Guid Id,T Message);


//public class StringGenericDummyService : CommandBusBase
//{
//    private readonly Dictionary<string, string> _messages;
//    public ImmutableDictionary<string, string> State => _messages.ToImmutableDictionary();

//    public StringGenericDummyService()
//    {
//        _messages = new Dictionary<string, string>();
//        On<GenericCmd<string>>((cmd, token) =>
//        {
//            _messages[cmd.Id.ToString()] = cmd.Message!.ToString()!;
//            return ValueTask.CompletedTask;
//        });

//        On<GenericCmd<string>, Result1>((cmd, token) =>
//        {
//            return new ValueTask<Result1>(new Result1($"{cmd.Id} - {cmd.Message}").AsTaskFromResult());
//        });
//    }

//}
public class GenericDummyService<T> : CommandBusBase
{
    private readonly Dictionary<string, string> _messages;
    public ImmutableDictionary<string, string> State => _messages.ToImmutableDictionary();

    public GenericDummyService()
    {
        _messages = new Dictionary<string, string>();
        On<GenericCmd<T>>((cmd, token) =>
        {
            _messages[cmd.Id.ToString()] = cmd.Message!.ToString()!;
            return ValueTask.CompletedTask;
        });

        On<GenericCmd<T>, Result1>((cmd, token) =>
        {            
            return new ValueTask<Result1>(new Result1($"{cmd.Id} - {cmd.Message}").AsTaskFromResult());
        });
    }
}
