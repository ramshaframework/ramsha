using Ramsha.JwtAuth.Shared;

namespace Ramsha.JwtAuth.Domain;

public interface IJwtAuthAuthenticator<TJwtAuthRequest, TJwtRefreshRequest, TJwtAuthResponse>
where TJwtAuthRequest : RamshaJwtAuthenticateRequest, new()
where TJwtAuthResponse : RamshaJwtAuthResponse, new()
where TJwtRefreshRequest : RamshaJwtRefreshRequest, new()
{
    Task<RamshaResult<TJwtAuthResponse>> AuthenticateAsync(TJwtAuthRequest request);
    Task<RamshaResult<TJwtAuthResponse>> RefreshAsync(TJwtRefreshRequest request);
    Task<IRamshaResult> RevokeAsync(string? refreshToken = null, bool autoSignout = true);
}