
namespace Ramsha.Localization;


public interface ILocalizationChangeNotifier
{
    Task OnChangeAsync(ResourceDefinition resource, Func<Task> action);
}