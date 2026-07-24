using A_exercise_EC_BE.Domain.Exceptions;
namespace A_exercise_EC_BE.Domain.Models;
/// <summary>
/// 注文明細を表すドメインオブジェクト
/// </summary>
public class OrdersDetail
{
    /// <summary>
    /// 注文明細ID
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 商品
    /// </summary>
    public Product Product { get; private set; }
    /// <summary>
    /// 合計金額
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OrdersDetail(int detailId, Product product, int count)
    {
        ValidateDetailId(detailId);
        Id = detailId;
        Product = product ?? throw new DomainException("商品は必須です。");
        Count = count;
        ValidateCount(count);
    }

    /// <summary>
    /// ID未定の注文明細を作成する場合のコンストラクタ
    /// </summary>
    public OrdersDetail(Product product, int count)
    {
        Product = product ?? throw new DomainException("商品は必須です。");
        ValidateCount(count);
        Count = count;
    }

    /// <summary>
    /// 注文明細IDの検証
    /// </summary>
    private void ValidateDetailId(int detailId)
    {
        if (detailId <= 0)
            throw new DomainException("注文明細IDが不正です。");
    }

    /// <summary>
    /// 合計金額の検証
    /// </summary>
    private int ValidateCount(int count)
    {
        if (count < 0)
            throw new DomainException("合計金額は0以上で入力してください");

        return count;
    }

    /// <summary>
    /// 等価性（IDによる比較）
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not OrdersDetail other) return false;

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
        => $"{Id}: {Count} / {Product.Name}";
}