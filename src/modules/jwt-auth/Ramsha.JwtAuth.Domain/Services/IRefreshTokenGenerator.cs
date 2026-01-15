

using Ramsha.JwtAuth.Shared;

namespace Ramsha.JwtAuth.Domain;

public interface IRefreshTokenGenerator
{
    RefreshTokenInfo Generate();
}
