using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_exercise_EC_BE.Domain.Exceptions;

namespace A_exercise_EC_BE.Domain.Tests.Exceptions;

/// <summary>
/// ExistsExceptionクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Exceptions")]
public class ExistsExceptionTests
{
    [TestMethod(DisplayName = "メッセージのみを渡した場合、Messageに正しく設定される")]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // データを用意する
        var message = "データが既に存在します";

        // インスタンスを生成する
        var exception = new ExistsException(message);

        // メッセージを検証する
        Assert.AreEqual(message, exception.Message);
    }

    [TestMethod(DisplayName = "メッセージのみを渡した場合、InnerExceptionがnullになる")]
    public void Constructor_WithMessage_ShouldSetInnerExceptionToNull()
    {
        // データを用意する
        var message = "データが既に存在します";

        // インスタンスを生成する
        var exception = new ExistsException(message);

        // InnerExceptionを検証する
        Assert.IsNull(exception.InnerException);
    }

    [TestMethod(DisplayName = "メッセージと元例外を渡した場合、Messageに正しく設定される")]
    public void Constructor_WithMessageAndInnerException_ShouldSetMessage()
    {
        // データを用意する
        var message = "データが既に存在します";
        var innerException = new Exception("元の例外です");

        // インスタンスを生成する
        var exception = new ExistsException(message, innerException);

        // メッセージを検証する
        Assert.AreEqual(message, exception.Message);
    }

    [TestMethod(DisplayName = "メッセージと元例外を渡した場合、InnerExceptionに渡した例外が設定される")]
    public void Constructor_WithMessageAndInnerException_ShouldSetInnerException()
    {
        // データを用意する
        var message = "データが既に存在します";
        var innerException = new Exception("元の例外です");

        // インスタンスを生成する
        var exception = new ExistsException(message, innerException);

        // InnerExceptionを検証する
        Assert.AreEqual(innerException, exception.InnerException);
    }
}