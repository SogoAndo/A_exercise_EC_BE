namespace A_exercise_EC_BE.Domain.Exceptions;

/// <summary>
/// アプリケーション内部の処理失敗を表す例外。
/// </summary>
public class InternalException : Exception
{
    public InternalException(string message) : base(message)
    {
    }

    public InternalException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
