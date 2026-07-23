namespace A_exercise_EC_BE.Application.Usecases.Customers;

/// <summary>
/// UC002 顧客ログインの入力値。
/// </summary>
public sealed record CustomerLoginRequest(
    string MailAddress,
    string Password);
