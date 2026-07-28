using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Entities;

namespace A_exercise_EC_BE.Infrastructures.Tests.Adapters;

[TestClass]
public class OrderStatusEntityAdapterTests
{
    private OrderStatusEntityAdapter _adapter = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _adapter = new OrderStatusEntityAdapter();
    }

    [TestMethod]
    public async Task ConvertAsync_OrderStatusをOrderStatusEntityに変換できる()
    {
        // Arrange
        var orderStatus = new OrderStatus(
            1,
            "注文受付"
        );

        // Act
        var result = await _adapter.ConvertAsync(orderStatus);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(orderStatus.Id, result.Id);
        Assert.AreEqual(orderStatus.Name, result.Name);
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
    public async Task RestoreAsync_OrderStatusEntityからOrderStatusを復元できる()
    {
        // Arrange
        var entity = new OrderStatusEntity
        {
            Id = 1,
            Name = "注文受付"
        };

        // Act
        var result = await _adapter.RestoreAsync(entity);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(entity.Id, result.Id);
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