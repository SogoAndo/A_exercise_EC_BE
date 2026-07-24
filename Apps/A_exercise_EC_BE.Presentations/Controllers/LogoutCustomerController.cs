using A_exercise_EC_BE.Applications.Usecases.Customers;
using A_exercise_EC_BE.Presentations.Authentication;
using A_exercise_EC_BE.Presentations.ViewModels.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A_exercise_EC_BE.Presentations.Controllers;

/// <summary>
/// UC008 顧客ログアウトAPI。
/// </summary>
[ApiController]
[Route("/")]
[Tags("UC008: 顧客ログアウト")]
public sealed class LogoutCustomerController(
    ILogoutCustomerUsecase logoutCustomerUsecase)
    : ControllerBase
{
    /// <summary>
    /// 顧客ログアウトを実行する。
    /// </summary>
    /// <returns>顧客ログアウト結果。</returns>
    [Authorize(
        AuthenticationSchemes =
            CustomerJwtAuthenticationDefaults.AuthenticationScheme)]
    [HttpPost("logout")]
    [ProducesResponseType(
        typeof(CustomerLogoutResponseViewModel),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CustomerLogoutResponseViewModel>>
        LogoutAsync()
    {
        var result =
            await logoutCustomerUsecase.LogoutAsync();

        return Ok(
            new CustomerLogoutResponseViewModel(
                result.LoggedOut));
    }
}
