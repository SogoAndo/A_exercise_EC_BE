using A_exercise_EC_BE.Domains.Exceptions;
namespace A_exercise_EC_BE.Domains.Models;
/// <summary>
/// 商品在庫を表すドメインオブジェクト
/// </summary>
public class ProductStock
{
    /// <summary>
    /// 商品在庫識別ID(UUID)
    /// </summary>
    public Guid StockUuid { get; private set; }
    /// <summary>
    /// 商品在庫数
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// quantityは7/8以前引数string戻り値intにしていたが、
    /// Adapterとの互換性のため引数int戻り値intに変更
    /// </summary>
    public ProductStock(Guid stockUuid, int quantity)
    {
        ValidateProductStockUuid(stockUuid);
        StockUuid = stockUuid;
        Quantity = quantity;
        ValidateQuantity(quantity);
    }

    /// <summary>
    /// ID未定の商品在庫を作成する場合のコンストラクタ
    /// </summary>
    public ProductStock(int quantity)
        : this(Guid.NewGuid(), quantity) { }

    /// <summary>
    /// 商品在庫識別IDの検証
    /// </summary>
    private void ValidateProductStockUuid(Guid stockUuid)
    {
        if (stockUuid == Guid.Empty)
            throw new DomainException("商品在庫識別IDが不正です");
    }

    /// <summary>
    /// 商品在庫数の検証
    /// </summary>
    private int ValidateQuantity(int quantity)
    {
        if (quantity < 0)
            throw new DomainException("在庫数は0以上で入力してください");

        return quantity;
    }
    /// <summary>
    /// 等価性（IDによる比較）
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not ProductStock other) return false;
        return StockUuid == other.StockUuid;
    }

    public override int GetHashCode() => StockUuid.GetHashCode();

    public override string ToString()
        => $"{StockUuid}: {Quantity}";
}