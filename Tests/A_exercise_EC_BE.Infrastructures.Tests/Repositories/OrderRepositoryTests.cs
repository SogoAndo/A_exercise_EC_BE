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

namespace A_exercise_EC_BE.Infrastructures.Tests.Repositories;

/// <summary>
/// 注文Repositoryのテスト。
/// </summary>
[TestClass]
[DoNotParallelize]
[TestCategory("Infrastructure/Repositories")]
public class OrderRepositoryTests
{
    /// <summary>
    /// DI設定済みServiceProvider。
    /// </summary>
    private static ServiceProvider? provider;

    /// <summary>
    /// テストごとのDIスコープ。
    /// </summary>
    private IServiceScope? scope;

    /// <summary>
    /// テスト対象Repository。
    /// </summary>
    private IOrderRepository repository =
        null!;

    /// <summary>
    /// テストデータ操作用DbContext。
    /// </summary>
    private AppDbContext dbContext =
        null!;

    /// <summary>
    /// 本物の注文変換Adapter。
    /// </summary>
    private OrdersFactory factory =
        null!;
    private OrdersEntityAdapter adapter = null!;

    /// <summary>
    /// テストクラス全体の初期化。
    /// </summary>
    [ClassInitialize]
    public static void ClassInitialize(
        TestContext testContext)
    {
        var presentationProjectPath =
            Path.GetFullPath(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "../../../../../Apps/"
                    + "A_exercise_EC_BE.Presentations"));

        var config =
            new ConfigurationBuilder()
                .SetBasePath(
                    presentationProjectPath)
                .AddJsonFile(
                    "appsettings.json",
                    optional: false)
                .AddEnvironmentVariables()
                .Build();

        provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    config);
    }

    /// <summary>
    /// テストクラス全体の終了処理。
    /// </summary>
    [ClassCleanup]
    public static void ClassCleanup()
    {
        provider?.Dispose();
        provider = null;
    }

    /// <summary>
    /// 各テスト実行前の初期化。
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        if (provider is null)
        {
            throw new InvalidOperationException(
                "テスト用ServiceProviderが"
                + "初期化されていません。");
        }

        scope =
            provider.CreateScope();

        dbContext =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        factory =
            scope.ServiceProvider
                .GetRequiredService<
                    OrdersFactory>();

        adapter =
            scope.ServiceProvider
                .GetRequiredService<
                    OrdersEntityAdapter>();

        repository =
            new OrderRepository(
                dbContext,
                factory,
                adapter);
    }

    /// <summary>
    /// 各テスト実行後の終了処理。
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        scope?.Dispose();
        scope = null;
    }

    /*
     * CreateAsync
     */

    /// <summary>
    /// 有効な注文を正常に登録できることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "有効な注文を指定した場合は正常に登録できる")]
    public async Task
        CreateAsync_WhenOrderIsValid_ShouldCreateOrder()
    {
        // Arrange
        var order =
            await CreateOrderAsync();

        var orderUuid =
            order.OrderUuid;

        try
        {
            // Act
            await repository.CreateAsync(
                order);

            // Assert
            var saved =
                await dbContext.Orders
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        entity =>
                            entity.OrderUuid
                            == orderUuid);

            Assert.IsNotNull(
                saved);

            Assert.AreEqual(
                orderUuid,
                saved.OrderUuid);

            var difference = (order.OrderDate - saved.OrderDate).Duration();

            Assert.IsTrue(difference < TimeSpan.FromMilliseconds(1),
                            $"注文日時の差が大きすぎます。"
                            + $" Expected:{order.OrderDate:O}"
                            + $" Actual:{saved.OrderDate:O}");

            Assert.AreEqual(
                order.AmountTotal,
                saved.AmountTotal);
        }
        finally
        {
            await DeleteOrderAsync(
                orderUuid);
        }
    }

    /// <summary>
    /// 注文がnullの場合にInternalExceptionが発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "注文がnullの場合はInternalExceptionが発生する")]
    public async Task
        CreateAsync_WhenOrderIsNull_ShouldThrowInternalException()
    {
        // Arrange
        Orders order = null!;

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await repository
                            .CreateAsync(
                                order);
                    });

        // Assert
        Assert.AreEqual(
            "永続化する注文がnullです。",
            exception.Message);
    }

    /// <summary>
    /// 注文登録時のDB接続エラーを確認する。
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

        await using var errorContext =
            new AppDbContext(
                options);

        var errorRepository =
            new OrderRepository(
                errorContext,
                factory,
                adapter);

        var order =
            await CreateOrderAsync();

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .CreateAsync(
                                order);
                    });

        // Assert
        Assert.AreEqual(
            "注文の永続化中に予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);
    }

    /*
     * FindByCustomerUuidAsync
     */

    /// <summary>
    /// 顧客UUIDに紐づく購入履歴を取得できることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "購入履歴が存在する顧客UUIDを指定した場合は購入履歴一覧を取得できる")]
    public async Task
        FindByCustomerUuidAsync_WhenOrdersExist_ShouldReturnOrders()
    {
        // Arrange
        var customer =
            await GetCustomerWithOrdersAsync();

        var expectedEntities =
            await dbContext.Orders
                .AsNoTracking()
                .Where(
                    order =>
                        order.CustomerId
                        == customer.Id)
                .OrderByDescending(
                    order =>
                        order.OrderDate)
                .ToListAsync();

        Assert.IsNotEmpty(
            expectedEntities,
            "購入履歴が存在する顧客データが必要です。");

        // Act
        var result =
            await repository
                .FindByCustomerUuidAsync(
                    customer.CustomerUuid);

        // Assert
        Assert.IsNotNull(
            result);

        Assert.AreEqual(
            expectedEntities.Count,
            result.Count);

        CollectionAssert.AreEqual(
            expectedEntities
                .Select(
                    entity =>
                        entity.OrderUuid)
                .ToList(),
            result
                .Select(
                    order =>
                        order.OrderUuid)
                .ToList());

        /*
         * OrderDate降順であることを確認する。
         */
        var expectedDates =
            result
                .Select(
                    order =>
                        order.OrderDate)
                .OrderByDescending(
                    date =>
                        date)
                .ToList();

        CollectionAssert.AreEqual(
            expectedDates,
            result
                .Select(
                    order =>
                        order.OrderDate)
                .ToList());
    }

    /// <summary>
    /// 購入履歴が存在しない場合に空リストが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "購入履歴が存在しない顧客UUIDを指定した場合は空リストを返す")]
    public async Task
        FindByCustomerUuidAsync_WhenOrdersDoNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var nonexistentCustomerUuid =
            Guid.NewGuid();

        // Act
        var result =
            await repository
                .FindByCustomerUuidAsync(
                    nonexistentCustomerUuid);

        // Assert
        Assert.IsNotNull(
            result);

        Assert.IsEmpty(
            result);
    }

    /// <summary>
    /// 購入履歴一覧取得時のDB接続エラーを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindByCustomerUuidAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        FindByCustomerUuidAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
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

        await using var errorContext =
            new AppDbContext(
                options);

        var errorRepository =
            new OrderRepository(
                errorContext,
                factory,
                adapter);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .FindByCustomerUuidAsync(
                                Guid.NewGuid());
                    });

        // Assert
        Assert.AreEqual(
            "購入履歴一覧の取得中に予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);
    }

    /*
     * FindByOrderUuidAsync
     */

    /// <summary>
    /// 注文UUIDで購入履歴詳細を取得できることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在する注文UUIDを指定した場合は購入履歴詳細を取得できる")]
    public async Task
        FindByOrderUuidAsync_WhenOrderExists_ShouldReturnOrder()
    {
        // Arrange
        var orderEntity =
            await dbContext.Orders
                .AsNoTracking()
                .Include(
                    order =>
                        order.OrderDetails)
                .ThenInclude(
                    detail =>
                        detail.Product)
                .Where(
                    order =>
                        order.OrderDetails.Count > 0)
                .OrderBy(
                    order =>
                        order.Id)
                .FirstAsync();

        // Act
        var result =
            await repository
                .FindByOrderUuidAsync(
                    orderEntity.OrderUuid);

        // Assert
        Assert.IsNotNull(
            result);

        Assert.AreEqual(
            orderEntity.OrderUuid,
            result.OrderUuid);

        Assert.AreEqual(
            orderEntity.OrderDate,
            result.OrderDate);

        Assert.AreEqual(
            orderEntity.AmountTotal,
            result.AmountTotal);
    }

    /// <summary>
    /// 注文が存在しない場合にnullが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しない注文UUIDを指定した場合はnullを返す")]
    public async Task
        FindByOrderUuidAsync_WhenOrderDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonexistentOrderUuid =
            Guid.NewGuid();

        // Act
        var result =
            await repository
                .FindByOrderUuidAsync(
                    nonexistentOrderUuid);

        // Assert
        Assert.IsNull(
            result);
    }

    /// <summary>
    /// 購入履歴詳細取得時のDB接続エラーを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindByOrderUuidAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        FindByOrderUuidAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
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

        await using var errorContext =
            new AppDbContext(
                options);

        var errorRepository =
            new OrderRepository(
                errorContext,
                factory,
                adapter);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .FindByOrderUuidAsync(
                                Guid.NewGuid());
                    });

        // Assert
        Assert.AreEqual(
            "購入履歴詳細の取得中に予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);
    }

    /*
     * 共通処理
     */

    /// <summary>
    /// 注文履歴が存在する顧客を1件取得する。
    /// </summary>
    private async Task<CustomerEntity>
        GetCustomerWithOrdersAsync()
    {
        return await dbContext.Customers
            .AsNoTracking()
            .Where(
                customer =>
                    dbContext.Orders.Any(
                        order =>
                            order.CustomerId
                            == customer.Id))
            .OrderBy(
                customer =>
                    customer.Id)
            .FirstAsync();
    }

    /// <summary>
    /// 有効な注文ドメインモデルを生成する。
    /// </summary>
    private async Task<Orders> CreateOrderAsync()
    {
        if (scope is null)
        {
            throw new InvalidOperationException(
                "テスト用ServiceScopeが初期化されていません。");
        }

        /*
         * 注文に関連付ける既存データを取得する。
         */
        var customerEntity =
            await dbContext.Customers
                .AsNoTracking()
                .OrderBy(customer =>
                    customer.Id)
                .FirstAsync();

        var orderStatusEntity =
            await dbContext.OrderStatuses
                .AsNoTracking()
                .OrderBy(status =>
                    status.Id)
                .FirstAsync();

        var paymentMethodEntity =
            await dbContext.PaymentMethods
                .AsNoTracking()
                .OrderBy(method =>
                    method.Id)
                .FirstAsync();

        /*
         * 本物のAdapterをDIから取得する。
         */
        var customerAdapter =
            scope.ServiceProvider
                .GetRequiredService<
                    CustomerEntityAdapter>();

        var orderStatusAdapter =
            scope.ServiceProvider
                .GetRequiredService<
                    OrderStatusEntityAdapter>();

        var paymentMethodAdapter =
            scope.ServiceProvider
                .GetRequiredService<
                    PaymentMethodEntityAdapter>();

        /*
         * Entityからドメインオブジェクトへ復元する。
         */
        var customer =
            await customerAdapter.RestoreAsync(
                customerEntity);

        var orderStatus =
            await orderStatusAdapter.RestoreAsync(
                orderStatusEntity);

        var paymentMethod =
            await paymentMethodAdapter.RestoreAsync(
                paymentMethodEntity);

        /*
         * 注文明細は空でもよいが、nullは許可されていないため
         * 空のListを渡す。
         */
        var orderDetails =
            new List<OrdersDetail>();

        /*
         * テスト実行時刻より確実に過去になるよう、
         * 1分前の日時を指定する。
         */
        return new Orders(
            Guid.NewGuid(),
            DateTime.Now.AddMinutes(-1),
            1_000,
            customer,
            orderStatus,
            paymentMethod,
            orderDetails);
    }

    /// <summary>
    /// テストで登録した注文を物理削除する。
    /// </summary>
    private async Task DeleteOrderAsync(
        Guid orderUuid)
    {
        dbContext.ChangeTracker.Clear();

        var entity =
            await dbContext.Orders
                .Include(
                    order =>
                        order.OrderDetails)
                .SingleOrDefaultAsync(
                    order =>
                        order.OrderUuid
                        == orderUuid);

        if (entity is null)
        {
            return;
        }

        if (entity.OrderDetails.Count > 0)
        {
            dbContext.OrdersDetails.RemoveRange(
                entity.OrderDetails);
        }

        dbContext.Orders.Remove(
            entity);

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// DBに存在しない顧客UUIDを指定した場合の例外を確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "DBに存在しない顧客UUIDを指定した場合はInternalExceptionが発生する")]
    public async Task
        CreateAsync_WhenCustomerDoesNotExist_ShouldThrowInternalException()
    {
        // Arrange
        var nonexistentCustomerUuid =
            Guid.NewGuid();

        /*
         * DBには登録しない顧客ドメインを生成する。
         * Customerのバリデーションを通すため、
         * UUID以外の必須項目も正しい値を指定する。
         */
        var nonexistentCustomer =
            new Customer(
                nonexistentCustomerUuid,
                "存在しない顧客",
                "ソンザイシナイコキャク",
                "東京都新宿区1-1-1",
                null,
                "09012345678",
                $"notfound-{Guid.NewGuid():N}@example.com",
                $"user{Guid.NewGuid():N}"[..20],
                "test-password",
                DateTime.Now.AddMinutes(-1));

        /*
         * 注文ステータスと支払い方法は
         * DBに存在するものを利用する。
         */
        var orderStatusEntity =
            await dbContext.OrderStatuses
                .AsNoTracking()
                .OrderBy(
                    status =>
                        status.Id)
                .FirstAsync();

        var paymentMethodEntity =
            await dbContext.PaymentMethods
                .AsNoTracking()
                .OrderBy(
                    method =>
                        method.Id)
                .FirstAsync();

        var orderStatusAdapter =
            scope!.ServiceProvider
                .GetRequiredService<
                    OrderStatusEntityAdapter>();

        var paymentMethodAdapter =
            scope.ServiceProvider
                .GetRequiredService<
                    PaymentMethodEntityAdapter>();

        var orderStatus =
            await orderStatusAdapter.RestoreAsync(
                orderStatusEntity);

        var paymentMethod =
            await paymentMethodAdapter.RestoreAsync(
                paymentMethodEntity);

        var order =
            new Orders(
                Guid.NewGuid(),
                DateTime.Now.AddMinutes(-1),
                1_000,
                nonexistentCustomer,
                orderStatus,
                paymentMethod,
                new List<OrdersDetail>());

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await repository.CreateAsync(
                            order);
                    });

        // Assert
        Assert.AreEqual(
            $"顧客UUID:{nonexistentCustomerUuid}"
            + "の顧客が存在しません。",
            exception.Message);
    }
}