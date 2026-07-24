using A_exercise_EC_BE.Applications.Usecases.Accounts;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.ViewModels.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace A_exercise_EC_BE.Presentations.Controllers;

/// <summary>
/// UC001: 顧客アカウント登録
/// FP003: 顧客アカウント登録(入力)画面
/// FP004: 顧客アカウント登録(確認)画面
/// FP005: 顧客アカウント登録(完了)画面
/// </summary>
[ApiController]
[Route("account")]
[Tags("UC001: 顧客アカウント登録")]
public class RegisterCustomerAccountController
    : ControllerBase
{
    private readonly
        IRegisterCustomerAccountUsecase
        _registerCustomerAccountUsecase;

    private readonly
        RegisterCustomerAccountViewModelAdapter
        _adapter;

    private readonly
        ILogger<RegisterCustomerAccountController>
        _logger;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="registerCustomerAccountUsecase">
    /// 顧客アカウント登録ユースケース
    /// </param>
    /// <param name="adapter">
    /// 顧客アカウント登録ViewModelを
    /// Customerなどへ変換するAdapter
    /// </param>
    /// <param name="logger">
    /// ログ出力機能
    /// </param>
    public RegisterCustomerAccountController(
        IRegisterCustomerAccountUsecase
            registerCustomerAccountUsecase,
        RegisterCustomerAccountViewModelAdapter
            adapter,
        ILogger<RegisterCustomerAccountController>
            logger)
    {
        _registerCustomerAccountUsecase =
            registerCustomerAccountUsecase;

        _adapter =
            adapter;

        _logger =
            logger;
    }

    /// <summary>
    /// FP003:
    /// 顧客アカウント登録入力画面の
    /// 初期表示情報を取得する
    /// </summary>
    /// <returns>
    /// 取得成功時: Ok(200)、
    /// システムエラー: InternalServerError(500)
    /// </returns>
    [HttpGet("form")]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status500InternalServerError)]
    public IActionResult GetForm()
    {
        try
        {
            return Ok(new
            {
                title =
                    "顧客アカウント登録(入力)",

                model =
                    new RegisterCustomerAccountViewModel()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "顧客アカウント登録入力画面の" +
                "初期表示情報取得中にエラーが発生しました。"
            );

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    code =
                        "SYSTEM_ERROR",

                    message =
                        "画面情報の取得に失敗しました"
                });
        }
    }

    /// <summary>
    /// アカウント名が既に存在するかを検証する
    /// </summary>
    /// <param name="username">
    /// 検証対象のアカウント名
    /// </param>
    /// <returns>
    /// 存在しない場合: Ok(200)、
    /// 入力値不正: BadRequest(400)、
    /// 存在する場合: Conflict(409)、
    /// システムエラー: InternalServerError(500)
    /// </returns>
    [HttpGet("validate/username")]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>
        ValidateUsername(
            [FromQuery] string username)
    {
        try
        {
            await _registerCustomerAccountUsecase
                .ExistsByUsernameAsync(
                    username);

            return Ok(new
            {
                exists =
                    false,

                message =
                    "使用できるアカウント名です"
            });
        }
        catch (ExistsException ex)
        {
            return Conflict(new
            {
                code =
                    "USERNAME_ALREADY_EXISTS",

                exists =
                    true,

                message =
                    ex.Message
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                code =
                    "VALIDATION_ERROR",

                message =
                    ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "アカウント名の存在確認中に" +
                "エラーが発生しました。" +
                "username: {Username}",
                username
            );

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    code =
                        "SYSTEM_ERROR",

                    message =
                        "システムエラーが発生しました。" +
                        "管理者に連絡してください"
                });
        }
    }

    /// <summary>
    /// メールアドレスが既に存在するかを検証する
    /// </summary>
    /// <param name="mailAddress">
    /// 検証対象のメールアドレス
    /// </param>
    /// <returns>
    /// 存在しない場合: Ok(200)、
    /// 入力値不正: BadRequest(400)、
    /// 存在する場合: Conflict(409)、
    /// システムエラー: InternalServerError(500)
    /// </returns>
    [HttpGet("validate/mail-address")]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>
        ValidateMailAddress(
            [FromQuery] string mailAddress)
    {
        try
        {
            await _registerCustomerAccountUsecase
                .ExistsByMailAddressAsync(
                    mailAddress);

            return Ok(new
            {
                exists =
                    false,

                message =
                    "使用できるメールアドレスです"
            });
        }
        catch (ExistsException ex)
        {
            return Conflict(new
            {
                code =
                    "MAIL_ADDRESS_ALREADY_EXISTS",

                exists =
                    true,

                message =
                    ex.Message
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                code =
                    "VALIDATION_ERROR",

                message =
                    ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "メールアドレスの存在確認中に" +
                "エラーが発生しました。" +
                "mailAddress: {MailAddress}",
                mailAddress
            );

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    code =
                        "SYSTEM_ERROR",

                    message =
                        "システムエラーが発生しました。" +
                        "管理者に連絡してください"
                });
        }
    }

    /// <summary>
    /// FP004:
    /// 顧客アカウント登録内容を確認する
    /// </summary>
    /// <param name="model">
    /// 顧客アカウント登録用ViewModel
    /// </param>
    /// <returns>
    /// 確認成功時: Ok(200)、
    /// 入力値不正: BadRequest(400)、
    /// 入力内容重複: Conflict(409)、
    /// システムエラー: InternalServerError(500)
    /// </returns>
    [HttpPost("confirm")]
    [ProducesResponseType(
        typeof(RegisterCustomerAccountConfirmViewModel),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>
        Confirm(
            [FromBody]
            RegisterCustomerAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                code =
                    "VALIDATION_ERROR",

                messages =
                    GetModelStateErrorMessages()
            });
        }

        try
        {
            // 確認画面へ進む前に
            // アカウント名の重複を確認する
            await _registerCustomerAccountUsecase
                .ExistsByUsernameAsync(
                    model.Username);

            // 確認画面へ進む前に
            // メールアドレスの重複を確認する
            await _registerCustomerAccountUsecase
                .ExistsByMailAddressAsync(
                    model.MailAddress);

            var confirmViewModel =
                _adapter.ToConfirmViewModel(
                    model);

            return Ok(
                confirmViewModel);
        }
        catch (ExistsException ex)
        {
            return Conflict(new
            {
                code =
                    "CUSTOMER_ACCOUNT_ALREADY_EXISTS",

                message =
                    ex.Message
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                code =
                    "VALIDATION_ERROR",

                message =
                    ex.Message
            });
        }
        catch (InternalException ex)
        {
            _logger.LogError(
                ex,
                "顧客アカウント登録確認処理中に" +
                "内部エラーが発生しました。"
            );

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    code =
                        "INTERNAL_ERROR",

                    message =
                        "確認処理に失敗しました。" +
                        "管理者に連絡してください"
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "顧客アカウント登録確認処理中に" +
                "予期しないエラーが発生しました。"
            );

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    code =
                        "SYSTEM_ERROR",

                    message =
                        "システムエラーが発生しました。" +
                        "管理者に連絡してください"
                });
        }
    }

    /// <summary>
    /// FP005:
    /// 顧客アカウントを登録し、
    /// 登録完了情報を返す
    /// </summary>
    /// <param name="model">
    /// 顧客アカウント登録用ViewModel
    /// </param>
    /// <returns>
    /// 登録成功時: Created(201)、
    /// 入力値不正: BadRequest(400)、
    /// 入力内容重複: Conflict(409)、
    /// システムエラー: InternalServerError(500)
    /// </returns>
    [HttpPost("complete")]
    [ProducesResponseType(
        typeof(RegisterCustomerAccountCompleteViewModel),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        typeof(object),
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>
        Complete(
            [FromBody]
            RegisterCustomerAccountViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                code =
                    "VALIDATION_ERROR",

                messages =
                    GetModelStateErrorMessages()
            });
        }

        try
        {
            // ViewModelをCustomerへ変換する
            var customer =
                _adapter.Convert(
                    model);

            // 顧客アカウントを登録する
            await _registerCustomerAccountUsecase
                .RegisterCustomerAccountAsync(
                    customer);

            // 登録完了画面用ViewModelへ変換する
            var completeViewModel =
                _adapter.ToCompleteViewModel(
                    customer);

            return Created(
                $"/account/{customer.CustomerUuid}",
                completeViewModel);
        }
        catch (ExistsException ex)
        {
            return Conflict(new
            {
                code =
                    "CUSTOMER_ACCOUNT_ALREADY_EXISTS",

                message =
                    ex.Message
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                code =
                    "VALIDATION_ERROR",

                message =
                    ex.Message
            });
        }
        catch (InternalException ex)
        {
            _logger.LogError(
                ex,
                "顧客アカウント登録中に" +
                "内部エラーが発生しました。"
            );

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    code =
                        "INTERNAL_ERROR",

                    message =
                        "登録処理に失敗しました。" +
                        "管理者に連絡してください"
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "顧客アカウント登録中に" +
                "予期しないエラーが発生しました。"
            );

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    code =
                        "SYSTEM_ERROR",

                    message =
                        "登録処理に失敗しました。" +
                        "管理者に連絡してください"
                });
        }
    }

    /// <summary>
    /// ModelStateのエラーメッセージを取得する
    /// </summary>
    /// <returns>
    /// エラーメッセージ一覧
    /// </returns>
    private List<string>
        GetModelStateErrorMessages()
    {
        return ModelState.Values
            .SelectMany(
                value =>
                    value.Errors)
            .Select(
                error =>
                    string.IsNullOrWhiteSpace(
                        error.ErrorMessage)
                        ? "入力内容が正しくありません"
                        : error.ErrorMessage)
            .ToList();
    }
}