namespace A_exercise_EC_BE.Applications.Usecases.Purchases;

/// <summary>
/// UC005 購入確定の入力値。
/// </summary>
public sealed record ConfirmPurchaseRequest(
    Guid CustomerUuid,
    IReadOnlyCollection<ConfirmPurchaseItemRequest> Items);
