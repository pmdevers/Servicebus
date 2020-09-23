namespace PMDEvers.Servicebus.Interfaces
{
    public interface IQueryHandler<in TQuery, out TResult> where TQuery : class, IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }
}
