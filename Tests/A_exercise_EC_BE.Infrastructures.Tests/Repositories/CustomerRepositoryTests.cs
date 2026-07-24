using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Entities;
using A_exercise_EC_BE.Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using A_exercise_EC_BE.Presentations.Configs;

namespace A_exercise_EC_BE.Infrastructures.Tests.Repositories;

/// <summary>
/// 顧客Repositoryのテスト。
/// </summary>
[TestClass]
[DoNotParallelize]
[TestCategory("Infrastructure/Repositories")]
public class CustomerRepositoryTests
{
    /// <summary>
    /// DI取得用スコープ。
    /// </summary>
    private IServiceScope scope = null!;

    /// <summary>
    /// テスト対象Repository。
    /// </summary>
    private ICustomerRepository repository = null!;

    /// <summary>
    /// テストデータ操作用DbContext。
    /// </summary>
    private AppDbContext dbContext = null!;

    /// <summary>
    /// ServiceProvider。
    ///
    /// ProductRepositoryTestsで使用している
    /// providerを共通化できる場合はそちらを使用してください。
    /// </summary>
    private static ServiceProvider provider = null!;

    /// <summary>
    /// テストクラス全体の初期化。
    /// </summary>
    [ClassInitialize]
    public static void ClassInitialize(
        TestContext testContext)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(
                "appsettings.json",
                optional: false)
            .AddJsonFile(
                "appsettings.json",
                optional: true)
            .AddEnvironmentVariables()
            .Build();

        provider = ApplicationDependencyExtensions.BuildAppProvider(config);
    }

    /// <summary>
    /// テストクラス全体の終了処理。
    /// </summary>
    [ClassCleanup]
    public static void ClassCleanup()
    {
        provider?.Dispose();
    }

    /// <summary>
    /// 各テストの初期化。
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        scope =
            provider.CreateScope();

        repository =
            scope.ServiceProvider
                .GetRequiredService<ICustomerRepository>();

        dbContext =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();
    }

    /// <summary>
    /// 各テストの終了処理。
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        scope?.Dispose();
    }

    /*
     * FindByMailAddressAsync
     */

    /// <summary>
    /// 存在するメールアドレスで顧客を取得できることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在するメールアドレスを指定した場合は顧客を取得できる")]
    public async Task
        FindByMailAddressAsync_WhenCustomerExists_ShouldReturnCustomer()
    {
        // Arrange
        var customerUuid =
            Guid.NewGuid();

        var mailAddress =
            $"find-{customerUuid:N}@example.com";

        var username =
            $"find";

        var entity =
            CreateCustomerEntity(
                customerUuid,
                username,
                mailAddress);

        dbContext.Customers.Add(
            entity);

        await dbContext.SaveChangesAsync();

        try
        {
            // Act
            var result =
                await repository
                    .FindByMailAddressAsync(
                        mailAddress);

            // Assert
            Assert.IsNotNull(
                result);

            Assert.AreEqual(
                mailAddress,
                result.MailAddress);

            Assert.AreEqual(
                username,
                result.Username);
        }
        finally
        {
            await DeleteCustomerAsync(
                customerUuid);
        }
    }

    /// <summary>
    /// 存在しないメールアドレスでnullが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しないメールアドレスを指定した場合はnullを返す")]
    public async Task
        FindByMailAddressAsync_WhenCustomerDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var mailAddress =
            $"not-exist-{Guid.NewGuid():N}@example.com";

        // Act
        var result =
            await repository
                .FindByMailAddressAsync(
                    mailAddress);

        // Assert
        Assert.IsNull(
            result);
    }

    /// <summary>
    /// DB接続エラー時にInternalExceptionが発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindByMailAddressAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        FindByMailAddressAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(
                    "Host=localhost;"
                    + "Port=9999;"
                    + "Database=All_Exercise;"
                    + "Username=postgres;"
                    + "Password=postgres")
                .Options;

        await using var context =
            new AppDbContext(options);

        var adapter =
            scope.ServiceProvider
                .GetRequiredService<CustomerEntityAdapter>();

        var errorRepository =
            new CustomerRepository(
                context,
                adapter);

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InternalException>(
            async () =>
            {
                await errorRepository
                    .FindByMailAddressAsync(
                        "error@example.com");
            });
    }

    /*
     * ExistsByUsernameAsync
     */

    /// <summary>
    /// 存在するアカウント名でtrueが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在するアカウント名を指定した場合はtrueを返す")]
    public async Task
        ExistsByUsernameAsync_WhenUsernameExists_ShouldReturnTrue()
    {
        // Arrange
        var customerUuid =
            Guid.NewGuid();

        var username =
            $"exists-user";

        var mailAddress =
            $"exists-user-{customerUuid:N}@example.com";

        var entity =
            CreateCustomerEntity(
                customerUuid,
                username,
                mailAddress);

        dbContext.Customers.Add(
            entity);

        await dbContext.SaveChangesAsync();

        try
        {
            // Act
            var result =
                await repository
                    .ExistsByUsernameAsync(
                        username);

            // Assert
            Assert.IsTrue(
                result);
        }
        finally
        {
            await DeleteCustomerAsync(
                customerUuid);
        }
    }

    /// <summary>
    /// 存在しないアカウント名でfalseが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しないアカウント名を指定した場合はfalseを返す")]
    public async Task
        ExistsByUsernameAsync_WhenUsernameDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var username =
            $"not-exist-{Guid.NewGuid():N}";

        // Act
        var result =
            await repository
                .ExistsByUsernameAsync(
                    username);

        // Assert
        Assert.IsFalse(
            result);
    }

    /// <summary>
    /// DB接続エラー時にInternalExceptionが発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExistsByUsernameAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        ExistsByUsernameAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(
                    "Host=localhost;"
                    + "Port=9999;"
                    + "Database=All_Exercise;"
                    + "Username=postgres;"
                    + "Password=postgres")
                .Options;

        await using var context =
            new AppDbContext(options);

        var adapter =
            scope.ServiceProvider
                .GetRequiredService<CustomerEntityAdapter>();

        var errorRepository =
            new CustomerRepository(
                context,
                adapter);

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InternalException>(
            async () =>
            {
                await errorRepository
                    .ExistsByUsernameAsync(
                        "database-error-user");
            });
    }

    /*
     * ExistsByMailAddressAsync
     */

    /// <summary>
    /// 存在するメールアドレスでtrueが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在するメールアドレスを指定した場合はtrueを返す")]
    public async Task
        ExistsByMailAddressAsync_WhenMailAddressExists_ShouldReturnTrue()
    {
        // Arrange
        var customerUuid =
            Guid.NewGuid();

        var username =
            $"exists-mail";

        var mailAddress =
            $"exists-mail-{customerUuid:N}@example.com";

        var entity =
            CreateCustomerEntity(
                customerUuid,
                username,
                mailAddress);

        dbContext.Customers.Add(
            entity);

        await dbContext.SaveChangesAsync();

        try
        {
            // Act
            var result =
                await repository
                    .ExistsByMailAddressAsync(
                        mailAddress);

            // Assert
            Assert.IsTrue(
                result);
        }
        finally
        {
            await DeleteCustomerAsync(
                customerUuid);
        }
    }

    /// <summary>
    /// 存在しないメールアドレスでfalseが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しないメールアドレスを指定した場合はfalseを返す")]
    public async Task
        ExistsByMailAddressAsync_WhenMailAddressDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var mailAddress =
            $"not-exist-{Guid.NewGuid():N}@example.com";

        // Act
        var result =
            await repository
                .ExistsByMailAddressAsync(
                    mailAddress);

        // Assert
        Assert.IsFalse(
            result);
    }

    /// <summary>
    /// DB接続エラー時にInternalExceptionが発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExistsByMailAddressAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        ExistsByMailAddressAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(
                    "Host=localhost;"
                    + "Port=9999;"
                    + "Database=All_Exercise;"
                    + "Username=postgres;"
                    + "Password=postgres")
                .Options;

        await using var context =
            new AppDbContext(options);

        var adapter =
            scope.ServiceProvider
                .GetRequiredService<CustomerEntityAdapter>();

        var errorRepository =
            new CustomerRepository(
                context,
                adapter);

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InternalException>(
            async () =>
            {
                await errorRepository
                    .ExistsByMailAddressAsync(
                        "database-error@example.com");
            });
    }

    /*
     * CreateAsync
     */

    /// <summary>
    /// 顧客アカウントを正常に登録できることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "有効な顧客アカウントを正常に登録できる")]
    public async Task
        CreateAsync_WhenCustomerIsValid_ShouldCreateCustomer()
    {
        // Arrange
        var customerUuid =
            Guid.NewGuid();

        var username =
            $"create-{customerUuid:N}";

        var mailAddress =
            $"create-{customerUuid:N}@example.com";

        var customer =
            CreateCustomer(
                customerUuid,
                username,
                mailAddress);

        try
        {
            // Act
            await repository.CreateAsync(
                customer);

            // Assert
            var saved =
                await dbContext.Customers
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        entity =>
                            entity.CustomerUuid
                            == customerUuid);

            Assert.IsNotNull(
                saved);

            Assert.AreEqual(
                username,
                saved.Username);

            Assert.AreEqual(
                mailAddress,
                saved.MailAddress);
        }
        finally
        {
            await DeleteCustomerAsync(
                customerUuid);
        }
    }

    /// <summary>
    /// DB接続エラー時にInternalExceptionが発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "CreateAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        CreateAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(
                    "Host=localhost;"
                    + "Port=9999;"
                    + "Database=All_Exercise;"
                    + "Username=postgres;"
                    + "Password=postgres")
                .Options;

        await using var context =
            new AppDbContext(options);

        var adapter =
            scope.ServiceProvider
                .GetRequiredService<CustomerEntityAdapter>();

        var errorRepository =
            new CustomerRepository(
                context,
                adapter);

        var customerUuid =
            Guid.NewGuid();

        var customer =
            CreateCustomer(
                customerUuid,
                $"db-error-{customerUuid:N}",
                $"db-error-{customerUuid:N}@example.com");

        // Act & Assert
        await Assert.ThrowsExactlyAsync<InternalException>(
            async () =>
            {
                await errorRepository
                    .CreateAsync(customer);
            });
    }

    /*
     * テストデータ生成
     */

    /// <summary>
    /// CustomerEntityを生成する。
    /// </summary>
    private static CustomerEntity
        CreateCustomerEntity(
            Guid customerUuid,
            string username,
            string mailAddress)
    {
        return new CustomerEntity
        {
            CustomerUuid =
                customerUuid,

            Username =
                username,

            MailAddress =
                mailAddress,

            Password =
                "test-password-hash",

            Name =
                "テスト顧客",

            Kana =
                "テストコキャク"
        };
    }

    /// <summary>
    /// Customerドメインモデルを生成する。
    /// </summary>
    private static Customer CreateCustomer(
        Guid customerUuid,
        string username,
        string mailAddress)
    {
        return new Customer(
            customerUuid,
            "テスト顧客",
            "テストコキャク",
            "東京都新宿区1-1-1",
            "テストマンション101号室",
            "09012345678",
            mailAddress,
            username,
            "test-password-hash",
            DateTime.Now);
    }

    /// <summary>
    /// テストで登録した顧客を削除する。
    /// </summary>
    private async Task DeleteCustomerAsync(
        Guid customerUuid)
    {
        var entity =
            await dbContext.Customers
                .SingleOrDefaultAsync(
                    customer =>
                        customer.CustomerUuid
                        == customerUuid);

        if (entity is null)
        {
            return;
        }

        dbContext.Customers.Remove(
            entity);

        await dbContext.SaveChangesAsync();
    }
}