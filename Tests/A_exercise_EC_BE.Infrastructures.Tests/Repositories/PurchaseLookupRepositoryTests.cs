using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Entities;
using A_exercise_EC_BE.Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructures.Tests.Repositories;

[TestClass]
[TestCategory("Infrastructure/Repositories")]
public class PurchaseLookupRepositoryTests
{
    [TestMethod]
    public async Task CustomerRepository_FindByCustomerUuidAsync_CoversAllResults()
    {
        await using var context = CreateContext();
        var customerUuid = Guid.NewGuid();
        context.Customers.Add(
            new CustomerEntity
            {
                Id = 1,
                CustomerUuid = customerUuid,
                Name = "顧客太郎",
                Kana = "コキャクタロウ",
                Address1 = "東京都千代田区",
                PhoneNumber = "09012345678",
                MailAddress = "customer@example.com",
                Username = "customer",
                Password = "hashed-password",
                CreatedAt = new DateTime(2026, 7, 1)
            });
        await context.SaveChangesAsync();
        var repository = new CustomerRepository(
            context,
            new CustomerEntityAdapter());

        var found = await repository.FindByCustomerUuidAsync(
            customerUuid);
        var missing = await repository.FindByCustomerUuidAsync(
            Guid.NewGuid());

        Assert.IsNotNull(found);
        Assert.AreEqual(customerUuid, found.CustomerUuid);
        Assert.IsNull(missing);
    }

    [TestMethod]
    public async Task CustomerRepository_FindByCustomerUuidAsync_WhenContextDisposed_ThrowsInternalException()
    {
        var context = CreateContext();
        var repository = new CustomerRepository(
            context,
            new CustomerEntityAdapter());
        await context.DisposeAsync();

        var exception = await Assert.ThrowsExactlyAsync<InternalException>(
            () => repository.FindByCustomerUuidAsync(Guid.NewGuid()));

        Assert.IsNotNull(exception.InnerException);
    }

    [TestMethod]
    public async Task OrderStatusRepository_FindByNameAsync_CoversAllResults()
    {
        await using var context = CreateContext();
        context.OrderStatuses.Add(
            new OrderStatusEntity
            {
                Id = 1,
                Name = "受付"
            });
        await context.SaveChangesAsync();
        var repository = new OrderStatusRepository(
            context,
            new OrderStatusEntityAdapter());

        var found = await repository.FindByNameAsync("受付");
        var missing = await repository.FindByNameAsync("存在しない");

        Assert.IsNotNull(found);
        Assert.AreEqual(1, found.Id);
        Assert.AreEqual("受付", found.Name);
        Assert.IsNull(missing);
    }

    [TestMethod]
    public async Task OrderStatusRepository_FindByNameAsync_WhenContextDisposed_ThrowsInternalException()
    {
        var context = CreateContext();
        var repository = new OrderStatusRepository(
            context,
            new OrderStatusEntityAdapter());
        await context.DisposeAsync();

        var exception = await Assert.ThrowsExactlyAsync<InternalException>(
            () => repository.FindByNameAsync("受付"));

        Assert.IsNotNull(exception.InnerException);
    }

    [TestMethod]
    public async Task PaymentMethodRepository_FindByIdAsync_CoversAllResults()
    {
        await using var context = CreateContext();
        context.PaymentMethods.Add(
            new PaymentMethodEntity
            {
                Id = 4,
                Name = "銀行振込"
            });
        await context.SaveChangesAsync();
        var repository = new PaymentMethodRepository(
            context,
            new PaymentMethodEntityAdapter());

        var found = await repository.FindByIdAsync(4);
        var missing = await repository.FindByIdAsync(999);

        Assert.IsNotNull(found);
        Assert.AreEqual(4, found.Id);
        Assert.AreEqual("銀行振込", found.Name);
        Assert.IsNull(missing);
    }

    [TestMethod]
    public async Task PaymentMethodRepository_FindByIdAsync_WhenContextDisposed_ThrowsInternalException()
    {
        var context = CreateContext();
        var repository = new PaymentMethodRepository(
            context,
            new PaymentMethodEntityAdapter());
        await context.DisposeAsync();

        var exception = await Assert.ThrowsExactlyAsync<InternalException>(
            () => repository.FindByIdAsync(4));

        Assert.IsNotNull(exception.InnerException);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;
        return new AppDbContext(options);
    }
}
