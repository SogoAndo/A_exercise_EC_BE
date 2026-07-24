namespace A_exercise_EC_BE.Applications.Usecases.Purchases;

/// <summary>
/// UC005 購入確定の商品入力値。
/// </summary>
public sealed record ConfirmPurchaseItemRequest(
    Guid ProductUuid,
    int Quantity);
