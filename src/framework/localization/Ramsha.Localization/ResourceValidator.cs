using Microsoft.Extensions.Logging;

namespace Ramsha.Localization;

public static class ResourceValidator
{
    public static bool ValidateResources(RamshaLocalizationOptions options, ILogger logger = null)
    {
        if (options?.Resources == null)
        {
            logger?.LogError("Localization options or resources are null");
            return false;
        }

        var allResources = options.Resources;
        var hasErrors = false;

        foreach (var resource in allResources.Values)
        {
            if (HasCircularDependencies(resource.Name, allResources, out var cyclePath))
            {
                logger?.LogError("Circular dependency detected for resource '{ResourceName}': {Cycle}",
                    resource.Name, cyclePath);
                hasErrors = true;
            }
        }

        return !hasErrors;
    }

    public static bool HasCircularDependencies(
      string resourceName,
      IDictionary<string, ResourceDefinition> allResources,
      out string cyclePath)
    {
        cyclePath = string.Empty;

        if (!allResources.TryGetValue(resourceName, out var resource))
            return false;

        var visited = new HashSet<string>();
        var path = new List<string>();
        return DetectCycle(resourceName, allResources, visited, path, out cyclePath);
    }

    private static bool DetectCycle(
        string currentResourceName,
        IDictionary<string, ResourceDefinition> allResources,
        HashSet<string> visited,
        List<string> currentPath,
        out string cyclePath)
    {
        cyclePath = string.Empty;

        if (!allResources.TryGetValue(currentResourceName, out var currentResource))
            return false;

        if (currentPath.Contains(currentResourceName))
        {
            var cycleIndex = currentPath.IndexOf(currentResourceName);
            var cycleNodes = currentPath.Skip(cycleIndex).Append(currentResourceName).ToList();
            cyclePath = string.Join(" -> ", cycleNodes);
            return true;
        }

        if (visited.Contains(currentResourceName))
            return false;

        currentPath.Add(currentResourceName);

        if (currentResource.Extends != null)
        {
            foreach (var extendedResourceName in currentResource.Extends)
            {
                if (DetectCycle(extendedResourceName, allResources, visited, currentPath, out cyclePath))
                {
                    return true;
                }
            }
        }

        currentPath.RemoveAt(currentPath.Count - 1);
        visited.Add(currentResourceName);

        return false;
    }


}
