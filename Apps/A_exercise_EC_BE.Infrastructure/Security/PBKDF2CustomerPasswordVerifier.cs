using A_exercise_EC_BE.Application.Security;
using A_exercise_EC_BE.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace A_exercise_EC_BE.Infrastructure.Security;

/// <summary>
/// ASP.NET Core Identity V3形式のPBKDF2ハッシュで顧客パスワードを検証する。
/// </summary>
public sealed class PBKDF2CustomerPasswordVerifier : ICustomerPasswordVerifier
{
    private readonly IPasswordHasher<CustomerPasswordContext> _passwordHasher;
    private readonly string _dummyPasswordHash;

    public PBKDF2CustomerPasswordVerifier(
        IPasswordHasher<CustomerPasswordContext> passwordHasher)
    {
        _passwordHasher = passwordHasher
            ?? throw new ArgumentNullException(nameof(passwordHasher));
        _dummyPasswordHash = _passwordHasher.HashPassword(
            new CustomerPasswordContext(),
            Guid.NewGuid().ToString("N"));
    }

    public bool Verify(string? passwordHash, string providedPassword)
    {
        if (passwordHash is not null && string.IsNullOrWhiteSpace(passwordHash))
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
            result = _passwordHasher.VerifyHashedPassword(
                new CustomerPasswordContext(),
                passwordHash ?? _dummyPasswordHash,
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
