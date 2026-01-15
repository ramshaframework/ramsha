namespace Ramsha;

public interface IInitHookContributor
{
    Task OnInitialize(InitContext context);
}

