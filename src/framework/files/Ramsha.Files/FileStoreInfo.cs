namespace Ramsha.Files;

public record FileStoreInfo(string FileName, string Directory, bool Overwrite = false, bool IsPublic = false);
