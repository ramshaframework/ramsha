namespace Ramsha;

public record RamshaResult(RamshaResultStatus Status)
: IRamshaResult
{
    public virtual RamshaErrorResult? Error
    => null;

    public virtual bool Succeeded
     => (int)Status < 400 || (int)Status >= 2000;

}

public record RamshaResult<T>(RamshaResultStatus Status) : RamshaResult(Status)
{
    public virtual T? Value => default;

    public static implicit operator RamshaResult<T>(T value)
    => new SuccessResult<T>(value);

    public static implicit operator RamshaResult<T>(RamshaErrorResult? error)
    =>
     error is not null ? new RamshaErrorResult<T>(error.Status, error.Code, error.Message, error.Errors, error.Context)
    : new RamshaErrorResult<T>(RamshaResultStatus.Unknown);


}




