
namespace Ramsha.Files;

public class RamshaFilesOptions
{
    public string RootFilesPath { get; set; } = "Files";
    public string PublicRootFilesPath { get; set; }
    public int BufferSize { get; set; } = 81920;

}
