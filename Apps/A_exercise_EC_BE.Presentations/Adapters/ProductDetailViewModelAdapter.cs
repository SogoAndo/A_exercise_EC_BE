using A_exercise_EC_BE.Applications.Usecases.Products;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Presentations.ViewModels.Products;

namespace A_exercise_EC_BE.Presentations.Adapters;

/// <summary>
/// UC004の商品詳細取得結果をViewModelへ変換する。
/// </summary>
public sealed class ProductDetailViewModelAdapter
{
    /// <summary>
    /// 商品詳細取得結果をAPIの応答へ変換する。
    /// </summary>
    public ProductDetailViewModel ConvertToViewModel(
        ProductDetailResult result)
    {
        _ = result
            ?? throw new InternalException(
                "引数resultがnullです。");

        return new ProductDetailViewModel(
            result.ProductUuid,
            result.ProductName,
            result.Price,
            result.ProductImage,
            result.StockQuantity);
    }
}
