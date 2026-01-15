namespace Ramsha.Identity.Contracts;

public class RamshaIdentityContractsOptions
{
    public Type KeyType { get; set; }
    public Dictionary<Type, Type?> ReplacedDtosTypes { get; } = [];

    public Type? ReplacedUserServiceType { get; private set; }
    public Type? ReplacedRoleServiceType { get; private set; }


    public RamshaIdentityContractsOptions ReplaceDto<TBaseDto, TTargetDto>()
    {
        ReplaceDto(typeof(TBaseDto), typeof(TTargetDto));
        return this;
    }

    public RamshaIdentityContractsOptions ReplaceDto(Type baseDtoType, Type targetDtoType)
    {
        ReplacedDtosTypes[baseDtoType] = targetDtoType;
        return this;
    }

    public Type GetReplacedDtoOrBase<TBaseDto>()
    {
        return GetReplacedDtoOrBase(typeof(TBaseDto));
    }

    public Type GetReplacedDtoOrBase(Type baseDto)
    {
        return ReplacedDtosTypes.ContainsKey(baseDto) ? ReplacedDtosTypes[baseDto] ?? baseDto : baseDto;
    }

    public RamshaIdentityContractsOptions ReplaceUserService<TService>()
    where TService : IRamshaIdentityUserServiceBase
    {
        ReplacedUserServiceType = typeof(TService);
        return this;
    }

    public RamshaIdentityContractsOptions ReplaceRoleService<T>()
    {
        ReplacedUserServiceType = typeof(T);
        return this;
    }


}
