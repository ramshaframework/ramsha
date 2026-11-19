using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Ramsha.UnitOfWork.Abstractions;

namespace Ramsha.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IServiceProvider _serviceProvider;
    private readonly List<UoWLocalEvent> _localEvents = new();
    internal Action? ReleaseScope { get; set; }
    private bool _isDispatching;

    public void EnqueueLocalEvent(UoWLocalEvent @event)
    {
        if (@event == null) throw new ArgumentNullException(nameof(@event));
        _localEvents.Add(@event);
    }

    public IReadOnlyList<object> GetLocalEvents() => _localEvents.AsReadOnly();
    public void ClearLocalEvents() => _localEvents.Clear();
    public UnitOfWork(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        Items = [];
        _databaseApis = [];
        _transactionApis = [];

    }
    public Guid Id => Guid.NewGuid();

    public IServiceProvider ServiceProvider => _serviceProvider;

    public const string UnitOfWorkReservationName = RamshaUnitOfWorkReservationNames.ActionUnitOfWorkReservationName;

    public Dictionary<string, object> Items { get; }
    protected List<Func<Task>> CompletedHandlers { get; } = new List<Func<Task>>();
    public IUnitOfWorkOptions Options { get; private set; } = default!;
    public IUnitOfWork? Outer { get; private set; }
    public virtual void SetOuter(IUnitOfWork? outer)
    {
        Outer = outer;
    }
    public bool IsReserved { get; set; }

    public bool IsDisposed { get; private set; }

    public bool IsCompleted { get; private set; }


    public string? ReservationName { get; set; }

    private bool _isCompleting;
    private bool _isRolledback;


    private Exception? _exception;

    private readonly Dictionary<string, IDatabaseApi> _databaseApis;
    private readonly Dictionary<string, ITransactionApi> _transactionApis;

    public virtual void AddDatabaseApi(string key, IDatabaseApi api)
    {
        if (_databaseApis.ContainsKey(key))
        {
            throw new Exception("This unit of work already contains a database API for the given key.");
        }

        _databaseApis.Add(key, api);
    }


    public virtual IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory)
    {
        if (_databaseApis.TryGetValue(key, out var existing))
            return existing;

        return _databaseApis[key] = factory();
    }

    public virtual ITransactionApi? FindTransactionApi(string key)
    {
        return _transactionApis.TryGetValue(key, out var existing) ? existing : default;
    }

    public virtual void AddTransactionApi(string key, ITransactionApi api)
    {
        if (_transactionApis.ContainsKey(key))
        {
            throw new Exception("This unit of work already contains a transaction API for the given key.");
        }

        _transactionApis.Add(key, api);
    }

    public IDatabaseApi? FindDatabaseApi(string key)
    {
        return _databaseApis.TryGetValue(key, out var existing) ? existing : default;
    }

    public ITransactionApi GetOrAddTransactionApi(string key, Func<ITransactionApi> factory)
    {
        if (_transactionApis.TryGetValue(key, out var existing))
            return existing;

        return _transactionApis[key] = factory();
    }




    public virtual void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        var release = ReleaseScope;
        ReleaseScope = null;

        try
        {
            DisposeTransactions();

        }
        finally
        {
            release?.Invoke();
        }

    }

    private void DisposeTransactions()
    {
        foreach (var transactionApi in GetAllActiveTransactionApis())
        {
            try
            {
                transactionApi.Dispose();
            }
            catch
            {

            }
        }
    }



    public void Reserve(string reservationName)
    {
        ReservationName = reservationName;
        IsReserved = true;
    }


    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_isRolledback)
        {
            return;
        }

        foreach (var databaseApi in GetAllActiveDatabaseApis())
        {
            if (databaseApi is ISupportsSavingChanges supportsSavingChangesDatabaseApi)
            {
                await supportsSavingChangesDatabaseApi.SaveChangesAsync(cancellationToken);
            }
        }
    }

    public virtual IReadOnlyList<IDatabaseApi> GetAllActiveDatabaseApis()
    {
        return _databaseApis.Values.ToImmutableList();
    }

    public virtual IReadOnlyList<ITransactionApi> GetAllActiveTransactionApis()
    {
        return _transactionApis.Values.ToImmutableList();
    }

    public virtual async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        if (_isRolledback)
        {
            return;
        }

        PreventMultipleComplete();

        try
        {
            //
            _isCompleting = true;
            await SaveChangesAsync(cancellationToken);

            if (_localEvents.Any())
            {
                var publisher = _serviceProvider.GetRequiredService<IUnitOfWorkLocalEventBus>();
                _isDispatching = true;
                await publisher.PublishMany(_localEvents, cancellationToken).ConfigureAwait(false);
                _isDispatching = false;

                // if handlers added entities
                await SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            //



            await CommitTransactionsAsync(cancellationToken);
            IsCompleted = true;
        }
        catch (Exception ex)
        {
            _exception = ex;
            throw;
        }
    }


    public virtual void OnCompleted(Func<Task> handler)
    {
        CompletedHandlers.Add(handler);
    }

    protected virtual async Task OnCompletedAsync()
    {
        foreach (var handler in CompletedHandlers)
        {
            await handler.Invoke();
        }
    }

    private void PreventMultipleComplete()
    {
        if (IsCompleted || _isCompleting)
        {
            throw new Exception("Completion has already been requested for this unit of work.");
        }
    }

    protected virtual async Task CommitTransactionsAsync(CancellationToken cancellationToken)
    {
        foreach (var transaction in GetAllActiveTransactionApis())
        {
            await transaction.CommitAsync(cancellationToken);
        }
    }

    public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_isRolledback)
        {
            return;
        }

        _isRolledback = true;

        await RollbackAllAsync(cancellationToken);
    }

    protected virtual async Task RollbackAllAsync(CancellationToken cancellationToken)
    {
        foreach (var databaseApi in GetAllActiveDatabaseApis())
        {
            if (databaseApi is ISupportsRollback supportsRollbackDatabaseApi)
            {
                try
                {
                    await supportsRollbackDatabaseApi.RollbackAsync(cancellationToken);
                }
                catch { }
            }
        }

        foreach (var transactionApi in GetAllActiveTransactionApis())
        {
            if (transactionApi is ISupportsRollback supportsRollbackTransactionApi)
            {
                try
                {
                    await supportsRollbackTransactionApi.RollbackAsync(cancellationToken);
                }
                catch { }
            }
        }
    }

    public void Initialize(UnitOfWorkOptions options)
    {
        if (Options != null)
        {
            throw new Exception("This unit of work has already been initialized.");
        }
        Options = options;
        IsReserved = false;
    }
}
