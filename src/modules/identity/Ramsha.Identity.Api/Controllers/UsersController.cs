
using Microsoft.AspNetCore.Mvc;
using Ramsha.AspNetCore.Mvc;
using Ramsha.Identity.Contracts;

namespace Ramsha.Identity.Api;

[ControllerName("users")]
public class RamshaIdentityUserController<TDto, TCreateDto, TUpdateDto, TId>(IRamshaIdentityUserService<TDto, TCreateDto, TUpdateDto, TId> userService) : RamshaApiController
where TCreateDto : CreateRamshaIdentityUserDto
where TUpdateDto : UpdateRamshaIdentityUserDto
where TDto : RamshaIdentityUserDto
{

  [HttpGet("{id}")]
  public async Task<ActionResult<TDto>> Get(TId id)
  => RamshaResult(await userService.Get(id));

  [HttpGet]
  public async Task<ActionResult<List<TDto>>> GetList()
  => RamshaResult(await userService.GetList());

  [HttpPost]
  public async Task<ActionResult<string>> Create(TCreateDto createDto)
  => RamshaResult(await userService.Create(createDto));

  [HttpPut("{id}")]
  public async Task<ActionResult> Update(TId id, TUpdateDto updateDto)
   => RamshaResult(await userService.Update(id, updateDto));

  [HttpDelete("{id}")]
  public async Task<ActionResult> Delete(TId id)
    => RamshaResult(await userService.Delete(id));

  // roles
  [HttpPost("{id}/roles/{roleName}")]
  public async Task<ActionResult> AddToRole(TId id, string roleName)
    => RamshaResult(await userService.AddToRoleAsync(id, roleName));


  [HttpDelete("{id}/roles/{roleName}")]
  public async Task<ActionResult> RemoveFromRole(TId id, string roleName)
    => RamshaResult(await userService.RemoveFromRoleAsync(id, roleName));


  [HttpPut("{id}/roles")]
  public async Task<ActionResult> SetRoles(TId id, List<string> roles)
    => RamshaResult(await userService.SetRolesAsync(id, roles));


  [HttpGet("{id}/roles")]
  public async Task<ActionResult<List<string>>> GetRoles(TId id)
    => RamshaResult(await userService.GetRolesAsync(id));


  //password
  [HttpPost("{id}/password/change")]
  public async Task<ActionResult> ChangePassword(TId id, string oldPassword, string newPassword)
   => RamshaResult(await userService.ChangePasswordAsync(id, oldPassword, newPassword));


  [HttpPost("{id}/password/set")]
  public async Task<ActionResult> SetPassword(TId id, string password)
   => RamshaResult(await userService.SetPasswordAsync(id, password));


  //email and user
  [HttpPost("{id}/email/set")]
  public async Task<ActionResult> SetEmail(TId id, string email)
   => RamshaResult(await userService.SetEmailAsync(id, email));

  [HttpPost("{id}/username/set")]
  public async Task<ActionResult> SetUsername(TId id, string userName)
   => RamshaResult(await userService.SetUserNameAsync(id, userName));


  //token
  [HttpPost("{id}/lockout")]
  public async Task<ActionResult> SetLockout(TId id, DateTimeOffset? end)
   => RamshaResult(await userService.SetLockoutAsync(id, end));


  [HttpPost("{id}/unlock")]
  public async Task<ActionResult> Unlock(TId id)
=> RamshaResult(await userService.UnlockAsync(id));


}
