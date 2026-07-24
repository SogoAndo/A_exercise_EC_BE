using A_exercise_EC_BE.Exceptions;
using A_exercise_EC_BE.Models;

namespace A_exercise_EC_BE.Tests.Models;

[TestClass]
[TestCategory("Domain/Models")]
public class CustomerTests
{
    [TestMethod]
    public void Constructor_WithValidValues_CreatesCustomer()
    {
        var customerUuid = Guid.NewGuid();
        var createdAt = new DateTime(2026, 7, 23, 10, 0, 0);

        var customer = CreateCustomer(customerUuid, createdAt: createdAt);

        Assert.AreEqual(customerUuid, customer.CustomerUuid);
        Assert.AreEqual("山田太郎", customer.Name);
        Assert.AreEqual("ヤマダタロウ", customer.Kana);
        Assert.AreEqual("taro@example.com", customer.MailAddress);
        Assert.AreEqual("hashed-password", customer.Password);
        Assert.AreEqual(createdAt, customer.CreatedAt);
    }

    [TestMethod]
    public void Constructor_WithoutOptionalAddress_CreatesCustomer()
    {
        var customer = CreateCustomer(address2: null);

        Assert.IsNull(customer.Address2);
    }

    [TestMethod]
    public void Constructor_WithoutKana_ThrowsDomainException()
    {
        Assert.ThrowsExactly<DomainException>(
            () => CreateCustomer(kana: null!));
    }

    [TestMethod]
    public void Constructor_WithEmptyUuid_ThrowsDomainException()
    {
        Assert.ThrowsExactly<DomainException>(
            () => CreateCustomer(customerUuid: Guid.Empty));
    }

    [TestMethod]
    public void Constructor_WithoutPasswordHash_ThrowsDomainException()
    {
        Assert.ThrowsExactly<DomainException>(
            () => CreateCustomer(passwordHash: string.Empty));
    }

    [TestMethod]
    public void Constructor_WithDefaultCreatedAt_ThrowsDomainException()
    {
        Assert.ThrowsExactly<DomainException>(
            () => CreateCustomer(createdAt: DateTime.MinValue));
    }

    private static Customer CreateCustomer(
        Guid? customerUuid = null,
        string kana = "ヤマダタロウ",
        string? address2 = "101号室",
        string passwordHash = "hashed-password",
        DateTime? createdAt = null) => new(
            customerUuid ?? Guid.NewGuid(),
            "山田太郎",
            kana,
            "東京都千代田区",
            address2,
            "09012345678",
            "taro@example.com",
            "taro",
            passwordHash,
            createdAt ?? new DateTime(2026, 7, 23, 10, 0, 0));
}
