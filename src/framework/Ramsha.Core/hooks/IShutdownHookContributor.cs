namespace Ramsha;

public interface IShutdownHookContributor
{
    Task OnShutdown(ShutdownContext context);
}
