using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using A_exercise_EC_BE.Applications.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace A_exercise_EC_BE.Infrastructures.Security;

/// <summary>
/// CustomerJwt設定を使用して顧客認証用JWTを発行する。
/// </summary>
public sealed class CustomerJwtTokenIssuer : ICustomerAccessTokenIssuer
{
    private const int TokenLifetimeMinutes = 30;

    private readonly CustomerJwtOptions _options;
    private readonly TimeProvider _timeProvider;

    public CustomerJwtTokenIssuer(
        IOptions<CustomerJwtOptions> options,
        TimeProvider? timeProvider = null)
    {
        _options = options?.Value
            ?? throw new ArgumentNullException(nameof(options));
        _timeProvider = timeProvider ?? TimeProvider.System;

        _options.Validate();
    }

    public CustomerAccessToken Issue(Guid customerUuid)
    {
        if (customerUuid == Guid.Empty)
        {
            throw new ArgumentException(
                "顧客識別IDを指定してください。",
                nameof(customerUuid));
        }

        var issuedAt = _timeProvider.GetUtcNow();
        var expiresAt = issuedAt.AddMinutes(TokenLifetimeMinutes);
        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SigningKey));
        var signingCredentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                customerUuid.ToString("D"))
        };
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: issuedAt.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingCredentials);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new CustomerAccessToken(accessToken, expiresAt);
    }
}
