

using Ramsha.Core;

namespace Ramsha.Identity.Contracts;

public interface IRamshaIdentityUserService<TDto, TCreateDto, TUpdateDto, TId> : IRamshaIdentityUserServiceBase
where TDto : RamshaIdentityUserDto
where TCreateDto : CreateRamshaIdentityUserDto
where TUpdateDto : UpdateRamshaIdentityUserDto
{
    Task<RamshaResult<TDto>> Get(TId id);
    Task<RamshaResult<List<TDto>>> GetList(TId id);
    Task<RamshaResult<string>> Create(TCreateDto createDto);
    Task<IRamshaResult> Update(TId id, TUpdateDto updateDto);
    Task<IRamshaResult> Delete(TId id);
    Task<IRamshaResult> AddToRoleAsync(TId id, string roleName);
    Task<IRamshaResult> ChangePasswordAsync(TId id, string oldPassword, string newPassword);
    Task<RamshaResult<List<string>>> GetRolesAsync(TId id);
    Task<IRamshaResult> RemoveFromRoleAsync(TId id, string roleName);
    Task<IRamshaResult> SetEmailAsync(TId id, string email);
    Task<IRamshaResult> SetLockoutAsync(TId id, DateTimeOffset? end);
    Task<IRamshaResult> SetPasswordAsync(TId id, string newPassword);
    Task<IRamshaResult> SetRolesAsync(TId id, IEnumerable<string> roleNames);
    Task<IRamshaResult> SetUserNameAsync(TId id, string username);
    Task<IRamshaResult> UnlockAsync(TId id);
}

public interface IRamshaIdentityUserServiceBase;


