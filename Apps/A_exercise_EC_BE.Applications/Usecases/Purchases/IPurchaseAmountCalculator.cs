using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Applications.Usecases.Purchases;

/// <summary>
/// 購入金額を計算する。
/// </summary>
public interface IPurchaseAmountCalculator
{
    /// <summary>
    /// 注文明細の小計を合算して購入金額を返す。
    /// </summary>
    /// <param name="orderDetails">購入対象の注文明細。</param>
    /// <returns>購入金額。</returns>
    int Calculate(
        IReadOnlyCollection<OrdersDetail>? orderDetails);
}
