using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Repositories;

namespace A_exercise_EC_BE.Applications.Usecases.Products;

/// <summary>
/// UC004 商品詳細取得UseCase。
/// </summary>
public sealed class GetProductDetailUsecase(
    IProductRepository productRepository)
    : IGetProductDetailUsecase
{
    /// <inheritdoc />
    public async Task<ProductDetailResult> GetAsync(
        Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new NotFoundException(
                "指定された商品は存在しません");
        }

        var product =
            await productRepository.FindByIdAsync(
                productId);

        if (product is null)
        {
            throw new NotFoundException(
                "指定された商品は存在しません");
        }

        var stock = product.ProductStock
            ?? throw new InternalException(
                "商品在庫情報が登録されていません。");

        return new ProductDetailResult(
            product.ProductUuid,
            product.Name,
            product.Price,
            product.ImageUrl,
            stock.Quantity);
    }
}
