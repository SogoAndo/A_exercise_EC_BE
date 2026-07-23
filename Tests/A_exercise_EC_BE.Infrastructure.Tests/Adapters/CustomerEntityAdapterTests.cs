using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Tests.Adapters;

[TestClass]
[TestCategory("Infrastructure/Adapters")]
public class CustomerEntityAdapterTests
{
    private readonly CustomerEntityAdapter _adapter = new();

    [TestMethod]
    public async Task RestoreAsync_WithCustomerEntity_RestoresCustomer()
    {
        var entity = CreateEntity();

        var customer = await _adapter.RestoreAsync(entity);

        Assert.AreEqual(entity.CustomerUuid, customer.CustomerUuid);
        Assert.AreEqual(entity.Name, customer.Name);
        Assert.AreEqual(entity.Kana, customer.Kana);
        Assert.AreEqual(entity.MailAddress, customer.MailAddress);
        Assert.AreEqual(entity.PasswordHash, customer.PasswordHash);
        Assert.AreEqual(entity.CreatedAt, customer.CreatedAt);
    }

    [TestMethod]
    public async Task RestoreAsync_WithNull_ThrowsInternalException()
    {
        await Assert.ThrowsExactlyAsync<InternalException>(
            () => _adapter.RestoreAsync(null!));
    }

    internal static CustomerEntity CreateEntity() => new()
    {
        Id = 1,
        CustomerUuid = Guid.NewGuid(),
        Name = "山田太郎",
        Kana = "ヤマダタロウ",
        Address1 = "東京都千代田区",
        Address2 = "101号室",
        PhoneNumber = "09012345678",
        MailAddress = "taro@example.com",
        Username = "taro",
        PasswordHash = "hashed-password",
        CreatedAt = new DateTime(2026, 7, 23, 10, 0, 0)
    };
}
