namespace Ramsha.Localization.Abstractions;

public class ResourceNameAttribute : Attribute
{
    public string Name { get; }

    public ResourceNameAttribute(string name)
    {
        Name = name;
    }

    public static ResourceNameAttribute? FindAttribute(Type resourceType)
    {
        return resourceType
            .GetCustomAttributes(true)
            .OfType<ResourceNameAttribute>()
            .FirstOrDefault();
    }

    public static string GetName(Type resourceType)
    {
        return (FindAttribute(resourceType)?.Name ?? resourceType.FullName)!;
    }
}
