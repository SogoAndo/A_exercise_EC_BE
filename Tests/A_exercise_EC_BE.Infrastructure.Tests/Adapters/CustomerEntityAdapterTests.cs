using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Tests.Adapters;

[TestClass]
public class CustomerEntityAdapterTests
{
    private CustomerEntityAdapter _adapter = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _adapter = new CustomerEntityAdapter();
    }

    [TestMethod]
    public async Task ConvertAsync_CustomerをCustomerEntityに変換できる()
    {
        // Arrange
        var customer = new Customer(
            Guid.NewGuid(),
            "山田太郎",
            "ヤマダタロウ",
            "東京都新宿区",
            "1-2-3",
            "090-1234-5678",
            "test@example.com",
            "yamada",
            "password",
            new DateTime(2026, 7, 9, 10, 0, 0)
        );

        // Act
        var result = await _adapter.ConvertAsync(customer);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(customer.CustomerUuid, result.CustomerUuid);
        Assert.AreEqual(customer.Name, result.Name);
        Assert.AreEqual(customer.Kana, result.Kana);
        Assert.AreEqual(customer.Address1, result.Address1);
        Assert.AreEqual(customer.Address2, result.Address2);
        Assert.AreEqual(customer.PhoneNumber, result.PhoneNumber);
        Assert.AreEqual(customer.MailAddress, result.MailAddress);
        Assert.AreEqual(customer.Username, result.Username);
        Assert.AreEqual(customer.Password, result.Password);
        Assert.AreEqual(customer.CreatedAt, result.CreatedAt);
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
    public async Task RestoreAsync_CustomerEntityからCustomerを復元できる()
    {
        // Arrange
        var entity = new CustomerEntity
        {
            CustomerUuid = Guid.NewGuid(),
            Name = "山田太郎",
            Kana = "ヤマダタロウ",
            Address1 = "東京都新宿区",
            Address2 = "1-2-3",
            PhoneNumber = "090-1234-5678",
            MailAddress = "test@example.com",
            Username = "yamada",
            Password = "password",
            CreatedAt = new DateTime(2026, 7, 9, 10, 0, 0)
        };

        // Act
        var result = await _adapter.RestoreAsync(entity);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(entity.CustomerUuid, result.CustomerUuid);
        Assert.AreEqual(entity.Name, result.Name);
        Assert.AreEqual(entity.Kana, result.Kana);
        Assert.AreEqual(entity.Address1, result.Address1);
        Assert.AreEqual(entity.Address2, result.Address2);
        Assert.AreEqual(entity.PhoneNumber, result.PhoneNumber);
        Assert.AreEqual(entity.MailAddress, result.MailAddress);
        Assert.AreEqual(entity.Username, result.Username);
        Assert.AreEqual(entity.Password, result.Password);
        Assert.AreEqual(entity.CreatedAt, result.CreatedAt);
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