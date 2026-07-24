namespace A_exercise_EC_BE.Domains.Exceptions;
/// <summary>
/// データが存在しないエラーを表す例外クラス
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// エラーメッセージだけをもつ例外クラス
    /// </summary>
    /// <param name="message">例外メッセージ</param>
    public NotFoundException(string message) : base(message) { }
    /// <summary>
    /// エラーメッセージ + 元の例外（原因）も一緒に持つ例外
    /// </summary>
    /// <param name="message">例外メッセージ</param>
    /// <param name="innerException">元の例外</param>
    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}