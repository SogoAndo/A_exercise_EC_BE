using System.IdentityModel.Tokens.Jwt;
using System.Text;
using A_exercise_EC_BE.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace A_exercise_EC_BE.Infrastructure.Tests.Security;

[TestClass]
[TestCategory("Infrastructure/Security")]
public class CustomerJwtTokenIssuerTests
{
    private const string Issuer = "https://ec.example.test";
    private const string Audience = "https://ec.example.test";
    private const string SigningKey =
        "customer-only-signing-key-0123456789-abcdef";

    [TestMethod]
    public void Issue_WithValidOptions_ReturnsCustomerJwt()
    {
        var customerUuid = Guid.NewGuid();
        var issuedAt = DateTimeOffset.UtcNow;
        var issuer = CreateIssuer(
            CreateOptions(),
            new FixedTimeProvider(issuedAt));

        var result = issuer.Issue(customerUuid);

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.AccessToken));
        Assert.AreEqual(issuedAt.AddMinutes(30), result.ExpiresAt);

        var handler = new JwtSecurityTokenHandler
        {
            MapInboundClaims = false
        };
        var principal = handler.ValidateToken(
            result.AccessToken,
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(SigningKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            },
            out var validatedToken);

        Assert.AreEqual(
            customerUuid.ToString("D"),
            principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value);
        Assert.IsInstanceOfType<JwtSecurityToken>(validatedToken);
        Assert.AreEqual(
            SecurityAlgorithms.HmacSha256,
            ((JwtSecurityToken)validatedToken).Header.Alg);
    }

    [TestMethod]
    [DataRow("Issuer")]
    [DataRow("Audience")]
    [DataRow("SigningKey")]
    public void Constructor_WithMissingRequiredOption_ThrowsInvalidOperationException(
        string missingOption)
    {
        var options = missingOption switch
        {
            "Issuer" => CreateOptions(issuer: string.Empty),
            "Audience" => CreateOptions(audience: string.Empty),
            "SigningKey" => CreateOptions(signingKey: string.Empty),
            _ => throw new ArgumentOutOfRangeException(nameof(missingOption))
        };

        Assert.ThrowsExactly<InvalidOperationException>(
            () => CreateIssuer(options));
    }

    [TestMethod]
    public void Constructor_WithShortSigningKey_ThrowsInvalidOperationException()
    {
        var options = CreateOptions(signingKey: "short-signing-key");

        Assert.ThrowsExactly<InvalidOperationException>(
            () => CreateIssuer(options));
    }

    [TestMethod]
    public void Issue_WithEmptyCustomerUuid_ThrowsArgumentException()
    {
        var issuer = CreateIssuer(CreateOptions());

        Assert.ThrowsExactly<ArgumentException>(
            () => issuer.Issue(Guid.Empty));
    }

    private static CustomerJwtTokenIssuer CreateIssuer(
        CustomerJwtOptions options,
        TimeProvider? timeProvider = null) =>
        new(Options.Create(options), timeProvider);

    private static CustomerJwtOptions CreateOptions(
        string issuer = Issuer,
        string audience = Audience,
        string signingKey = SigningKey) => new()
    {
        Issuer = issuer,
        Audience = audience,
        SigningKey = signingKey
    };

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
