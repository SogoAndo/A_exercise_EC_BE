using System.Globalization;
using System.Security.Cryptography;
using A_exercise_EC_BE.Applications.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_exercise_EC_BE.Applications.Tests.Security;

/// <summary>
/// PasswordHashingServiceの単体テスト
/// </summary>
[TestClass]
[TestCategory("Security")]
public class PasswordHashingServiceTests
{
    private const string ValidPassword =
        "P@ssw0rd123!";

    private PasswordHashingService
        _service = null!;

    /// <summary>
    /// 各テスト実行前の初期化処理
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _service =
            new PasswordHashingService();
    }

    /*
     * Hash
     */

    /// <summary>
    /// 有効なパスワードを
    /// ハッシュ化できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "Hash_有効なパスワードをハッシュ化できる")]
    public void
        Hash_WhenPasswordIsValid_ReturnsValidHash()
    {
        // Act
        var actual =
            _service.Hash(
                ValidPassword);

        // Assert
        Assert.AreNotEqual(
            ValidPassword,
            actual);

        var parts =
            actual.Split(
                '$',
                StringSplitOptions.None);

        Assert.HasCount(
            5,
            parts);

        Assert.AreEqual(
            "PBKDF2",
            parts[0]);

        Assert.AreEqual(
            "SHA256",
            parts[1]);

        Assert.AreEqual(
            "600000",
            parts[2]);

        var salt =
            Convert.FromBase64String(
                parts[3]);

        var hash =
            Convert.FromBase64String(
                parts[4]);

        Assert.HasCount(
            16,
            salt);

        Assert.HasCount(
            32,
            hash);

        Assert.IsTrue(
            _service.Verify(
                ValidPassword,
                actual));
    }

    /// <summary>
    /// 同じパスワードでも、
    /// ソルトにより異なるハッシュ値が
    /// 生成されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "Hash_同じパスワードでも異なるハッシュ値を生成する")]
    public void
        Hash_WhenSamePasswordIsHashedTwice_ReturnsDifferentHashes()
    {
        // Act
        var first =
            _service.Hash(
                ValidPassword);

        var second =
            _service.Hash(
                ValidPassword);

        // Assert
        Assert.AreNotEqual(
            first,
            second);
    }

    /// <summary>
    /// パスワードが未入力の場合、
    /// ArgumentExceptionをスローすること
    /// </summary>
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void
        Hash_WhenPasswordIsNotEntered_ThrowsExactlyArgumentException(
            string? password)
    {
        // Act
        var exception =
            Assert.ThrowsExactly<
                ArgumentException>(
                () =>
                {
                    _service.Hash(
                        password!);
                });

        // Assert
        StringAssert.Contains(
            exception.Message,
            "パスワードを入力してください。");

        Assert.AreEqual(
            "password",
            exception.ParamName);
    }

    /*
     * Verify：正常な形式
     */

    /// <summary>
    /// 正しいパスワードの場合、
    /// trueを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_正しいパスワードの場合はtrueを返す")]
    public void
        Verify_WhenPasswordIsCorrect_ReturnsTrue()
    {
        // Arrange
        var hashedPassword =
            CreateValidHashedPassword(
                ValidPassword);

        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword);

        // Assert
        Assert.IsTrue(
            actual);
    }

    /// <summary>
    /// 間違ったパスワードの場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_間違ったパスワードの場合はfalseを返す")]
    public void
        Verify_WhenPasswordIsIncorrect_ReturnsFalse()
    {
        // Arrange
        var hashedPassword =
            CreateValidHashedPassword(
                ValidPassword);

        // Act
        var actual =
            _service.Verify(
                "WrongPassword",
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /*
     * Verify：引数の未入力
     */

    /// <summary>
    /// 平文パスワードが未入力の場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void
        Verify_WhenPasswordIsNotEntered_ReturnsFalse(
            string? password)
    {
        // Arrange
        var hashedPassword =
            CreateValidHashedPassword(
                ValidPassword);

        // Act
        var actual =
            _service.Verify(
                password!,
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /// <summary>
    /// ハッシュ化済みパスワードが未入力の場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void
        Verify_WhenHashedPasswordIsNotEntered_ReturnsFalse(
            string? hashedPassword)
    {
        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword!);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /*
     * Verify：文字列フォーマット
     */

    /// <summary>
    /// ハッシュ文字列が5要素でない場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_ハッシュ文字列が5要素でない場合はfalseを返す")]
    public void
        Verify_WhenPartsLengthIsInvalid_ReturnsFalse()
    {
        // Arrange
        const string hashedPassword =
            "PBKDF2$SHA256$1$only-four-parts";

        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /// <summary>
    /// フォーマット名がPBKDF2でない場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_フォーマット名がPBKDF2でない場合はfalseを返す")]
    public void
        Verify_WhenFormatNameIsInvalid_ReturnsFalse()
    {
        // Arrange
        var hashedPassword =
            CreateEncodedHashedPassword(
                formatName: "INVALID",
                algorithmName: "SHA256",
                iterationText: "1",
                saltText:
                    Convert.ToBase64String(
                        new byte[16]),
                hashText:
                    Convert.ToBase64String(
                        new byte[32]));

        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /// <summary>
    /// アルゴリズム名がSHA256でない場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_アルゴリズム名がSHA256でない場合はfalseを返す")]
    public void
        Verify_WhenAlgorithmNameIsInvalid_ReturnsFalse()
    {
        // Arrange
        var hashedPassword =
            CreateEncodedHashedPassword(
                formatName: "PBKDF2",
                algorithmName: "SHA512",
                iterationText: "1",
                saltText:
                    Convert.ToBase64String(
                        new byte[16]),
                hashText:
                    Convert.ToBase64String(
                        new byte[32]));

        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /*
     * Verify：反復回数
     */

    /// <summary>
    /// 反復回数を整数へ変換できない場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_反復回数が整数でない場合はfalseを返す")]
    public void
        Verify_WhenIterationCountIsNotInteger_ReturnsFalse()
    {
        // Arrange
        var hashedPassword =
            CreateEncodedHashedPassword(
                formatName: "PBKDF2",
                algorithmName: "SHA256",
                iterationText: "not-number",
                saltText:
                    Convert.ToBase64String(
                        new byte[16]),
                hashText:
                    Convert.ToBase64String(
                        new byte[32]));

        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /// <summary>
    /// 反復回数が0の場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_反復回数が0の場合はfalseを返す")]
    public void
        Verify_WhenIterationCountIsZero_ReturnsFalse()
    {
        // Arrange
        var hashedPassword =
            CreateEncodedHashedPassword(
                formatName: "PBKDF2",
                algorithmName: "SHA256",
                iterationText: "0",
                saltText:
                    Convert.ToBase64String(
                        new byte[16]),
                hashText:
                    Convert.ToBase64String(
                        new byte[32]));

        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /// <summary>
    /// 反復回数が最大対応回数を
    /// 超えている場合、falseを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_反復回数が最大対応回数を超える場合はfalseを返す")]
    public void
        Verify_WhenIterationCountExceedsMaximum_ReturnsFalse()
    {
        // Arrange
        var hashedPassword =
            CreateEncodedHashedPassword(
                formatName: "PBKDF2",
                algorithmName: "SHA256",
                iterationText: "2000001",
                saltText:
                    Convert.ToBase64String(
                        new byte[16]),
                hashText:
                    Convert.ToBase64String(
                        new byte[32]));

        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /*
     * Verify：Base64
     */

    /// <summary>
    /// ソルトが不正なBase64の場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_ソルトが不正なBase64の場合はfalseを返す")]
    public void
        Verify_WhenSaltIsInvalidBase64_ReturnsFalse()
    {
        // Arrange
        var hashedPassword =
            CreateEncodedHashedPassword(
                formatName: "PBKDF2",
                algorithmName: "SHA256",
                iterationText: "1",
                saltText: "invalid-base64***",
                hashText:
                    Convert.ToBase64String(
                        new byte[32]));

        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /// <summary>
    /// ハッシュ値が不正なBase64の場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_ハッシュ値が不正なBase64の場合はfalseを返す")]
    public void
        Verify_WhenHashIsInvalidBase64_ReturnsFalse()
    {
        // Arrange
        var hashedPassword =
            CreateEncodedHashedPassword(
                formatName: "PBKDF2",
                algorithmName: "SHA256",
                iterationText: "1",
                saltText:
                    Convert.ToBase64String(
                        new byte[16]),
                hashText: "invalid-base64***");

        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /*
     * Verify：バイト配列の長さ
     */

    /// <summary>
    /// ソルトの長さが16バイトでない場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_ソルトの長さが16バイトでない場合はfalseを返す")]
    public void
        Verify_WhenSaltLengthIsInvalid_ReturnsFalse()
    {
        // Arrange
        var hashedPassword =
            CreateEncodedHashedPassword(
                formatName: "PBKDF2",
                algorithmName: "SHA256",
                iterationText: "1",
                saltText:
                    Convert.ToBase64String(
                        new byte[15]),
                hashText:
                    Convert.ToBase64String(
                        new byte[32]));

        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /// <summary>
    /// ハッシュ値の長さが32バイトでない場合、
    /// falseを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Verify_ハッシュ値の長さが32バイトでない場合はfalseを返す")]
    public void
        Verify_WhenHashLengthIsInvalid_ReturnsFalse()
    {
        // Arrange
        var hashedPassword =
            CreateEncodedHashedPassword(
                formatName: "PBKDF2",
                algorithmName: "SHA256",
                iterationText: "1",
                saltText:
                    Convert.ToBase64String(
                        new byte[16]),
                hashText:
                    Convert.ToBase64String(
                        new byte[31]));

        // Act
        var actual =
            _service.Verify(
                ValidPassword,
                hashedPassword);

        // Assert
        Assert.IsFalse(
            actual);
    }

    /*
     * テストデータ生成
     */

    /// <summary>
    /// 少ない反復回数を使用して、
    /// テスト用の有効なハッシュ値を生成する
    /// </summary>
    private static string
        CreateValidHashedPassword(
            string password)
    {
        const int iterationCount =
            1;

        var salt =
            Enumerable.Range(
                    1,
                    16)
                .Select(
                    value =>
                        (byte)value)
                .ToArray();

        var hash =
            Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterationCount,
                HashAlgorithmName.SHA256,
                32);

        return CreateEncodedHashedPassword(
            formatName: "PBKDF2",
            algorithmName: "SHA256",
            iterationText:
                iterationCount.ToString(
                    CultureInfo.InvariantCulture),
            saltText:
                Convert.ToBase64String(
                    salt),
            hashText:
                Convert.ToBase64String(
                    hash));
    }

    /// <summary>
    /// 指定された各要素から
    /// ハッシュ文字列を生成する
    /// </summary>
    private static string
        CreateEncodedHashedPassword(
            string formatName,
            string algorithmName,
            string iterationText,
            string saltText,
            string hashText)
    {
        return string.Join(
            "$",
            formatName,
            algorithmName,
            iterationText,
            saltText,
            hashText);
    }
}