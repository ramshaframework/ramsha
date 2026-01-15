using Microsoft.AspNetCore.Mvc;
using Ramsha.AspNetCore.Mvc;
using Ramsha.JwtAuth.Domain;
using Ramsha.JwtAuth.Shared;

namespace Ramsha.JwtAuth.Api;

[ControllerName("jwt-auth")]
public class RamshaJwtAuthController<TJwtAuthRequest, TJwtRefreshRequest, TJwtAuthResponse>(
    IJwtAuthAuthenticator<TJwtAuthRequest, TJwtRefreshRequest, TJwtAuthResponse> service)
     : RamshaApiController
where TJwtAuthRequest : RamshaJwtAuthenticateRequest, new()
where TJwtAuthResponse : RamshaJwtAuthResponse, new()
where TJwtRefreshRequest : RamshaJwtRefreshRequest, new()
{
    [HttpPost("authenticate")]
    public async Task<ActionResult<TJwtAuthResponse>> AuthenticateAsync(TJwtAuthRequest request)
     => RamshaResult(await UnitOfWork(() => service.AuthenticateAsync(request)));

    [HttpPost("refresh")]
    public async Task<ActionResult<TJwtAuthResponse>> RefreshAsync(TJwtRefreshRequest request)
     => RamshaResult(await UnitOfWork(() => service.RefreshAsync(request)));

    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeAsync(string? refreshToken = null, bool autoSignout = true)
     => RamshaResult(await UnitOfWork(() => service.RevokeAsync(refreshToken, autoSignout)));
}
