﻿using System.Threading;
using System.Threading.Tasks;

namespace PMDEvers.Servicebus
{
    public interface ICancellableAsyncCommandHandler<in TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }

    public interface ICancellableAsyncCommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}
