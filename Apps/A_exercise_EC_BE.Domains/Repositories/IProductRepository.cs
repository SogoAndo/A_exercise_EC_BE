using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Repositories;

/// <summary>
/// EC向けの商品参照操作。
/// 実装では削除済み商品を結果に含めない。
/// </summary>
public interface IProductRepository
{
    Task<List<Product>> FindAllAsync();

    Task<List<Product>> SelectByProductCategoryIdAsync(Guid productCategoryUuid);

    Task<Product?> FindByIdAsync(Guid productUuid);
}
