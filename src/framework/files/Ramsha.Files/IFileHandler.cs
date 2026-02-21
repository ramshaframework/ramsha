namespace Ramsha.Files;

public interface IFileStorage
{
    Task<RamshaResult<FileStoreResponse>> SaveAsync(
        Stream stream,
       FileStoreInfo storeInfo,
        CancellationToken cancellationToken = default);

    Task<Stream?> ReadAsync(RamshaFileInfo fileInfo, CancellationToken cancellationToken = default);

    Task<IRamshaResult> DeleteAsync(RamshaFileInfo fileInfo, CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(RamshaFileInfo fileInfo, CancellationToken cancellationToken = default);
}

public interface IFileHandler
{
    Task<RamshaResult<FileStoreResponse>> SaveAsync(Stream stream, FileStoreInfo storeInfo, CancellationToken cancellationToken = default);
    Task<Stream?> GetAsync(RamshaFileInfo fileInfo, CancellationToken cancellationToken = default);
    Task<IRamshaResult> DeleteAsync(RamshaFileInfo fileInfo, CancellationToken cancellationToken = default);
}
