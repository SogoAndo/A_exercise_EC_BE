using System.Globalization;
using System.Security.Cryptography;
using A_exercise_EC_BE.Applications.Security;

namespace A_exercise_EC_BE.Applications.Security;

/// <summary>
/// PBKDF2を使用してパスワードの
/// ハッシュ化および照合を行うサービス
/// </summary>
public sealed class PasswordHashingService
    : IPasswordHashingService
{
    private const string FormatName = "PBKDF2";
    private const string AlgorithmName = "SHA256";

    private const int IterationCount = 600_000;
    private const int SaltSize = 16;
    private const int HashSize = 32;

    private const int MaximumSupportedIterationCount =
        2_000_000;

    /// <summary>
    /// 平文パスワードをハッシュ化する
    /// </summary>
    /// <param name="password">
    /// 平文パスワード
    /// </param>
    /// <returns>
    /// ソルトなどを含むハッシュ化済みパスワード
    /// </returns>
    public string Hash(
        string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException(
                "パスワードを入力してください。",
                nameof(password));
        }

        var salt =
            RandomNumberGenerator.GetBytes(
                SaltSize);

        var hash =
            Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                IterationCount,
                HashAlgorithmName.SHA256,
                HashSize);

        return string.Join(
            "$",
            FormatName,
            AlgorithmName,
            IterationCount.ToString(
                CultureInfo.InvariantCulture),
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    /// <summary>
    /// 平文パスワードと保存済みハッシュ値を照合する
    /// </summary>
    /// <param name="password">
    /// 入力された平文パスワード
    /// </param>
    /// <param name="hashedPassword">
    /// 保存済みのハッシュ化済みパスワード
    /// </param>
    /// <returns>
    /// 一致する場合true
    /// </returns>
    public bool Verify(
        string password,
        string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(hashedPassword))
        {
            return false;
        }

        var parts =
            hashedPassword.Split(
                '$',
                StringSplitOptions.None);

        if (parts.Length != 5)
        {
            return false;
        }

        if (parts[0] != FormatName ||
            parts[1] != AlgorithmName)
        {
            return false;
        }

        if (!int.TryParse(
                parts[2],
                NumberStyles.None,
                CultureInfo.InvariantCulture,
                out var iterationCount))
        {
            return false;
        }

        if (iterationCount <= 0 ||
            iterationCount >
                MaximumSupportedIterationCount)
        {
            return false;
        }

        byte[] salt;
        byte[] expectedHash;

        try
        {
            salt =
                Convert.FromBase64String(
                    parts[3]);

            expectedHash =
                Convert.FromBase64String(
                    parts[4]);
        }
        catch (FormatException)
        {
            return false;
        }

        if (salt.Length != SaltSize ||
            expectedHash.Length != HashSize)
        {
            return false;
        }

        var actualHash =
            Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterationCount,
                HashAlgorithmName.SHA256,
                expectedHash.Length);

        return CryptographicOperations
            .FixedTimeEquals(
                actualHash,
                expectedHash);
    }
}