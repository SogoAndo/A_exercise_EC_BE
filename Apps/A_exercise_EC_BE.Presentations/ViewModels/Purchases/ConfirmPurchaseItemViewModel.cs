using System.ComponentModel.DataAnnotations;

namespace A_exercise_EC_BE.Presentations.ViewModels.Purchases;

/// <summary>
/// UC005 購入確定の商品入力情報。
/// </summary>
public sealed class ConfirmPurchaseItemViewModel
{
    /// <summary>
    /// 商品UUID。
    /// </summary>
    public Guid ProductUuid { get; init; }

    /// <summary>
    /// 購入数量。
    /// </summary>
    [Range(
        1,
        int.MaxValue,
        ErrorMessage =
            "購入数量は1以上で入力してください。")]
    public int Quantity { get; init; }
}
