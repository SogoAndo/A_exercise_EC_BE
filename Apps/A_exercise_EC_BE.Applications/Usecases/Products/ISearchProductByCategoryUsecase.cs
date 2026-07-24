using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Applications.Usecases.Products;
/// <summary>
/// ユースケース:[商品をカテゴリー検索をする]を実装するインターフェイス
/// </summary>

public interface ISearchProductByCategoryUsecase
{
    /// <summary>
    /// 指定されたカテゴリーの商品を返す
    /// </summary>
    /// <param name="productCategoryId"></param>
    /// <returns>カテゴリー検索結果</returns>
    Task<List<Product>>ExecuteAsync(Guid? productCategoryId);
}