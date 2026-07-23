using A_exercise_EC_BE.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace A_exercise_EC_BE.Application.Security;

/// <summary>
/// ASP.NET Core Identity V3形式のPBKDF2ハッシュで顧客パスワードを検証する。
/// </summary>
public sealed class PBKDF2CustomerPasswordVerifier(
    IPasswordHasher<CustomerPasswordContext> passwordHasher)
    : ICustomerPasswordVerifier
{
    public bool Verify(string passwordHash, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("パスワードハッシュは必須です。");
        }

        if (string.IsNullOrWhiteSpace(providedPassword))
        {
            throw new DomainException("パスワードは必須です。");
        }

        PasswordVerificationResult result;
        try
        {
            result = passwordHasher.VerifyHashedPassword(
                new CustomerPasswordContext(),
                passwordHash,
                providedPassword);
        }
        catch (FormatException)
        {
            return false;
        }

        return result is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
    }
}

/// <summary>
/// 顧客パスワード検証時にPasswordHasherへ渡すコンテキスト。
/// PasswordHasherは現在この値を参照しない。
/// </summary>
public sealed class CustomerPasswordContext;
