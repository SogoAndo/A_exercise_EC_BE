using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Tests.Adapters;

[TestClass]
public class ProductStockEntityAdapterTests
{
    private ProductStockEntityAdapter _adapter = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _adapter = new ProductStockEntityAdapter();
    }

    [TestMethod]
    public async Task ConvertAsync_ProductStockをProductStockEntityに変換できる()
    {
        // Arrange
        var productStock = new ProductStock(
            Guid.NewGuid(),
            10
        );

        // Act
        var result = await _adapter.ConvertAsync(productStock);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(productStock.StockUuid, result.StockUuid);
        Assert.AreEqual(productStock.Quantity, result.Quantity);
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
    public async Task RestoreAsync_ProductStockEntityからProductStockを復元できる()
    {
        // Arrange
        var entity = new ProductStockEntity
        {
            StockUuid = Guid.NewGuid(),
            Quantity = 10
        };

        // Act
        var result = await _adapter.RestoreAsync(entity);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(entity.StockUuid, result.StockUuid);
        Assert.AreEqual(entity.Quantity, result.Quantity);
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
}