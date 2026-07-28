using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_exercise_EC_BE.Domains.Exceptions;

namespace A_exercise_EC_BE.Domains.Tests.Exceptions;

/// <summary>
/// NotFoundExceptionクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Exceptions")]
public class NotFoundExceptionTests
{
    [TestMethod(DisplayName = "メッセージのみを渡した場合、Messageに正しく設定される")]
    public void Constructor_WithMessage_ShouldSetMessage()
    {
        // データを用意する
        var message = "データが存在しません";

        // インスタンスを生成する
        var exception = new NotFoundException(message);

        // メッセージを検証する
        Assert.AreEqual(message, exception.Message);
    }

    [TestMethod(DisplayName = "メッセージのみを渡した場合、InnerExceptionがnullになる")]
    public void Constructor_WithMessage_ShouldSetInnerExceptionToNull()
    {
        // データを用意する
        var message = "データが存在しません";

        // インスタンスを生成する
        var exception = new NotFoundException(message);

        // InnerExceptionを検証する
        Assert.IsNull(exception.InnerException);
    }

    [TestMethod(DisplayName = "メッセージと元例外を渡した場合、Messageに正しく設定される")]
    public void Constructor_WithMessageAndInnerException_ShouldSetMessage()
    {
        // データを用意する
        var message = "データが存在しません";
        var innerException = new Exception("元の例外です");

        // インスタンスを生成する
        var exception = new NotFoundException(message, innerException);

        // メッセージを検証する
        Assert.AreEqual(message, exception.Message);
    }

    [TestMethod(DisplayName = "メッセージと元例外を渡した場合、InnerExceptionに渡した例外が設定される")]
    public void Constructor_WithMessageAndInnerException_ShouldSetInnerException()
    {
        // データを用意する
        var message = "データが存在しません";
        var innerException = new Exception("元の例外です");

        // インスタンスを生成する
        var exception = new NotFoundException(message, innerException);

        // InnerExceptionを検証する
        Assert.AreEqual(innerException, exception.InnerException);
    }
}