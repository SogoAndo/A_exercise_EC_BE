namespace A_exercise_EC_BE.Presentations.ViewModels.Purchases;

/// <summary>
/// 購入履歴一覧レスポンス。
/// </summary>
public sealed record PurchaseHistoryResultViewModel(
    List<PurchaseHistoryListItemViewModel> OrderList,
    string? Message);
