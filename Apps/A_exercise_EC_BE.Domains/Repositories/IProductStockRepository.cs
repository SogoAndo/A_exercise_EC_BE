namespace A_exercise_EC_BE.Domains.Repositories;

/// <summary>
/// 商品在庫Repositoryのインターフェース。
/// </summary>
public interface IProductStockRepository
{
    /// <summary>
    /// 在庫が足りる場合に、
    /// 指定した数量分だけ在庫を減らす。
    /// </summary>
    /// <param name="productUuid">
    /// 商品UUID
    /// </param>
    /// <param name="quantity">
    /// 減らす数量
    /// </param>
    /// <returns>
    /// 在庫を減らせた場合はtrue。
    /// 在庫不足または商品在庫が存在しない場合はfalse。
    /// </returns>
    Task<bool> TryDecreaseAsync(
        Guid productUuid,
        int quantity
    );
}