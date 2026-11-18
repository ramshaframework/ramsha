

namespace Ramsha.Permissions.Contracts;

public record PermissionDefinitionDto(string Name, string GroupName, string? Description, List<string> Providers);



