namespace A_exercise_EC_BE.Applications.Usecases.Customers;

/// <summary>
/// UC008 顧客ログアウト結果。
/// </summary>
public sealed record CustomerLogoutResult(bool LoggedOut)
{
    /// <summary>
    /// ログアウト成功結果を生成する。
    /// </summary>
    /// <returns>ログアウト済みを表す結果。</returns>
    public static CustomerLogoutResult CreateLoggedOut()
        => new(true);
}
