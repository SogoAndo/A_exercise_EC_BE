using A_exercise_EC_BE.Domains.Exceptions;

namespace A_exercise_EC_BE.Presentations.Middleware;

/// <summary>
/// APIで発生した例外をHTTPレスポンスへ変換するミドルウェア
/// </summary>
public sealed class ApiExceptionMiddleware
{
    private const string SystemErrorMessage =
        "システムエラーが発生しました。";

    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionMiddleware> _logger;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ApiExceptionMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 後続処理で発生した例外を処理する
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            await WriteErrorResponseAsync(
                context,
                exception);
        }
    }

    private async Task WriteErrorResponseAsync(
        HttpContext context,
        Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            DomainException =>
                (StatusCodes.Status400BadRequest, exception.Message),
            ExistsException =>
                (StatusCodes.Status409Conflict, exception.Message),
            UnauthorizedAccessException =>
                (StatusCodes.Status401Unauthorized, exception.Message),
            InternalException =>
                (StatusCodes.Status500InternalServerError, SystemErrorMessage),
            _ =>
                (StatusCodes.Status500InternalServerError, SystemErrorMessage)
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "API処理中に予期しないエラーが発生しました。");
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(
            new
            {
                message
            });
    }
}
