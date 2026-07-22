using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Adapters;

/// <summary>
/// 商品Entityの集約から商品ドメインを復元する。
/// </summary>
public class ProductFactory(ProductEntityAdapter productAdapter)
{
    public Task<Product> RestoreAsync(ProductEntity target) =>
        productAdapter.RestoreAsync(target);

    public async Task<List<Product>> RestoreAsync(IEnumerable<ProductEntity> targets)
    {
        var products = new List<Product>();

        foreach (var target in targets)
        {
            products.Add(await RestoreAsync(target));
        }

        return products;
    }
}
