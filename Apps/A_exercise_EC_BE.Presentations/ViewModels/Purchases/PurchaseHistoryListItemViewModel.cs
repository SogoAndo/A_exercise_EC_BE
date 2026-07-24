namespace A_exercise_EC_BE.Presentations.ViewModels.Purchases;

/// <summary>
/// 購入履歴一覧の1件分。
/// </summary>
public sealed record PurchaseHistoryListItemViewModel(
    Guid OrderUuid,
    string OrderDate,
    string OrderStatus,
    int TotalPrice,
    string DetailUrl);
