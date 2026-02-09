using Ramsha.Localization.Abstractions;

namespace Ramsha.Localization;

public class ResourceDefinitions : Dictionary<string, ResourceDefinition>
{
    public IReadOnlyList<ResourceDefinition> ResolveResourceHierarchy(
    ResourceDefinition root)
    {
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var ordered = new List<ResourceDefinition>();
        var stack = new Stack<ResourceDefinition>();

        stack.Push(root);

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            if (!visited.Add(current.Name))
            {
                continue;
            }

            ordered.Add(current);

            if (current.Extends.Count > 0)
            {
                for (int i = current.Extends.Count - 1; i >= 0; i--)
                {
                    var name = current.Extends[i];

                    if (TryGetValue(name, out var extended))
                    {
                        stack.Push(extended);
                    }
                }
            }
        }

        ordered.Reverse();

        return ordered;
    }

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
