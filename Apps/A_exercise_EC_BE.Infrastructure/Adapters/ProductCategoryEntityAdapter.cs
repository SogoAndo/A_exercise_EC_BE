using A_exercise_EC_BE.Domain.Adapters;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Adapters;

public class ProductCategoryEntityAdapter
    : IRestorer<ProductCategory, ProductCategoryEntity>
{
    public Task<ProductCategory> RestoreAsync(ProductCategoryEntity target)
    {
        _ = target ?? throw new InternalException("商品カテゴリの復元対象がnullです。");

        return Task.FromResult(
            new ProductCategory(target.CategoryUuid, target.Name));
    }
}
