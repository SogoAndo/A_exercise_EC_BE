using A_exercise_EC_BE.Applications.Security;
using A_exercise_EC_BE.Applications.Usecases.Customers;
using A_exercise_EC_BE.Presentations.ViewModels.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace A_exercise_EC_BE.Presentations.Controllers;

/// <summary>
/// UC002 顧客ログインAPI。
/// </summary>
[ApiController]
[Route("/")]
[Tags("UC002: 顧客ログイン")]
public sealed class LoginCustomerController(
    ILoginCustomerUsecase loginCustomerUsecase,
    ICustomerAccessTokenIssuer customerAccessTokenIssuer)
    : ControllerBase
{
    /// <summary>
    /// 顧客を認証してアクセストークンを発行する。
    /// </summary>
    /// <param name="viewModel">顧客ログイン情報。</param>
    /// <returns>顧客認証用アクセストークン。</returns>
    [HttpPost("login")]
    [ProducesResponseType(
        typeof(CustomerLoginResponseViewModel),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerLoginResponseViewModel>>
        LoginAsync(
            [FromBody] CustomerLoginViewModel viewModel)
    {
        var loginResult = await loginCustomerUsecase.LoginAsync(
            new CustomerLoginRequest(
                viewModel.MailAddress,
                viewModel.Password));
        var accessToken = customerAccessTokenIssuer.Issue(
            loginResult.CustomerUuid);

        return Ok(
            new CustomerLoginResponseViewModel(
                accessToken.AccessToken,
                accessToken.ExpiresAt,
                loginResult.Username));
    }
}
