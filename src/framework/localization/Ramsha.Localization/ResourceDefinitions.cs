using Ramsha.Localization.Abstractions;

namespace Ramsha.Localization;

public class ResourceDefinitions : Dictionary<string, ResourceDefinition>
{


    public ResourceDefinition Get(string resourceName)
    {
        if (string.IsNullOrWhiteSpace(resourceName))
            throw new ArgumentException("Resource name cannot be null or empty", nameof(resourceName));

        if (!TryGetValue(resourceName, out var resource))
            throw new KeyNotFoundException($"Resource '{resourceName}' not found ");

        return resource;
    }

    public ResourceDefinition Get<ResourceType>()
    {
        var resourceName = ResourceNameAttribute.GetName(typeof(ResourceType));
        return Get(resourceName);
    }
    public ResourceDefinition Add(string resourceName)
    {
        if (string.IsNullOrWhiteSpace(resourceName))
            throw new ArgumentException("Resource name cannot be null or empty", nameof(resourceName));

        if (ContainsKey(resourceName))
            throw new InvalidOperationException($"Resource '{resourceName}' already exists");

        var resource = new ResourceDefinition(resourceName);

        this[resourceName] = resource;
        return resource;
    }

    public ResourceDefinition Add<ResourceType>()
    {
        var resourceName = ResourceNameAttribute.GetName(typeof(ResourceType));
        return Add(resourceName);
    }
}
