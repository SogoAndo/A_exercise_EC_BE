namespace A_exercise_EC_BE.Presentations.ViewModels.Purchases;

/// <summary>
/// UC005 購入完了時の応答。
/// </summary>
public sealed record PurchaseCompleteViewModel(
    string CompleteMessage,
    Guid OrderUuid,
    string OrderDate,
    int TotalPrice);
