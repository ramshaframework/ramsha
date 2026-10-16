using Microsoft.Extensions.DependencyInjection;
using Ramsha.LocalMessaging.Abstractions;
using Ramsha.UnitOfWork.Abstractions;
using static Ramsha.RamshaErrorsCodes;


namespace Ramsha.LocalMessaging;

public abstract class RamshaCommandHandler<TCommand, TResult> : IRamshaCommandHandler<TCommand, TResult>
where TCommand : IRamshaCommand<TResult>
{
    [Injectable]
    public IServiceProvider ServiceProvider { get; set; } = default!;
    protected IUnitOfWorkManager UnitOfWorkManager => ServiceProvider.GetLazyRequiredService<IUnitOfWorkManager>().Value;

    protected Task<T> TransactionalUnitOfWork<T>(Func<Task<T>> action)
    {
        return UnitOfWork(action, true);
    }
    protected Task TransactionalUnitOfWork(Func<Task> action)
    {
        return UnitOfWork(action, true);
    }

    protected async Task<T> UnitOfWork<T>(Func<Task<T>> action, bool isTransactional = false)
    {
        var options = new UnitOfWorkOptions();
        options.IsTransactional = isTransactional;

        if (UnitOfWorkManager.TryBeginReserved(
                RamshaUnitOfWorkReservationNames.ActionUnitOfWorkReservationName,
                options))
        {
            var result = await action();
            if (UnitOfWorkManager.Current is not null)
            {
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }
            return result;
        }

        using (var uow = UnitOfWorkManager.Begin(options))
        {
            var result = await action();
            await uow.CompleteAsync();
            return result;
        }
    }

    protected async Task UnitOfWork(Func<Task> action, bool isTransactional = false)
    {
        var options = new UnitOfWorkOptions();
        options.IsTransactional = isTransactional;

        if (UnitOfWorkManager.TryBeginReserved(
                RamshaUnitOfWorkReservationNames.ActionUnitOfWorkReservationName,
                options))
        {
            await action();
            if (UnitOfWorkManager.Current is not null)
            {
                await UnitOfWorkManager.Current.SaveChangesAsync();
            }
            return;
        }

        using (var uow = UnitOfWorkManager.Begin(options))
        {
            await action();
            await uow.CompleteAsync();
        }
    }
    public abstract Task<TResult> HandleAsync(TCommand message, CancellationToken cancellationToken = default);
}

public abstract class RamshaCommandHandler<TCommand> : RamshaCommandHandler<TCommand, IRamshaResult>
where TCommand : IRamshaCommand<IRamshaResult>
{
    protected NoContentResult NoContent => RamshaResults.NoContent;

    protected SuccessResult Success()
    => RamshaResults.Success();

    protected SuccessResult<TValue> Success<TValue>(TValue value)
        => RamshaResults.Success(value);

    protected AcceptedResult Accepted(JobInfo job) => RamshaResults.Accepted(job);

    protected NotFoundError NotFound()
        => RamshaResults.NotFound();

    protected NotFoundError NotFound(string code = NOT_FOUND, string? message = null, IEnumerable<NamedError>? errors = null)
        => RamshaResults.NotFound(code, message, errors);

    protected InvalidError Invalid()
    => RamshaResults.Invalid();

    protected InvalidError Invalid(string code = INVALID, string? message = null, IEnumerable<NamedError>? errors = null)
        => RamshaResults.Invalid(code, message, errors);


    protected UnauthenticatedError Unauthenticated()
        => RamshaResults.Unauthenticated();

    protected UnauthenticatedError Unauthenticated(string code = UNAUTHENTICATED, string? message = null, IEnumerable<NamedError>? errors = null)
        => RamshaResults.Unauthenticated(code, message, errors);

    protected ForbiddenError Forbidden()
        => RamshaResults.Forbidden();

    protected ForbiddenError Forbidden(string code = FORBIDDEN, string? message = null, IEnumerable<NamedError>? errors = null)
        => RamshaResults.Forbidden(code, message, errors);

    protected InternalError InternalError()
        => RamshaResults.InternalError();

    protected InternalError InternalError(string code = INTERNAL_ERROR, string? message = null, IEnumerable<NamedError>? errors = null)
        => RamshaResults.InternalError(code, message, errors);
}

