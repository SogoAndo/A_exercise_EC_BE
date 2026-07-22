using A_exercise_EC_BE.Domain.Adapters;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Adapters;

public class ProductEntityAdapter(
    ProductCategoryEntityAdapter categoryAdapter,
    ProductStockEntityAdapter stockAdapter)
    : IRestorer<Product, ProductEntity>
{
    public async Task<Product> RestoreAsync(ProductEntity target)
    {
        _ = target ?? throw new InternalException("商品の復元対象がnullです。");

        if (target.ProductCategory is null)
        {
            throw new InternalException("商品カテゴリが読み込まれていません。");
        }

        if (target.ProductStock is null)
        {
            throw new InternalException("商品在庫が読み込まれていません。");
        }

        var category = await categoryAdapter.RestoreAsync(target.ProductCategory);
        var stock = await stockAdapter.RestoreAsync(target.ProductStock);

        return new Product(
            target.ProductUuid,
            target.Name,
            target.Price,
            target.ImageUrl,
            category,
            stock,
            target.DeleteFlg);
    }
}
