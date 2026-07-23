using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Contexts;
using A_exercise_EC_BE.Infrastructure.Entities;
using A_exercise_EC_BE.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructure.Tests.Repositories;

[TestClass]
[TestCategory("Infrastructure/Repositories")]
public class CustomerRepositoryTests
{
    private AppDbContext _context = null!;
    private CustomerRepository _repository = null!;
    private Guid _customerUuid;

    [TestInitialize]
    public async Task Initialize()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CustomerRepository(_context, new CustomerEntityAdapter());
        _customerUuid = Guid.NewGuid();

        _context.Customers.Add(CreateEntity());
        await _context.SaveChangesAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _context.DisposeAsync();
    }

    [TestMethod]
    public async Task FindByMailAddressAsync_WithMatch_ReturnsCustomer()
    {
        var customer = await _repository.FindByMailAddressAsync("taro@example.com");

        Assert.IsNotNull(customer);
        Assert.AreEqual(_customerUuid, customer.CustomerUuid);
        Assert.AreEqual("taro@example.com", customer.MailAddress);
        Assert.AreEqual("hashed-password", customer.Password);
    }

    [TestMethod]
    public async Task FindByMailAddressAsync_WithoutMatch_ReturnsNull()
    {
        var customer = await _repository.FindByMailAddressAsync("nobody@example.com");

        Assert.IsNull(customer);
    }

    private CustomerEntity CreateEntity() => new()
    {
        Id = 1,
        CustomerUuid = _customerUuid,
        Name = "山田太郎",
        Kana = "ヤマダタロウ",
        Address1 = "東京都千代田区",
        Address2 = null,
        PhoneNumber = "09012345678",
        MailAddress = "taro@example.com",
        Username = "taro",
        Password = "hashed-password",
        CreatedAt = new DateTime(2026, 7, 23, 10, 0, 0)
    };
}
