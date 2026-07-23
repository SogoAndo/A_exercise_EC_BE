namespace A_exercise_EC_BE.Application.Security;

/// <summary>
/// 顧客認証用のアクセストークン。
/// </summary>
public sealed record CustomerAccessToken(
    string AccessToken,
    DateTimeOffset ExpiresAt);
