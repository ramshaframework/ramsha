

namespace Ramsha;

public interface IModuleContainer
{
    IReadOnlyList<IModuleDescriptor> Modules { get; }
}