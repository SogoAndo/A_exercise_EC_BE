using System.Text;

namespace A_exercise_EC_BE.Infrastructure.Security;

/// <summary>
/// 顧客認証用JWTの設定。
/// </summary>
public sealed class CustomerJwtOptions
{
    private const int MinimumSigningKeyBytes = 32;

    public const string SectionName = "CustomerJwt";

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string SigningKey { get; init; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Issuer))
        {
            throw new InvalidOperationException(
                "CustomerJwt:Issuerを設定してください。");
        }

        if (string.IsNullOrWhiteSpace(Audience))
        {
            throw new InvalidOperationException(
                "CustomerJwt:Audienceを設定してください。");
        }

        if (string.IsNullOrWhiteSpace(SigningKey))
        {
            throw new InvalidOperationException(
                "CustomerJwt:SigningKeyを設定してください。");
        }

        if (Encoding.UTF8.GetByteCount(SigningKey)
            < MinimumSigningKeyBytes)
        {
            throw new InvalidOperationException(
                "CustomerJwt:SigningKeyは32バイト以上で設定してください。");
        }
    }
}
