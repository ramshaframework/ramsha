namespace Ramsha.AspNetCore;

public interface IRamshaPipelineContributor
{
    void Configure(RamshaPipelineContext context)
    {
    }

    Task ConfigureAsync(RamshaPipelineContext context)
    {
        Configure(context);
        return Task.CompletedTask;
    }
}



