using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Repositories;

/// <summary>
/// 商品カテゴリRepositoryインターフェース
/// </summary>
public interface IProductCategoryRepository
{
    /// <summary>
    /// すべての商品カテゴリを取得する
    /// </summary>
    /// <returns>商品カテゴリ一覧</returns>
    Task<List<ProductCategory>> FindAllAsync();
}