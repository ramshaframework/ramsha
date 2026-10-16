using LiteBus.Commands.Abstractions;
using Ramsha.LocalMessaging.Abstractions;

namespace Ramsha.LocalMessaging;

public interface IRamshaCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>, IRamshaRequestHandler
where TCommand : IRamshaCommand<TResult>
{

}

public interface IRamshaCommandHandler<TCommand> : ICommandHandler<TCommand, IRamshaResult>, IRamshaRequestHandler
where TCommand : IRamshaCommand
{

}