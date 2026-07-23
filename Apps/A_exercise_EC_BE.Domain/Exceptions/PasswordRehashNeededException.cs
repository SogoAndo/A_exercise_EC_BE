namespace A_exercise_EC_BE.Domains.Exceptions;
/// <summary>
/// パスワードの再ハッシュが必要な場合にスローされる例外
/// </summary>
public class PasswordRehashNeededException : Exception
{
    /// <summary>
    /// 再ハッシュが必要な場合の例外
    /// </summary>
    public PasswordRehashNeededException() { }

    /// <summary>
    /// エラーメッセージだけをもつ例外クラス
    /// </summary>
    /// <param name="message">例外メッセージ</param>
    public PasswordRehashNeededException(string message)
        : base(message) { }

    /// <summary>
    /// エラーメッセージ + 元の例外（原因）も一緒に持つ例外
    /// </summary>
    /// <param name="message">例外メッセージ</param>
    /// <param name="innerException">元の例外</param>
    public PasswordRehashNeededException(string message, Exception innerException)
        : base(message, innerException) { }
}