using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Tests.Adapters;

[TestClass]
public class ProductEntityAdapterTests
{
    private ProductEntityAdapter _adapter = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _adapter = new ProductEntityAdapter();
    }

    [TestMethod]
    public async Task ConvertAsync_ProductをProductEntityに変換できる()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "りんご",
            150
        );

        // Act
        var result = await _adapter.ConvertAsync(product);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(product.ProductUuid, result.ProductUuid);
        Assert.AreEqual(product.Name, result.Name);
        Assert.AreEqual(product.Price, result.Price);
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
    public async Task RestoreAsync_ProductEntityからProductを復元できる()
    {
        // Arrange
        var entity = new ProductEntity
        {
            ProductUuid = Guid.NewGuid(),
            Name = "りんご",
            Price = 150
        };

        // Act
        var result = await _adapter.RestoreAsync(entity);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(entity.ProductUuid, result.ProductUuid);
        Assert.AreEqual(entity.Name, result.Name);
        Assert.AreEqual(entity.Price, result.Price);
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