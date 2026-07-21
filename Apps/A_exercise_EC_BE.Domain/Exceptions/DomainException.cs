namespace A_exercise_EC_BE.Domain.Exceptions;

/// <summary>
/// ドメインルール違反を表す例外。
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
