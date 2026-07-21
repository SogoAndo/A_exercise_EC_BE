using A_exercise_EC_BE.Domain.Exceptions;

namespace A_exercise_EC_BE.Domain.Models;

/// <summary>
/// 商品在庫を表すドメインオブジェクト。
/// </summary>
public class ProductStock
{
    public Guid StockUuid { get; }
    public int Quantity { get; }

    public ProductStock(Guid stockUuid, int quantity)
    {
        if (stockUuid == Guid.Empty)
        {
            throw new DomainException("商品在庫識別IDが不正です");
        }

        if (quantity < 0)
        {
            throw new DomainException("在庫数は0以上で入力してください");
        }

        StockUuid = stockUuid;
        Quantity = quantity;
    }

    public override bool Equals(object? obj) =>
        obj is ProductStock other && StockUuid == other.StockUuid;

    public override int GetHashCode() => StockUuid.GetHashCode();
}
