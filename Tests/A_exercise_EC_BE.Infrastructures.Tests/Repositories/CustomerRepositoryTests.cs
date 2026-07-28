using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Entities;
using A_exercise_EC_BE.Infrastructures.Repositories;
using A_exercise_EC_BE.Presentations.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    /// </summary>
    private static ServiceProvider provider = null!;

    /// <summary>
    /// テストクラス全体の初期化。
    /// </summary>
    [ClassInitialize]
    public static void ClassInitialize(
        TestContext testContext)
    {
        _ = testContext;

        var config =
            new ConfigurationBuilder()
                .SetBasePath(
                    AppContext.BaseDirectory)
                .AddJsonFile(
                    "appsettings.json",
                    optional: false)
                .AddJsonFile(
                    "appsettings.Test.json",
                    optional: true)
                .AddEnvironmentVariables()
                .Build();

        provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(config);
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
                .GetRequiredService<
                    ICustomerRepository>();

        dbContext =
            scope.ServiceProvider
                .GetRequiredService<
                    AppDbContext>();
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
    /// 存在するメールアドレスで
    /// 顧客を取得できることを確認する。
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

        var username =
            CreateUniqueUsername(
                "find",
                customerUuid);

        var mailAddress =
            CreateUniqueMailAddress(
                "find",
                customerUuid);

        var entity =
            CreateCustomerEntity(
                customerUuid,
                username,
                mailAddress);

        try
        {
            dbContext.Customers.Add(
                entity);

            await dbContext
                .SaveChangesAsync();

            // Act
            var result =
                await repository
                    .FindByMailAddressAsync(
                        mailAddress);

            // Assert
            Assert.IsNotNull(
                result);

            Assert.AreEqual(
                customerUuid,
                result.CustomerUuid);

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
    /// 存在しないメールアドレスで
    /// nullが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しないメールアドレスを指定した場合はnullを返す")]
    public async Task
        FindByMailAddressAsync_WhenCustomerDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var mailAddress =
            CreateUniqueMailAddress(
                "notfound",
                Guid.NewGuid());

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
    /// DB接続エラー時にInternalExceptionが
    /// 発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindByMailAddressAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        FindByMailAddressAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<
                AppDbContext>()
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
                .GetRequiredService<
                    CustomerEntityAdapter>();

        var errorRepository =
            new CustomerRepository(
                context,
                adapter);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .FindByMailAddressAsync(
                                "error@example.com");
                    });

        // Assert
        Assert.IsNotNull(
            exception);
    }

    /*
     * ExistsByUsernameAsync
     */

    /// <summary>
    /// 存在するアカウント名で
    /// trueが返ることを確認する。
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
            CreateUniqueUsername(
                "exists",
                customerUuid);

        var mailAddress =
            CreateUniqueMailAddress(
                "exists-user",
                customerUuid);

        var entity =
            CreateCustomerEntity(
                customerUuid,
                username,
                mailAddress);

        try
        {
            dbContext.Customers.Add(
                entity);

            await dbContext
                .SaveChangesAsync();

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
    /// 存在しないアカウント名で
    /// falseが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しないアカウント名を指定した場合はfalseを返す")]
    public async Task
        ExistsByUsernameAsync_WhenUsernameDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var username =
            CreateUniqueUsername(
                "none",
                Guid.NewGuid());

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
    /// DB接続エラー時にInternalExceptionが
    /// 発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExistsByUsernameAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        ExistsByUsernameAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<
                AppDbContext>()
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
                .GetRequiredService<
                    CustomerEntityAdapter>();

        var errorRepository =
            new CustomerRepository(
                context,
                adapter);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .ExistsByUsernameAsync(
                                "database-error-user");
                    });

        // Assert
        Assert.IsNotNull(
            exception);
    }

    /*
     * ExistsByMailAddressAsync
     */

    /// <summary>
    /// 存在するメールアドレスで
    /// trueが返ることを確認する。
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
            CreateUniqueUsername(
                "mail",
                customerUuid);

        var mailAddress =
            CreateUniqueMailAddress(
                "exists-mail",
                customerUuid);

        var entity =
            CreateCustomerEntity(
                customerUuid,
                username,
                mailAddress);

        try
        {
            dbContext.Customers.Add(
                entity);

            await dbContext
                .SaveChangesAsync();

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
    /// 存在しないメールアドレスで
    /// falseが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しないメールアドレスを指定した場合はfalseを返す")]
    public async Task
        ExistsByMailAddressAsync_WhenMailAddressDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var mailAddress =
            CreateUniqueMailAddress(
                "notfound",
                Guid.NewGuid());

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
    /// DB接続エラー時にInternalExceptionが
    /// 発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExistsByMailAddressAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        ExistsByMailAddressAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<
                AppDbContext>()
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
                .GetRequiredService<
                    CustomerEntityAdapter>();

        var errorRepository =
            new CustomerRepository(
                context,
                adapter);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .ExistsByMailAddressAsync(
                                "database-error@example.com");
                    });

        // Assert
        Assert.IsNotNull(
            exception);
    }

    /*
     * CreateAsync
     */

    /// <summary>
    /// 顧客アカウントを正常に
    /// 登録できることを確認する。
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
            CreateUniqueUsername(
                "create",
                customerUuid);

        var mailAddress =
            CreateUniqueMailAddress(
                "create",
                customerUuid);

        var customer =
            CreateCustomer(
                customerUuid,
                username,
                mailAddress);

        try
        {
            // Act
            await repository
                .CreateAsync(customer);

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
                customerUuid,
                saved.CustomerUuid);

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
    /// DB接続エラー時にInternalExceptionが
    /// 発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "CreateAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        CreateAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
    {
        // Arrange
        var options =
            new DbContextOptionsBuilder<
                AppDbContext>()
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
                .GetRequiredService<
                    CustomerEntityAdapter>();

        var errorRepository =
            new CustomerRepository(
                context,
                adapter);

        var customerUuid =
            Guid.NewGuid();

        var username =
            CreateUniqueUsername(
                "dberr",
                customerUuid);

        var mailAddress =
            CreateUniqueMailAddress(
                "db-error",
                customerUuid);

        var customer =
            CreateCustomer(
                customerUuid,
                username,
                mailAddress);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .CreateAsync(customer);
                    });

        // Assert
        Assert.IsNotNull(
            exception);
    }

    /*
     * テストデータ生成
     */

    /// <summary>
    /// テスト用の一意なアカウント名を生成する。
    /// </summary>
    private static string CreateUniqueUsername(
        string prefix,
        Guid customerUuid)
    {
        var suffix =
            customerUuid
                .ToString("N")[..12];

        return $"{prefix}{suffix}";
    }

    /// <summary>
    /// テスト用の一意なメールアドレスを生成する。
    /// </summary>
    private static string CreateUniqueMailAddress(
        string prefix,
        Guid customerUuid)
    {
        return
            $"{prefix}-{customerUuid:N}@example.com";
    }

    /// <summary>
    /// テスト用のCustomerEntityを生成する。
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

            Name =
                "テスト顧客",

            Kana =
                "テストコキャク",

            Address1 =
                "東京都新宿区1-1-1",

            Address2 =
                "テストマンション101",

            PhoneNumber =
                "09012345678",

            MailAddress =
                mailAddress,

            Username =
                username,

            Password =
                "test-password-hash",

            CreatedAt =
                DateTime.Now
                    .AddMinutes(-1)
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
        /*
         * SaveChangesAsyncで失敗した場合でも
         * Added状態のEntityを残さないようにする。
         */
        dbContext
            .ChangeTracker
            .Clear();

        /*
         * Entityを追跡せず、
         * CustomerUuidを条件に直接削除する。
         *
         * 対象が存在しない場合も例外にはならない。
         */
        await dbContext.Customers
            .Where(
                customer =>
                    customer.CustomerUuid
                    == customerUuid)
            .ExecuteDeleteAsync();
    }
}