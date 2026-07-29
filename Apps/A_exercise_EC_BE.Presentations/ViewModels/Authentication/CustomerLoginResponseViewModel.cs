namespace A_exercise_EC_BE.Presentations.ViewModels.Authentication;

/// <summary>
/// UC002 顧客ログイン成功時の応答。
/// </summary>
public sealed record CustomerLoginResponseViewModel(
    string AccessToken,
    DateTimeOffset ExpiresAt,
    string Username);
