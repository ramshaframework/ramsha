using Microsoft.Extensions.DependencyInjection;
using Ramsha.UnitOfWork.Abstractions;

namespace Ramsha.UnitOfWork;

public class UnitOfWorkManager(ICurrentUnitOfWork ambientUnitOfWork, IServiceProvider serviceProvider) : IUnitOfWorkManager
{
    public IUnitOfWork? Current => ambientUnitOfWork.GetActive();



    public IUnitOfWork Begin(UnitOfWorkOptions options, bool requiresNew = false)
    {
        var currentUow = Current;
        if (currentUow != null && !requiresNew)
        {
            return new ChildUnitOfWork(currentUow);
        }

        var unitOfWork = CreateNewUnitOfWork();
        unitOfWork.Initialize(options);

        return unitOfWork;
    }

    private IUnitOfWork CreateNewUnitOfWork()
    {

        var outerUow = ambientUnitOfWork.UnitOfWork;

        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        unitOfWork.SetOuter(outerUow);

        ambientUnitOfWork.SetUnitOfWork(unitOfWork);

        return unitOfWork;

    }

    public IUnitOfWork Reserve(string reservationName, bool requiresNew = false)
    {

        if (!requiresNew &&
            ambientUnitOfWork.UnitOfWork != null &&
            ambientUnitOfWork.UnitOfWork.IsReservedFor(reservationName))
        {
            return new ChildUnitOfWork(ambientUnitOfWork.UnitOfWork);
        }

        var unitOfWork = CreateNewUnitOfWork();
        unitOfWork.Reserve(reservationName);

        return unitOfWork;
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
