using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Applications
    .Usecases.ProductCategories;

/// <summary>
/// 商品カテゴリ一覧取得ユースケース。
/// </summary>
public interface IFindAllProductCategoriesUsecase
{
    /// <summary>
    /// 商品カテゴリをすべて取得する。
    /// </summary>
    /// <returns>商品カテゴリ一覧。</returns>
    Task<List<ProductCategory>> ExecuteAsync();
}