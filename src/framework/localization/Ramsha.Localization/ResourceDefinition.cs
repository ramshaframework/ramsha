using Ramsha.Localization.Abstractions;

namespace Ramsha.Localization;



public class ResourceDefinition
{
    public string Name { get; private set; }
    public string? Path { get; private set; }
    private readonly List<string> _stores = [];
    public IReadOnlyList<string> Stores => _stores.AsReadOnly();

    private readonly List<string> _extends = [];
    public IReadOnlyList<string> Extends => _extends.AsReadOnly();

    public string GetPathOrDefault(string? prefix = null)
    {
        var path = Path ?? Name.Split('.').Last();
        return prefix is not null ? System.IO.Path.Combine(prefix, path) : path;
    }

    public ResourceDefinition WithStores(List<string> storesNames)
    {
        _stores.Clear();
        _stores.AddRange(storesNames);
        return this;
    }

    public ResourceDefinition AddStore(params string[] storesNames)
    {
        foreach (var name in storesNames)
        {
            AddStore(name);
        }
        return this;
    }


    public ResourceDefinition AddStore(string storeName)
    {
        if (string.IsNullOrWhiteSpace(storeName))
            throw new ArgumentException("store name cannot be null or empty", nameof(storeName));

        if (!_stores.Contains(storeName))
        {
            _stores.Add(storeName);
        }
        return this;
    }

    public ResourceDefinition RemoveStore(params string[] storesNames)
    {
        foreach (var name in storesNames)
        {
            _stores.Remove(name);
        }
        return this;
    }





    public ResourceDefinition SetPath(string path)
    {
        Path = path;
        return this;
    }

    public ResourceDefinition RemoveExtend(params string[] resourceNames)
    {
        foreach (var name in resourceNames)
        {
            _extends.Remove(name);
        }
        return this;
    }


    public ResourceDefinition Extend(string resourceName)
    {
        if (string.IsNullOrWhiteSpace(resourceName))
            throw new ArgumentException("resource name cannot be null or empty", nameof(resourceName));

        if (resourceName == Name)
            throw new InvalidOperationException($"resource '{Name}' cannot extend itself");

        if (!_extends.Contains(resourceName))
        {
            _extends.Add(resourceName);
        }
        return this;
    }
    public ResourceDefinition Extend<TResource>()
    {
        Extend(ResourceNameAttribute.GetName(typeof(TResource)));
        return this;
    }

    public ResourceDefinition Extend(params Type[] resourceTypes)
    {
        foreach (var resourceType in resourceTypes)
        {
            Extend(ResourceNameAttribute.GetName(resourceType));
        }
        return this;
    }

    public ResourceDefinition RemoveExtend(params Type[] resourceTypes)
    {
        foreach (var resourceType in resourceTypes)
        {
            _extends.Remove(ResourceNameAttribute.GetName(resourceType));
        }
        return this;
    }


    internal ResourceDefinition(string name)
    {
        Name = name;
    }


}
