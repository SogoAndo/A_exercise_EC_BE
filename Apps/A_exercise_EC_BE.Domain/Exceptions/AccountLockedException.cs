namespace A_exercise_EC_BE.Domains.Exceptions;

/// <summary>
/// ログイン失敗回数の上限に達したことを表す例外
/// </summary>
public class AccountLockedException : Exception
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public AccountLockedException(string message) : base(message)
    {
    }
}