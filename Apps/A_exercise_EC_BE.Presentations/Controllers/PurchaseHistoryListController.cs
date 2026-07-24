using System.IdentityModel.Tokens.Jwt;
using A_exercise_EC_BE.Applications.Usecases.Purchases;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.Authentication;
using A_exercise_EC_BE.Presentations.ViewModels.Purchases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A_exercise_EC_BE.Presentations.Controllers;

/// <summary>
/// UC007 購入履歴一覧API。
/// </summary>
[ApiController]
[Route("purchase/history")]
[Tags("UC007: 購入履歴閲覧")]
public sealed class PurchaseHistoryListController(
    ISearchPurchaseHistoryUsecase purchaseHistoryUsecase,
    PurchaseHistoryViewModelAdapter adapter)
    : ControllerBase
{
    /// <summary>
    /// 認証済み顧客の購入履歴一覧を取得する。
    /// </summary>
    [Authorize(
        AuthenticationSchemes =
            CustomerJwtAuthenticationDefaults.AuthenticationScheme)]
    [HttpGet]
    [ProducesResponseType(
        typeof(PurchaseHistoryResultViewModel),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PurchaseHistoryResultViewModel>>
        GetAsync()
    {
        var orders = await purchaseHistoryUsecase.SearchAsync(
            GetCustomerUuid());

        return Ok(
            adapter.ConvertToResultViewModel(orders));
    }

    private Guid GetCustomerUuid()
    {
        var subject = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(subject, out var customerUuid)
            || customerUuid == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "顧客認証情報が正しくありません。");
        }

        return customerUuid;
    }
}
