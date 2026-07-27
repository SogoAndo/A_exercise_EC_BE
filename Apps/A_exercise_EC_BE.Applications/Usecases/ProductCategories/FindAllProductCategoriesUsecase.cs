using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;

namespace A_exercise_EC_BE.Applications
    .Usecases.ProductCategories;

/// <summary>
/// 商品カテゴリ一覧取得ユースケース。
/// </summary>
public class FindAllProductCategoriesUsecase(
    IProductCategoryRepository
        productCategoryRepository)
    : IFindAllProductCategoriesUsecase
{
    /// <inheritdoc />
    public async Task<List<ProductCategory>>
        ExecuteAsync()
    {
        return await productCategoryRepository
            .FindAllAsync();
    }
}