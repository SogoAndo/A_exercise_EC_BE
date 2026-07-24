namespace A_exercise_EC_BE.Applications.Usecases.Customers;

/// <summary>
/// UC002 顧客ログインの認証結果。
/// </summary>
public sealed record CustomerLoginResult(
    Guid CustomerUuid,
    string Username,
    string CustomerName);
