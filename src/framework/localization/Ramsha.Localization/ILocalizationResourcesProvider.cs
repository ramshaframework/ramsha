
namespace Ramsha.Localization
{
    public interface IResourcesDefinitionsProvider
    {
        Task<IReadOnlyList<ResourceDefinition>> GetAllResourcesAsync();
    }
}