
using Microsoft.Extensions.Options;

namespace Ramsha.Files;

public class LocalFileStorage(IOptions<RamshaFilesOptions> options) : IFileStorage
{
    private readonly RamshaFilesOptions _options = options.Value;
    public async Task<RamshaResult<FileStoreResponse>> SaveAsync(Stream stream, FileStoreInfo storeInfo, CancellationToken cancellationToken = default)
    {
        var relativePath = string.IsNullOrWhiteSpace(storeInfo.Directory)
            ? Path.GetFileName(storeInfo.FileName)
            : Path.Combine(storeInfo.Directory, Path.GetFileName(storeInfo.FileName));

        var fullPath = BuildFullPath(relativePath, storeInfo.IsPublic);

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

        if (!storeInfo.Overwrite && File.Exists(fullPath))
            return RamshaResults.Invalid("File already exists");

        await using var fileStream = new FileStream(
            fullPath,
            storeInfo.Overwrite ? FileMode.Create : FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            _options.BufferSize,
            true);

        await stream.CopyToAsync(fileStream, cancellationToken);

        return new FileStoreResponse(
            new RamshaFileInfo(relativePath, storeInfo.IsPublic),
            null);

    }
    public Task<IRamshaResult> DeleteAsync(RamshaFileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            var fullPath = BuildFullPath(fileInfo.RelativePath, fileInfo.IsPublic);

            if (!File.Exists(fullPath))
                return Task.FromResult<IRamshaResult>(RamshaResults.NotFound());

            File.Delete(fullPath);

            return Task.FromResult<IRamshaResult>(RamshaResults.Success());
        }
        catch (Exception ex)
        {
            return Task.FromResult<IRamshaResult>(RamshaResults.InternalError(ex.Message));
        }
    }

    public Task<bool> ExistsAsync(RamshaFileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        var fullPath = BuildFullPath(fileInfo.RelativePath, fileInfo.IsPublic);
        return Task.FromResult(File.Exists(fullPath));
    }

    public Task<Stream?> ReadAsync(RamshaFileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        var fullPath = BuildFullPath(fileInfo.RelativePath, fileInfo.IsPublic);

        if (!File.Exists(fullPath))
            return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            _options.BufferSize,
            true);

        return Task.FromResult<Stream?>(stream);
    }

    private string BuildFullPath(string relativePath, bool isPublic)
    {
        var root = Path.GetFullPath(isPublic
       ? _options.PublicRootFilesPath
       : _options.RootFilesPath);

        var full = Path.GetFullPath(Path.Combine(root, relativePath));

        if (!full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid file path");

        return full;
    }


}
