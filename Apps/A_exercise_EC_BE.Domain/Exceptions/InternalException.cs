namespace A_exercise_EC_BE.Domain.Exceptions;
/// <summary>
/// 内部エラーを表す例外クラス
/// </summary>
public class InternalException : Exception
{
    /// <summary>
    /// エラーメッセージだけをもつ例外クラス
    /// </summary>
    /// <param name="message">例外メッセージ</param>
    public InternalException(string message) : base(message) { }
    /// <summary>
    /// エラーメッセージ + 元の例外（原因）も一緒に持つ例外
    /// </summary>
    /// <param name="message">例外メッセージ</param>
    /// <param name="innerException">元の例外</param>
    public InternalException(string message, Exception innerException) : base(message, innerException) { }
}