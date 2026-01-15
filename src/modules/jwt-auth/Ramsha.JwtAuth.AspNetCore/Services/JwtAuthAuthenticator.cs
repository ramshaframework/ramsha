using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ramsha.AspNetCore;
using Ramsha.Identity.Domain;
using Ramsha.JwtAuth.Domain;
using Ramsha.JwtAuth.Shared;

namespace Ramsha.JwtAuth.AspNetCore;

public class JwtAuthAuthenticator<TUser, TId, TJwtAuthRequest, TJwtRefreshRequest, TJwtAuthResponse>(
    UserManager<TUser> userManager,
    SignInManager<TUser> signInManager,
    RefreshTokenManager refreshTokenManager,
    IRefreshTokenRepository refreshTokenRepository,
    IOptionsSnapshot<RamshaJwTValidationOptions> jwtValidationOptions,
    IOptionsSnapshot<RamshaRefreshTokenOptions> refreshOptions,
    IRamshaCookieService cookieService) : IJwtAuthAuthenticator<TJwtAuthRequest, TJwtRefreshRequest, TJwtAuthResponse>
where TId : IEquatable<TId>
where TUser : RamshaIdentityUserBase<TId>
where TJwtAuthRequest : RamshaJwtAuthenticateRequest, new()
where TJwtAuthResponse : RamshaJwtAuthResponse, new()
where TJwtRefreshRequest : RamshaJwtRefreshRequest, new()
{
    public virtual async Task<IRamshaResult> RevokeAsync(string? refreshToken = null, bool autoSignout = true)
    {
        refreshToken ??= GetRefreshTokenFromCookie();

        if (refreshToken is null)
        {
            return RamshaResults.NotFound(message: "no refreshToken was send");
        }

        var existRefreshToken = await refreshTokenRepository.FindAsync(t => t.Token == refreshToken);
        if (existRefreshToken is null)
        {
            return RamshaResults.NotFound(message: "the refreshToken not exist");
        }

        await refreshTokenManager.RevokeAsync(existRefreshToken);

        if (autoSignout)
        {
            await signInManager.SignOutAsync();
        }

        RemoveRefreshTokenFromCookie();
        return RamshaResults.Success();
    }

    public virtual async Task<RamshaResult<TJwtAuthResponse>> RefreshAsync(TJwtRefreshRequest request)
    {
        request.RefreshToken ??= GetRefreshTokenFromCookie();

        if (request.RefreshToken is null)
        {
            return RamshaResults.NotFound(message: "no refreshToken was send");
        }

        var existRefreshToken = await refreshTokenRepository.FindAsync(x => x.Token == request.RefreshToken);

        if (existRefreshToken is null)
        {
            return RamshaResults.NotFound(message: "the token not exist");
        }

        if (existRefreshToken.IsRevoked)
        {
            return RamshaResults.Invalid(message: "the token is revoked");
        }


        var user = await userManager.FindByIdAsync(existRefreshToken.UserId);

        if (user is null)
            return RamshaResults.NotFound(message: "no user found for this token");

        string finalRefreshTokenValue;
        DateTime finalRefreshTokenExpiration;

        if (existRefreshToken.IsExpired)
        {
            await refreshTokenManager.RevokeAsync(existRefreshToken);
            var newRefreshToken = await refreshTokenManager.GenerateAsync(user.Id.ToString());
            finalRefreshTokenValue = newRefreshToken.Token;
            finalRefreshTokenExpiration = newRefreshToken.ExpiresOn;
        }
        else
        {
            finalRefreshTokenValue = existRefreshToken.Token;
            finalRefreshTokenExpiration = existRefreshToken.ExpiresOn;
        }

        IEnumerable<Claim>? claims;
        if (request.OldAccessToken is not null)
            claims = GetPrincipalFromExpiredToken(request.OldAccessToken).Claims;
        else
            claims = await GetClaimsAsync(user);

        var newAccessToken = GenerateAccessTokenFromClaims(claims);

        var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);

        SetRefreshTokenCookie(finalRefreshTokenValue, finalRefreshTokenExpiration);

        return MapResponseForRefresh(user, newAccessToken, finalRefreshTokenValue, finalRefreshTokenExpiration, rolesList);
    }

    protected virtual void RemoveRefreshTokenFromCookie()
    {
        cookieService.RemoveCookie(RamshaJwtAuthCookiesNames.RefreshToken);
    }

    protected virtual string? GetRefreshTokenFromCookie()
    {
        return cookieService.GetCookieValue(RamshaJwtAuthCookiesNames.RefreshToken);
    }
    protected virtual void SetRefreshTokenCookie(string refreshToken, DateTime refreshTokenExpiration)
    {
        cookieService.SetCookieValue(RamshaJwtAuthCookiesNames.RefreshToken,
           refreshToken,
           new()
           {
               HttpOnly = true,
               Expires = refreshTokenExpiration.ToLocalTime(),
               SameSite = refreshOptions.Value.CookieOptions.SameSite,
               Secure = refreshOptions.Value.CookieOptions.Secure,
           });

    }


    public virtual async Task<RamshaResult<TJwtAuthResponse>> AuthenticateAsync(TJwtAuthRequest request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            return RamshaResults.Invalid(message: "Bad credentials");
        }

        var userId = user.Id.ToString();
        var result = await signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return RamshaResults.Invalid(message: "Bad credentials");
        }

        var rolesList = await userManager.GetRolesAsync(user).ConfigureAwait(false);

        var claimsPrincipal = await signInManager.ClaimsFactory.CreateAsync(user);

        var accessToken = await GenerateJwtTokenFromUser(user);

        var refreshTokens = await refreshTokenRepository.GetAllActiveForUser(userId);

        string finalRefreshToken;
        DateTime finalRefreshTokenExpiration;

        if (refreshTokens.Any())
        {
            var activeRefreshToken = refreshTokens.FirstOrDefault(t => t.IsActive);
            finalRefreshToken = activeRefreshToken.Token;
            finalRefreshTokenExpiration = activeRefreshToken.ExpiresOn;
        }
        else
        {
            var refreshToken = await refreshTokenManager.GenerateAsync(userId);
            finalRefreshToken = refreshToken.Token;
            finalRefreshTokenExpiration = refreshToken.ExpiresOn;
            await userManager.UpdateAsync(user);
        }

        SetRefreshTokenCookie(finalRefreshToken, finalRefreshTokenExpiration);

        return MapResponseForAuthenticate(user, request, accessToken, finalRefreshTokenExpiration, rolesList);
    }




    protected virtual TJwtAuthResponse MapResponseForRefresh(TUser user, string accessToken, string refreshToken, DateTime refreshTokenExpiration, IEnumerable<string> roles)
    {
        return new TJwtAuthResponse()
        {
            UserId = user.Id.ToString(),
            AccessToken = accessToken,
            Email = user.Email,
            Username = user.UserName,
            Role = roles.ToArray(),
            IsVerified = user.EmailConfirmed,
            RefreshTokenExpiration = refreshTokenExpiration
        };
    }

    protected virtual TJwtAuthResponse MapResponseForAuthenticate(TUser user, TJwtAuthRequest request, string accessToken, DateTime refreshTokenExpiration, IEnumerable<string> roles)
    {
        return new TJwtAuthResponse()
        {
            UserId = user.Id.ToString(),
            AccessToken = accessToken,
            Email = user.Email,
            Username = user.UserName,
            Role = roles.ToArray(),
            IsVerified = user.EmailConfirmed,
            RefreshTokenExpiration = refreshTokenExpiration
        };
    }


    protected virtual ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = jwtValidationOptions.Value.Audience,
            ValidateIssuer = true,
            ValidIssuer = jwtValidationOptions.Value.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtValidationOptions.Value.SecurityKey)),
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        return principal;
    }



    protected virtual string GenerateAccessTokenFromClaims(IEnumerable<Claim> claims)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtValidationOptions.Value.SecurityKey));
        var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
        var tokeOptions = new JwtSecurityToken(
            issuer: jwtValidationOptions.Value.Issuer,
            audience: jwtValidationOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtValidationOptions.Value.DurationInMinutes),
            signingCredentials: signinCredentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
        return tokenString;
    }


    protected virtual async Task<string> GenerateJwtTokenFromUser(TUser user)
    {
        await userManager.UpdateSecurityStampAsync(user);
        var claims = await GetClaimsAsync(user);
        return GenerateAccessTokenFromClaims(claims);
    }


    protected virtual async Task<IList<Claim>> GetClaimsAsync(TUser user)
    {
        var result = await signInManager.ClaimsFactory.CreateAsync(user);
        return result.Claims.ToList();
    }
}
