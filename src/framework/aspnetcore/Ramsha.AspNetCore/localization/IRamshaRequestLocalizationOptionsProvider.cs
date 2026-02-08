using Microsoft.AspNetCore.Builder;

namespace Ramsha.AspNetCore;

public interface IRamshaRequestLocalizationOptionsProvider
{
    void InitializeOptions(Action<RequestLocalizationOptions>? optionsAction = null);
    Task<RequestLocalizationOptions> GetOptionsAsync();
}
