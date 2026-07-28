using System.IdentityModel.Tokens.Jwt;
using A_exercise_EC_BE.Applications.Usecases.Purchases;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.Authentication;
using A_exercise_EC_BE.Presentations.ViewModels.Purchases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace A_exercise_EC_BE.Presentations.Controllers;

/// <summary>
/// UC005 購入確定API。
/// </summary>
[ApiController]
[Route("purchase")]
[Tags("UC005: 購入確定")]
public sealed class ConfirmPurchaseController(
    IConfirmPurchaseUsecase confirmPurchaseUsecase,
    PurchaseViewModelAdapter adapter)
    : ControllerBase
{
    /// <summary>
    /// 認証済み顧客の購入を確定する。
    /// </summary>
    /// <param name="viewModel">
    /// 支払い方法と購入商品
    /// </param>
    /// <returns>
    /// 確定した注文情報
    /// </returns>
    [Authorize(
        AuthenticationSchemes =
            CustomerJwtAuthenticationDefaults.AuthenticationScheme)]
    [HttpPost("complete")]
    [ProducesResponseType(
        typeof(PurchaseCompleteViewModel),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PurchaseCompleteViewModel>>
        CompleteAsync(
            [FromBody]
            ConfirmPurchaseViewModel viewModel)
    {
        var request = adapter.ConvertToRequest(
            GetCustomerUuid(),
            viewModel);

        var result =
            await confirmPurchaseUsecase
                .ConfirmAsync(request);

        return Created(
            $"/purchase/history/{result.OrderUuid:D}",
            adapter.ConvertToCompleteViewModel(
                result));
    }

    private Guid GetCustomerUuid()
    {
        var subject = User.FindFirst(
            JwtRegisteredClaimNames.Sub)?.Value;

        if (!Guid.TryParse(
                subject,
                out var customerUuid)
            || customerUuid == Guid.Empty)
        {
            throw new UnauthorizedAccessException(
                "顧客認証情報が正しくありません。");
        }

        return customerUuid;
    }
}
