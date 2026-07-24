using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Entities;

namespace A_exercise_EC_BE.Infrastructures.Tests.Adapters;

[TestClass]
public class PaymentMethodEntityAdapterTests
{
    private PaymentMethodEntityAdapter _adapter = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _adapter = new PaymentMethodEntityAdapter();
    }

    [TestMethod]
    public async Task ConvertAsync_PaymentMethodをPaymentMethodEntityに変換できる()
    {
        // Arrange
        var paymentMethod = new PaymentMethod(
            "クレジットカード"
        );

        // Act
        var result = await _adapter.ConvertAsync(paymentMethod);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(paymentMethod.Name, result.Name);
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
    public async Task RestoreAsync_PaymentMethodEntityからPaymentMethodを復元できる()
    {
        // Arrange
        var entity = new PaymentMethodEntity
        {
            Id = 1,
            Name = "クレジットカード"
        };

        // Act
        var result = await _adapter.RestoreAsync(entity);

        // Assert
        Assert.IsNotNull(result);
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