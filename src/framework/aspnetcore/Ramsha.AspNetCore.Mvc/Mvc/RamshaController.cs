

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Ramsha.Common.Domain;
using Ramsha.UnitOfWork.Abstractions;

namespace Ramsha.AspNetCore.Mvc;

[Route("[controller]")]
public class RamshaController : Controller
{
    protected IUnitOfWorkManager UnitOfWorkManager => HttpContext.RequestServices.GetLazyRequiredService<IUnitOfWorkManager>().Value;
    protected IGlobalQueryFilterManager GlobalQueryFilterManager => HttpContext.RequestServices.GetLazyRequiredService<IGlobalQueryFilterManager>().Value;

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

}
