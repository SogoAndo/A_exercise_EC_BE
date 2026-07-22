using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Tests.Adapters;

[TestClass]
[TestCategory("Infrastructure/Adapters")]
public class ProductFactoryTests
{
    private readonly ProductFactory _factory = new(
        new ProductEntityAdapter(
            new ProductCategoryEntityAdapter(),
            new ProductStockEntityAdapter()));

    [TestMethod]
    public async Task RestoreAsync_WithAggregate_RestoresProduct()
    {
        var entity = CreateEntity();

        var product = await _factory.RestoreAsync(entity);

        Assert.AreEqual(entity.ProductUuid, product.ProductUuid);
        Assert.AreEqual(entity.Name, product.Name);
        Assert.AreEqual(entity.Price, product.Price);
        Assert.AreEqual(entity.ImageUrl, product.ImageUrl);
        Assert.AreEqual(entity.ProductCategory.CategoryUuid, product.ProductCategory.CategoryUuid);
        Assert.AreEqual(entity.ProductStock!.StockUuid, product.ProductStock.StockUuid);
        Assert.AreEqual(entity.ProductStock.Quantity, product.ProductStock.Quantity);
    }

    [TestMethod]
    public async Task RestoreAsync_WithoutCategory_ThrowsInternalException()
    {
        var entity = CreateEntity();
        entity.ProductCategory = null!;

        await Assert.ThrowsExactlyAsync<InternalException>(
            () => _factory.RestoreAsync(entity));
    }

    [TestMethod]
    public async Task RestoreAsync_WithoutStock_ThrowsInternalException()
    {
        var entity = CreateEntity();
        entity.ProductStock = null;

        await Assert.ThrowsExactlyAsync<InternalException>(
            () => _factory.RestoreAsync(entity));
    }

    private static ProductEntity CreateEntity() => new()
    {
        ProductUuid = Guid.NewGuid(),
        Name = "水性ボールペン",
        Price = 120,
        ImageUrl = null,
        DeleteFlg = 0,
        ProductCategory = new ProductCategoryEntity
        {
            CategoryUuid = Guid.NewGuid(),
            Name = "文房具"
        },
        ProductStock = new ProductStockEntity
        {
            StockUuid = Guid.NewGuid(),
            Quantity = 10
        }
    };
}
