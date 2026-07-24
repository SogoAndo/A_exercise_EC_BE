using A_exercise_EC_BE.Application.Usecases.Customers;
using A_exercise_EC_BE.Presentation.Adapters;
using A_exercise_EC_BE.Presentation.ViewModels.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace A_exercise_EC_BE.Presentation.Controllers;

/// <summary>
/// 顧客アカウント登録API
/// </summary>
[ApiController]
[Route("api/customer/accounts")]
public class RegisterCustomerAccountController
    : ControllerBase
{
    private readonly
        IRegisterCustomerAccountUsecase
        _registerCustomerAccountUsecase;

    private readonly
        RegisterCustomerAccountViewModelAdapter
        _viewModelAdapter;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="registerCustomerAccountUsecase">
    /// 顧客アカウント登録ユースケース
    /// </param>
    /// <param name="viewModelAdapter">
    /// ViewModel変換Adapter
    /// </param>
    public RegisterCustomerAccountController(
        IRegisterCustomerAccountUsecase
            registerCustomerAccountUsecase,
        RegisterCustomerAccountViewModelAdapter
            viewModelAdapter)
    {
        _registerCustomerAccountUsecase =
            registerCustomerAccountUsecase;

        _viewModelAdapter =
            viewModelAdapter;
    }

    /// <summary>
    /// アカウント名が既に使用されているかを確認する
    /// </summary>
    /// <param name="username">
    /// 確認対象のアカウント名
    /// </param>
    /// <returns>
    /// 使用されていなければ204
    /// </returns>
    [HttpGet("validate/username")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>
        ValidateUsernameAsync(
            [FromQuery] string username)
    {
        await _registerCustomerAccountUsecase
            .ExistsByUsernameAsync(
                username);

        return NoContent();
    }

    /// <summary>
    /// メールアドレスが既に使用されているかを確認する
    /// </summary>
    /// <param name="mailAddress">
    /// 確認対象のメールアドレス
    /// </param>
    /// <returns>
    /// 使用されていなければ204
    /// </returns>
    [HttpGet("validate/mail-address")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>
        ValidateMailAddressAsync(
            [FromQuery] string mailAddress)
    {
        await _registerCustomerAccountUsecase
            .ExistsByMailAddressAsync(
                mailAddress);

        return NoContent();
    }

    /// <summary>
    /// 顧客アカウントを登録する
    /// </summary>
    /// <param name="viewModel">
    /// 顧客アカウント登録情報
    /// </param>
    /// <returns>
    /// 登録成功時201
    /// </returns>
    [HttpPost]
    [ProducesResponseType(
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>
        RegisterCustomerAccountAsync(
            [FromBody]
            RegisterCustomerAccountViewModel
                viewModel)
    {
        var customer =
            _viewModelAdapter.Convert(
                viewModel);

        await _registerCustomerAccountUsecase
            .RegisterCustomerAccountAsync(
                customer);

        return StatusCode(
            StatusCodes.Status201Created);
    }
}