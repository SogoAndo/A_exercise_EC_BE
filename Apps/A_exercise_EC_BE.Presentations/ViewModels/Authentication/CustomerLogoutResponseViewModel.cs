namespace A_exercise_EC_BE.Presentations.ViewModels.Authentication;

/// <summary>
/// UC008 顧客ログアウト成功時の応答。
/// </summary>
public sealed record CustomerLogoutResponseViewModel(
    bool LoggedOut);
