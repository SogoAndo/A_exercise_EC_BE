namespace A_exercise_EC_BE.Applications.Usecases.Purchases;

/// <summary>
/// UC005 購入確定UseCaseのインターフェース。
/// </summary>
public interface IConfirmPurchaseUsecase
{
    /// <summary>
    /// カート内の商品を購入確定する。
    /// </summary>
    /// <param name="request">購入確定の入力値。</param>
    /// <returns>確定した注文情報。</returns>
    Task<ConfirmPurchaseResult> ConfirmAsync(
        ConfirmPurchaseRequest request);
}
