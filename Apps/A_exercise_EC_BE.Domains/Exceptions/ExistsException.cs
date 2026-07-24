namespace A_exercise_EC_BE.Domain.Exceptions;
/// <summary>
/// データが既に存在するエラーを表す例外クラス
/// </summary>
public class ExistsException : Exception
{
    /// <summary>
    /// エラーメッセージだけをもつ例外クラス
    /// </summary>
    /// <param name="message">例外メッセージ</param>
    public ExistsException(string message) : base(message) { }
    /// <summary>
    /// エラーメッセージ + 元の例外（原因）も一緒に持つ例外
    /// </summary>
    /// <param name="message">例外メッセージ</param>
    /// <param name="innerException">元の例外</param>
    public ExistsException(string message, Exception innerException) : base(message, innerException) { }
}