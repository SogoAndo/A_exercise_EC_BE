namespace A_exercise_EC_BE.Infrastructure.Security;

/// <summary>
/// 顧客認証用JWTの設定。
/// </summary>
public sealed class CustomerJwtOptions
{
    public const string SectionName = "CustomerJwt";

    public string Issuer { get; init; } = string.Empty;

    public string Audience { get; init; } = string.Empty;

    public string SigningKey { get; init; } = string.Empty;
}
