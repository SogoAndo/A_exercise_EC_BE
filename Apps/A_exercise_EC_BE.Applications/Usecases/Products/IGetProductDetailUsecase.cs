namespace A_exercise_EC_BE.Applications.Usecases.Products;

/// <summary>
/// UC004 商品詳細取得UseCaseのインターフェース。
/// </summary>
public interface IGetProductDetailUsecase
{
    /// <summary>
    /// 商品UUIDを指定して、商品詳細と現在の在庫数を取得する。
    /// </summary>
    /// <param name="productId">商品UUID。</param>
    /// <returns>商品詳細。</returns>
    Task<ProductDetailResult> GetAsync(
        Guid productId);
}
