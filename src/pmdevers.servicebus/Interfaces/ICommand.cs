namespace PMDEvers.Servicebus
{
    public interface ICommand : ICommand<Unit>
    {
    }

    public interface ICommand<out TResponse>
    {
    }
}
