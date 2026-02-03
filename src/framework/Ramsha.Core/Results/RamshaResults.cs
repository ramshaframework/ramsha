using static Ramsha.RamshaErrorsCodes;

namespace Ramsha;

public class RamshaResults
{
    public static SuccessResult Success() => new();
    public static NoContentResult NoContent => new();

    public static PagedResult<T> Paged<T>(List<T> value,RamshaPagedInfo info)
    => new (value,info);
    public static SuccessResult<TValue> Success<TValue>(TValue value) => new(value);

    public static AcceptedResult Accepted(JobInfo job) => new(job);

    public static RamshaErrorResult Error(
        RamshaResultStatus status,
        string? message = null,
        string code = ERROR,
        RamshaErrorContext? context = null,
        IEnumerable<NamedError>? errors = null
    ) =>
        ((ushort)status) is < 400 or >= 2000
            ? throw new ArgumentOutOfRangeException(nameof(status))
            : new(Status: status, Message: message, Code: code, Context: context, Errors: errors);

    public static InvalidError Invalid(
        string code = INVALID,
        string? message = null,
        IEnumerable<NamedError>? errors = null) => new(code, message, errors);

    public static UnauthenticatedError Unauthenticated(
        string code = UNAUTHENTICATED,
        string? message = null,
        IEnumerable<NamedError>? errors = null) => new(code, message, errors);

    public static ForbiddenError Forbidden(
        string code = FORBIDDEN,
        string? message = null,
        IEnumerable<NamedError>? errors = null) => new(code, message, errors);

    public static NotFoundError NotFound(
        string code = NOT_FOUND,
        string? message = null,
        IEnumerable<NamedError>? errors = null) => new(code, message, errors);

    public static InternalError InternalError(
      string code = INTERNAL_ERROR,
      string? message = null,
      IEnumerable<NamedError>? errors = null) => new(code, message, errors);



}



