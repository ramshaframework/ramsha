using Microsoft.Extensions.DependencyInjection;
using Ramsha.UnitOfWork.Abstractions;

namespace Ramsha.UnitOfWork;

public class UnitOfWorkManager(ICurrentUnitOfWork ambientUnitOfWork, IServiceScopeFactory scopeFactory) : IUnitOfWorkManager
{
    public IUnitOfWork? Current => ambientUnitOfWork.GetActive();



    public IUnitOfWork Begin(UnitOfWorkOptions options, bool requiresNew = false)
    {
        var currentUow = Current;
        if (currentUow != null && !requiresNew)
        {
            return new ChildUnitOfWork(currentUow);
        }

        return CreateNewUnitOfWork(uow => uow.Initialize(options));

    }

    private IUnitOfWork CreateNewUnitOfWork(Action<IUnitOfWork> configure)
    {

        var outer = ambientUnitOfWork.UnitOfWork;
        var scope = scopeFactory.CreateScope();

        try
        {
            var uow = scope.ServiceProvider.GetRequiredService<UnitOfWork>();

            uow.SetOuter(outer);
            configure(uow);

            uow.ReleaseScope = () =>
            {
                try
                {
                    if (ReferenceEquals(ambientUnitOfWork.UnitOfWork, uow))
                    {
                        ambientUnitOfWork.SetUnitOfWork(outer);
                    }
                }
                finally
                {
                    scope.Dispose();
                }
            };

            ambientUnitOfWork.SetUnitOfWork(uow);
            return uow;
        }
        catch
        {
            scope.Dispose();
            throw;
        }


    }

    public IUnitOfWork Reserve(string reservationName, bool requiresNew = false)
    {

        if (!requiresNew &&
            ambientUnitOfWork.UnitOfWork != null &&
            ambientUnitOfWork.UnitOfWork.IsReservedFor(reservationName))
        {
            return new ChildUnitOfWork(ambientUnitOfWork.UnitOfWork);
        }

        return CreateNewUnitOfWork(uow => uow.Reserve(reservationName));
    }

    public void BeginReserved(string reservationName, UnitOfWorkOptions options)
    {
        if (!TryBeginReserved(reservationName, options))
        {
            throw new Exception($"Could not find a reserved unit of work with reservation name: {reservationName}");
        }
    }

    public bool TryBeginReserved(string reservationName, UnitOfWorkOptions options)
    {

        var uow = ambientUnitOfWork.UnitOfWork;

        while (uow != null && !uow.IsReservedFor(reservationName))
        {
            uow = uow.Outer;
        }

        if (uow == null)
        {
            return false;
        }

        uow.Initialize(options);

        return true;
    }
}
