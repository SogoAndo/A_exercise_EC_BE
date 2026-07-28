namespace A_exercise_EC_BE.Presentations.ViewModels.Purchases;

/// <summary>
/// カートおよび購入履歴で表示する商品明細。
/// </summary>
public sealed record CartItemViewModel(
    Guid ProductUuid,
    string ProductName,
    int Price,
    int Quantity,
    int Subtotal);
