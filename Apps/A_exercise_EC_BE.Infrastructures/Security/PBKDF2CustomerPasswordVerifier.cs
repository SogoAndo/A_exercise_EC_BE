using A_exercise_EC_BE.Applications.Security;
using A_exercise_EC_BE.Domains.Exceptions;

namespace A_exercise_EC_BE.Infrastructures.Security;

/// <summary>
/// 顧客アカウント登録と同じPBKDF2形式で顧客パスワードを検証する。
/// </summary>
public sealed class PBKDF2CustomerPasswordVerifier : ICustomerPasswordVerifier
{
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly string _dummyPasswordHash;

    public PBKDF2CustomerPasswordVerifier(
        IPasswordHashingService passwordHashingService)
    {
        _passwordHashingService = passwordHashingService
            ?? throw new ArgumentNullException(nameof(passwordHashingService));
        _dummyPasswordHash = _passwordHashingService.Hash(
            Guid.NewGuid().ToString("N"));
    }

    public bool Verify(string? passwordHash, string providedPassword)
    {
        if (string.IsNullOrWhiteSpace(providedPassword))
        {
            throw new DomainException("パスワードは必須です。");
        }

        return _passwordHashingService.Verify(
            providedPassword,
            string.IsNullOrWhiteSpace(passwordHash)
                ? _dummyPasswordHash
                : passwordHash);
    }
}

/// <summary>
/// 既存のDI登録との互換性を維持するためのコンテキスト。
/// </summary>
public sealed class CustomerPasswordContext;
