namespace A_exercise_EC_BE.Presentations.ViewModels.Purchases;

/// <summary>
/// 購入履歴詳細レスポンス。
/// </summary>
public sealed record PurchaseHistoryDetailViewModel(
    Guid OrderUuid,
    string OrderDate,
    int OrderStatusId,
    string OrderStatusName,
    List<CartItemViewModel> OrderItems,
    int TotalPrice);
