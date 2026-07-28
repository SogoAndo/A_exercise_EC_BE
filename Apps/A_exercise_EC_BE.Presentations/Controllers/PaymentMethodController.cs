using A_exercise_EC_BE.Applications.Usecases.PaymentMethods;
using A_exercise_EC_BE.Presentations.ViewModels.PaymentMethods;
using Microsoft.AspNetCore.Mvc;

namespace A_exercise_EC_BE.Presentations.Controllers;

/// <summary>
/// 支払い方法API。
/// </summary>
[ApiController]
[Route("payment-method")]
[Tags("支払い方法")]
public class PaymentMethodController(
    IFindAllPaymentMethodsUsecase
        findAllPaymentMethodsUsecase)
    : ControllerBase
{
    /// <summary>
    /// 支払い方法のプルダウン項目を取得する。
    /// </summary>
    /// <returns>支払い方法一覧。</returns>
    [HttpGet("options")]
    [ProducesResponseType(
        typeof(List<PaymentMethodOptionViewModel>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<
        List<PaymentMethodOptionViewModel>>>
        FindAllAsync()
    {
        var paymentMethods =
            await findAllPaymentMethodsUsecase
                .ExecuteAsync();

        var viewModels = paymentMethods
            .Select(
                paymentMethod =>
                    new PaymentMethodOptionViewModel
                    {
                        Value = paymentMethod.Id,
                        Label = paymentMethod.Name
                    })
            .ToList();

        return Ok(viewModels);
    }
}