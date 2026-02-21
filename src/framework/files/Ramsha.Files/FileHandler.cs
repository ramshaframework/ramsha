namespace Ramsha.Files;

public class FileHandler(IFileStorage storage) : IFileHandler
{
    public async Task<IRamshaResult> DeleteAsync(RamshaFileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        return await storage.DeleteAsync(fileInfo, cancellationToken);
    }

    public async Task<Stream?> GetAsync(RamshaFileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        return await storage.ReadAsync(fileInfo, cancellationToken);
    }

    public async Task<RamshaResult<FileStoreResponse>> SaveAsync(Stream stream, FileStoreInfo storeInfo, CancellationToken cancellationToken = default)
    {
        return await storage.SaveAsync(stream, storeInfo, cancellationToken);
    }
}
