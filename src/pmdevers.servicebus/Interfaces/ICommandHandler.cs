namespace PMDEvers.Servicebus
{
    public interface ICommandHandler<in TCommand, out TResponse>
        where TCommand : ICommand<TResponse>
    {
        TResponse Handle(TCommand command);
    }

    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        void Handle(TCommand command);
    }
}
