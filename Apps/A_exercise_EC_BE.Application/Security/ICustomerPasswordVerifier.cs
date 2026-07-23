namespace A_exercise_EC_BE.Application.Security;

/// <summary>
/// UC002で顧客パスワードを検証するためのインターフェース。
/// </summary>
public interface ICustomerPasswordVerifier
{
    bool Verify(string passwordHash, string providedPassword);
}
