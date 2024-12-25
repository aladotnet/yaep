namespace YAEP.Utils;

public delegate ValueTask<TResult> CommandHandler<TCommand, TResult>(TCommand command, CancellationToken cancellationToken);
public delegate ValueTask CommandHandler<TCommand>(TCommand command, CancellationToken cancellationToken);

public abstract class CommandBusBase
{
    private readonly Dictionary<Type, Delegate> _handlers = new();

    protected void On<TCommand, TResult>(CommandHandler<TCommand, TResult> handler)
    {
        var key = typeof(CommandWrapper<TCommand, TResult>);
        _handlers[key] = handler;
    }

    protected void On<TCommand>(CommandHandler<TCommand> handler)
    {
        _handlers[typeof(CommandWrapper<TCommand>)] = handler;
    }

    public ValueTask Handle<TCommand>(TCommand command, CancellationToken cancellationToken)
    {
        var key = _handlers.Keys.FirstOrDefault(k => k.GenericTypeArguments.Length == 1 &&
                                                     k.GenericTypeArguments.Any(arg => arg == command!.GuardAgainstNull(nameof(command))!.GetType()));

        if (key.IsNull())
        {
            throw new InvalidOperationException($"No handler registered for this command type [{typeof(TCommand)}].");
        }

        var handler = _handlers[key!];

        return ((CommandHandler<TCommand>)handler)(command, cancellationToken);
    }

    public ValueTask<TResult> Handle<TResult>(object command, CancellationToken cancellationToken)
    {
        var key = _handlers.Keys.FirstOrDefault(k => k.GenericTypeArguments.Length == 2 &&
                                                     k.GenericTypeArguments[0] == command.GetType() &&
                                                     k.GenericTypeArguments[1] == typeof(TResult));

        if (key.IsNull())
        {
            throw new InvalidOperationException($"No handler registered for this command type [{command.GetType()}].");
        }

        var handler = _handlers[key!];

        object obj = handler.Method.Invoke(handler.Target, [command, cancellationToken]).GuardAgainstNull("result")!;

        return
         (ValueTask<TResult>)obj;
    }
}

internal record CommandWrapper<TCommand>(TCommand Command);
internal record CommandWrapper<TCommand, TResult>(TCommand Command);
