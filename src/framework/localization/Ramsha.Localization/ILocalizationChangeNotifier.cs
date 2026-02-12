
namespace Ramsha.Localization;


public interface ILocalizationChangeNotifier
{
    Task OnChangeAsync(IReadOnlyList<ResourceDefinition> resourceHierarchy, Func<Task> action);
}