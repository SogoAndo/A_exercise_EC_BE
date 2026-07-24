using A_exercise_EC_BE.Domains.Exceptions;

namespace A_exercise_EC_BE.Domains.Tests.Exceptions;

/// <summary>
/// PasswordRehashNeededExceptionの単体テスト
/// </summary>
[TestClass]
public class PasswordRehashNeededExceptionTest
{
    /// <summary>
    /// デフォルトコンストラクタで例外をスローできること
    /// </summary>
    [TestMethod(DisplayName = "PasswordRehashNeededException_デフォルトコンストラクタで例外をスローできる")]
    public void PasswordRehashNeededException_DefaultConstructor_ThrowsExactly()
    {
        // Act
        var exception = Assert.ThrowsExactly<PasswordRehashNeededException>(() =>
        {
            throw new PasswordRehashNeededException();
        });

        // Assert
        Assert.IsNotNull(exception);
    }

    /// <summary>
    /// メッセージ付きコンストラクタで例外をスローできること
    /// </summary>
    [TestMethod(DisplayName = "PasswordRehashNeededException_メッセージ付きコンストラクタで例外をスローできる")]
    public void PasswordRehashNeededException_MessageConstructor_ThrowsExactly()
    {
        // Arrange
        var message = "パスワードは認証されたが、再ハッシュが必要です。";

        // Act
        var exception = Assert.ThrowsExactly<PasswordRehashNeededException>(() =>
        {
            throw new PasswordRehashNeededException(message);
        });

        // Assert
        Assert.AreEqual(message, exception.Message);
    }

    /// <summary>
    /// メッセージと内部例外付きコンストラクタで例外をスローできること
    /// </summary>
    [TestMethod(DisplayName = "PasswordRehashNeededException_メッセージと内部例外付きコンストラクタで例外をスローできる")]
    public void PasswordRehashNeededException_MessageAndInnerExceptionConstructor_ThrowsExactly()
    {
        // Arrange
        var message = "パスワードは認証されたが、再ハッシュが必要です。";
        var innerException = new InvalidOperationException("元の例外です。");

        // Act
        var exception = Assert.ThrowsExactly<PasswordRehashNeededException>(() =>
        {
            throw new PasswordRehashNeededException(message, innerException);
        });

        // Assert
        Assert.AreEqual(message, exception.Message);
        Assert.AreSame(innerException, exception.InnerException);
    }
}