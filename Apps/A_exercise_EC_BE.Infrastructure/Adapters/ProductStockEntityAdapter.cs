using A_exercise_EC_BE.Domain.Adapters;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Adapters;

public class ProductStockEntityAdapter
    : IRestorer<ProductStock, ProductStockEntity>
{
    public Task<ProductStock> RestoreAsync(ProductStockEntity target)
    {
        _ = target ?? throw new InternalException("商品在庫の復元対象がnullです。");

        return Task.FromResult(
            new ProductStock(target.StockUuid, target.Quantity));
    }
}
