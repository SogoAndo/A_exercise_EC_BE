namespace A_exercise_EC_BE.Applications.Usecases.Purchases;

/// <summary>
/// UC005 購入確定の処理結果。
/// </summary>
public sealed record ConfirmPurchaseResult(
    Guid OrderUuid,
    DateTime OrderDate,
    int AmountTotal);
