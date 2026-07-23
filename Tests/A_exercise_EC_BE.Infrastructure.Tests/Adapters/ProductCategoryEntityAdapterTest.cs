using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Tests.Adapters;

[TestClass]
public class ProductCategoryEntityAdapterTests
{
    private ProductCategoryEntityAdapter _adapter = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _adapter = new ProductCategoryEntityAdapter();
    }

    [TestMethod]
    public async Task ConvertAsync_ProductCategoryをProductCategoryEntityに変換できる()
    {
        // Arrange
        var category = new ProductCategory(
            Guid.NewGuid(),
            "食品"
        );

        // Act
        var result = await _adapter.ConvertAsync(category);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(category.CategoryUuid, result.CategoryUuid);
        Assert.AreEqual(category.Name, result.Name);
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
    public async Task RestoreAsync_ProductCategoryEntityからProductCategoryを復元できる()
    {
        // Arrange
        var entity = new ProductCategoryEntity
        {
            CategoryUuid = Guid.NewGuid(),
            Name = "食品"
        };

        // Act
        var result = await _adapter.RestoreAsync(entity);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(entity.CategoryUuid, result.CategoryUuid);
        Assert.AreEqual(entity.Name, result.Name);
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