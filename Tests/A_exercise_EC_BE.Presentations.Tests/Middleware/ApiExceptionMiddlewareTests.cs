using System.Text.Json;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Presentations.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace A_exercise_EC_BE.Presentations.Tests.Middleware;

[TestClass]
[TestCategory("Presentations/Middleware")]
public class ApiExceptionMiddlewareTests
{
    [TestMethod]
    public async Task InvokeAsync_WhenDomainException_ReturnsBadRequest()
    {
        const string message = "入力内容が正しくありません。";

        var result = await ExecuteAsync(
            new DomainException(message));

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            result.StatusCode);
        Assert.AreEqual(message, result.Message);
    }

    [TestMethod]
    public async Task InvokeAsync_WhenExistsException_ReturnsConflict()
    {
        const string message = "既に登録されています。";

        var result = await ExecuteAsync(
            new ExistsException(message));

        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            result.StatusCode);
        Assert.AreEqual(message, result.Message);
    }

    [TestMethod]
    public async Task InvokeAsync_WhenNotFoundException_ReturnsNotFound()
    {
        const string message = "対象のデータが見つかりません。";

        var result = await ExecuteAsync(
            new NotFoundException(message));

        Assert.AreEqual(
            StatusCodes.Status404NotFound,
            result.StatusCode);
        Assert.AreEqual(message, result.Message);
    }

    [TestMethod]
    public async Task InvokeAsync_WhenUnauthorizedAccessException_ReturnsUnauthorized()
    {
        const string message =
            "メールアドレスまたはパスワードが正しくありません。";

        var result = await ExecuteAsync(
            new UnauthorizedAccessException(message));

        Assert.AreEqual(
            StatusCodes.Status401Unauthorized,
            result.StatusCode);
        Assert.AreEqual(message, result.Message);
    }

    [TestMethod]
    public async Task InvokeAsync_WhenInternalException_ReturnsGenericSystemError()
    {
        var result = await ExecuteAsync(
            new InternalException("データベース接続に失敗しました。"));

        Assert.AreEqual(
            StatusCodes.Status500InternalServerError,
            result.StatusCode);
        Assert.AreEqual(
            "システムエラーが発生しました。",
            result.Message);
    }

    [TestMethod]
    public async Task InvokeAsync_WhenUnexpectedException_ReturnsGenericSystemError()
    {
        var result = await ExecuteAsync(
            new InvalidOperationException(
                "外部へ返してはいけないエラー情報"));

        Assert.AreEqual(
            StatusCodes.Status500InternalServerError,
            result.StatusCode);
        Assert.AreEqual(
            "システムエラーが発生しました。",
            result.Message);
    }

    private static async Task<ErrorResult> ExecuteAsync(
        Exception exception)
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        var middleware = new ApiExceptionMiddleware(
            _ => throw exception,
            NullLogger<ApiExceptionMiddleware>.Instance);

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        using var responseJson = await JsonDocument.ParseAsync(
            context.Response.Body);
        var message = responseJson.RootElement
            .GetProperty("message")
            .GetString();

        return new ErrorResult(
            context.Response.StatusCode,
            message);
    }

    private sealed record ErrorResult(
        int StatusCode,
        string? Message);
}
