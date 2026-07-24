using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Entities;

namespace A_exercise_EC_BE.Infrastructures.Tests.Adapters;

[TestClass]
public class OrdersDetailEntityAdapterTests
{
    private OrdersDetailEntityAdapter _adapter = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _adapter = new OrdersDetailEntityAdapter();
    }

    [TestMethod]
    public async Task ConvertAsync_引数がnullの場合InternalExceptionをthrowする()
    {
        // Act & Assert
        var ex = await Assert.ThrowsExactlyAsync<InternalException>(
            async () => await _adapter.ConvertAsync(null!)
        );

        Assert.AreEqual("引数domainがnullです。", ex.Message);
    }

    [TestMethod]
    public async Task RestoreAsync_OrdersDetailEntityからOrdersDetailを復元できる()
    {
        // Arrange
        var productUuid = Guid.NewGuid();

        var entity = new OrdersDetailEntity
        {
            Id = 1,
            Count = 3,
            Product = new ProductEntity
            {
                ProductUuid = productUuid,
                Name = "テスト商品",
                Price = 1000,
                ImageUrl = "https://example.com/product.png",
                DeleteFlg = 0
            }
        };

        // Act
        var result = await _adapter.RestoreAsync(entity);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(entity.Id, result.Id);
        Assert.AreEqual(entity.Count, result.Count);

        Assert.IsNotNull(result.Product);
        Assert.AreEqual(productUuid, result.Product.ProductUuid);
        Assert.AreEqual("テスト商品", result.Product.Name);
    }

    [TestMethod]
    public async Task RestoreAsync_引数がnullの場合InternalExceptionをthrowする()
    {
        // Act & Assert
        var ex = await Assert.ThrowsExactlyAsync<InternalException>(
            async () => await _adapter.RestoreAsync(null!)
        );

        Assert.AreEqual("引数targetがnullです。", ex.Message);
    }

    [TestMethod]
    public async Task RestoreAsync_Productがnullの場合InternalExceptionをthrowする()
    {
        // Arrange
        var entity = new OrdersDetailEntity
        {
            Id = 1,
            Count = 3,
            Product = null!
        };

        // Act
        var ex = await Assert.ThrowsExactlyAsync<InternalException>(
            async () => await _adapter.RestoreAsync(entity)
        );

        // Assert
        Assert.AreEqual(
            "注文明細の商品が取得できていません。",
            ex.Message);
    }
}