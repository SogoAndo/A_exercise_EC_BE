using System.ComponentModel.DataAnnotations;

namespace A_exercise_EC_BE.Presentations.ViewModels.Purchases;

/// <summary>
/// UC005 購入確定の入力情報。
/// </summary>
public sealed class ConfirmPurchaseViewModel
{
    /// <summary>
    /// 支払い方法ID。
    /// </summary>
    [Range(
        1,
        int.MaxValue,
        ErrorMessage =
            "支払い方法を選択してください")]
    public int PaymentMethodId { get; init; }

    /// <summary>
    /// 購入する商品一覧。
    /// </summary>
    [Required(
        ErrorMessage =
            "カートに商品がありません")]
    [MinLength(
        1,
        ErrorMessage =
            "カートに商品がありません")]
    public List<ConfirmPurchaseItemViewModel>
        Items
    { get; init; } = [];
}
