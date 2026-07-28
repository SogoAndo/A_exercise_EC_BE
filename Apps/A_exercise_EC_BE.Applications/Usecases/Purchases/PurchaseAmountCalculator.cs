using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Applications.Usecases.Purchases;

/// <summary>
/// 購入金額を計算する。
/// </summary>
public sealed class PurchaseAmountCalculator
    : IPurchaseAmountCalculator
{
    private const string EmptyCartMessage =
        "カートに商品がありません";
    private const string InvalidQuantityMessage =
        "購入数量は1以上で入力してください。";
    private const string AmountOverflowMessage =
        "合計金額が計算可能な範囲を超えています。";

    /// <inheritdoc />
    public int Calculate(
        IReadOnlyCollection<OrdersDetail>? orderDetails)
    {
        if (orderDetails is null
            || orderDetails.Count == 0)
        {
            throw new DomainException(
                EmptyCartMessage);
        }

        try
        {
            var totalAmount = 0;

            foreach (var orderDetail in orderDetails)
            {
                if (orderDetail.Count <= 0)
                {
                    throw new DomainException(
                        InvalidQuantityMessage);
                }

                var subtotal = checked(
                    orderDetail.Product.Price
                    * orderDetail.Count);

                totalAmount = checked(
                    totalAmount + subtotal);
            }

            return totalAmount;
        }
        catch (OverflowException exception)
        {
            throw new DomainException(
                AmountOverflowMessage,
                exception);
        }
    }
}
