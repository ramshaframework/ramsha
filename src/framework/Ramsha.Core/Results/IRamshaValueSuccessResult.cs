namespace Ramsha;

public interface IRamshaValueSuccessResult : IRamshaSuccessResult
{
    object Value { get; }
}


public interface IRamshaPagedResult : IRamshaValueSuccessResult
{
RamshaPagedInfo PagedInfo {get;}
}

public record RamshaPagedInfo(int Total,int PageSize,int PageNumber);