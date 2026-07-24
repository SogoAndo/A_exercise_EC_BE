using A_exercise_EC_BE.Domains.Adapters;
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
/// 注文明細Repositoryのテスト。
/// </summary>
[TestClass]
[DoNotParallelize]
[TestCategory("Infrastructure/Repositories")]
public class OrderDetailRepositoryTests
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
    private IOrderDetailRepository repository =
        null!;

    /// <summary>
    /// テストデータ操作用DbContext。
    /// </summary>
    private AppDbContext dbContext =
        null!;

    /// <summary>
    /// 本物の注文明細Adapter。
    /// </summary>
    private IConverter<
        OrdersDetail,
        OrdersDetailEntity
    > adapter = null!;

    /// <summary>
    /// 商品Entityをドメインモデルへ復元するFactory。
    /// </summary>
    private ProductFactory productFactory =
        null!;

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
                .BuildAppProvider(config);
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

        adapter =
            scope.ServiceProvider
                .GetRequiredService<
                    IConverter<
                        OrdersDetail,
                        OrdersDetailEntity
                    >
                >();

        productFactory =
            scope.ServiceProvider
                .GetRequiredService<ProductFactory>();

        repository =
            new OrderDetailRepository(
                dbContext,
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
     * 正常系
     */

    /// <summary>
    /// 有効な注文明細を複数件登録できることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "有効な注文明細を複数件指定した場合は正常に登録できる")]
    public async Task
        CreateRangeAsync_WhenOrderDetailsAreValid_ShouldCreateAllOrderDetails()
    {
        // Arrange
        var order =
            await GetExistingOrderAsync();

        var product =
            await GetExistingProductAsync();

        var firstCount =
            CreateUniqueCount();

        var secondCount =
            firstCount + 1;

        var firstOrderDetail =
            new OrdersDetail(
                product,
                firstCount);

        var secondOrderDetail =
            new OrdersDetail(
                product,
                secondCount);

        IReadOnlyCollection<OrdersDetail>
            orderDetails =
                new List<OrdersDetail>
                {
                    firstOrderDetail,
                    secondOrderDetail
                };

        /*
         * 実行前に存在している明細IDを保存する。
         * 実行後との差分をテスト登録分として扱う。
         */
        var beforeIds =
            await dbContext.OrdersDetails
                .AsNoTracking()
                .Select(
                    detail =>
                        detail.Id)
                .ToListAsync();

        var createdIds =
            new List<int>();

        try
        {
            // Act
            await repository.CreateRangeAsync(
                order.Id,
                orderDetails);

            // Assert
            var createdEntities =
                await dbContext.OrdersDetails
                    .AsNoTracking()
                    .Where(
                        detail =>
                            !beforeIds.Contains(
                                detail.Id))
                    .OrderBy(
                        detail =>
                            detail.Id)
                    .ToListAsync();

            createdIds =
                createdEntities
                    .Select(
                        detail =>
                            detail.Id)
                    .ToList();

            Assert.HasCount(
                2,
                createdEntities);

            Assert.IsTrue(
                createdEntities.All(
                    detail =>
                        detail.OrderId
                        == order.Id));

            Assert.IsTrue(
                createdEntities.All(
                    detail =>
                        detail.ProductId
                        == product.ProductUuid
                            .Equals(Guid.Empty)
                            ? false
                            : true));

            CollectionAssert.AreEquivalent(
                new[]
                {
                    firstCount,
                    secondCount
                },
                createdEntities
                    .Select(
                        detail =>
                            detail.Count)
                    .ToArray());
        }
        finally
        {
            await DeleteOrderDetailsAsync(
                createdIds);
        }
    }

    /// <summary>
    /// 有効な注文明細を1件登録できることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "有効な注文明細を1件指定した場合は正常に登録できる")]
    public async Task
        CreateRangeAsync_WhenOneOrderDetailIsValid_ShouldCreateOrderDetail()
    {
        // Arrange
        var order =
            await GetExistingOrderAsync();

        var productEntity =
            await GetExistingProductEntityAsync();

        var product =
            await productFactory.RestoreAsync(
                productEntity);

        var count =
            CreateUniqueCount();

        var orderDetail =
            new OrdersDetail(
                product,
                count);

        var beforeIds =
            await dbContext.OrdersDetails
                .AsNoTracking()
                .Select(
                    detail =>
                        detail.Id)
                .ToListAsync();

        var createdIds =
            new List<int>();

        try
        {
            // Act
            await repository.CreateRangeAsync(
                order.Id,
                new[]
                {
                    orderDetail
                });

            // Assert
            var createdEntity =
                await dbContext.OrdersDetails
                    .AsNoTracking()
                    .SingleAsync(
                        detail =>
                            !beforeIds.Contains(
                                detail.Id));

            createdIds.Add(
                createdEntity.Id);

            Assert.AreEqual(
                order.Id,
                createdEntity.OrderId);

            Assert.AreEqual(
                productEntity.Id,
                createdEntity.ProductId);

            Assert.AreEqual(
                count,
                createdEntity.Count);
        }
        finally
        {
            await DeleteOrderDetailsAsync(
                createdIds);
        }
    }

    /*
     * 入力値異常
     */

    /// <summary>
    /// 注文明細がnullの場合の例外を確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "注文明細がnullの場合はInternalExceptionが発生する")]
    public async Task
        CreateRangeAsync_WhenOrderDetailsAreNull_ShouldThrowInternalException()
    {
        // Arrange
        IReadOnlyCollection<OrdersDetail>
            orderDetails = null!;

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await repository
                            .CreateRangeAsync(
                                1,
                                orderDetails);
                    });

        // Assert
        Assert.AreEqual(
            "永続化する注文明細がnullです。",
            exception.Message);
    }

    /// <summary>
    /// 注文明細が空の場合の例外を確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "注文明細が0件の場合はInternalExceptionが発生する")]
    public async Task
        CreateRangeAsync_WhenOrderDetailsAreEmpty_ShouldThrowInternalException()
    {
        // Arrange
        IReadOnlyCollection<OrdersDetail>
            orderDetails =
                Array.Empty<OrdersDetail>();

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await repository
                            .CreateRangeAsync(
                                1,
                                orderDetails);
                    });

        // Assert
        Assert.AreEqual(
            "永続化する注文明細が存在しません。",
            exception.Message);
    }

    /// <summary>
    /// DBに存在しない商品の場合の例外を確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "DBに存在しない商品を指定した場合はInternalExceptionが発生する")]
    public async Task
        CreateRangeAsync_WhenProductDoesNotExist_ShouldThrowInternalException()
    {
        // Arrange
        var order =
            await GetExistingOrderAsync();

        var nonexistentProductUuid =
            Guid.NewGuid();

        var nonexistentProduct =
            new Product(
                nonexistentProductUuid,
                "存在しない商品",
                100,
                "https://example.com/not-found.png",
                null,
                null,
                0);

        var orderDetail =
            new OrdersDetail(
                nonexistentProduct,
                1);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await repository
                            .CreateRangeAsync(
                                order.Id,
                                new[]
                                {
                                    orderDetail
                                });
                    });

        // Assert
        Assert.AreEqual(
            $"商品UUID:{nonexistentProductUuid}"
            + "の商品が存在しません。",
            exception.Message);
    }

    /*
     * DB異常
     */

    /// <summary>
    /// 存在しない注文IDによるDBエラーを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しない注文IDを指定した場合はInternalExceptionが発生する")]
    public async Task
        CreateRangeAsync_WhenOrderDoesNotExist_ShouldThrowInternalException()
    {
        // Arrange
        var product =
            await GetExistingProductAsync();

        var orderDetail =
            new OrdersDetail(
                product,
                1);

        /*
         * DBに存在しない可能性が高い負数を指定する。
         */
        const int nonexistentOrderId =
            -1;

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await repository
                            .CreateRangeAsync(
                                nonexistentOrderId,
                                new[]
                                {
                                    orderDetail
                                });
                    });

        // Assert
        Assert.AreEqual(
            "注文明細の永続化中に予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);

        /*
         * SaveChanges失敗後に追跡状態が残るため、
         * ChangeTrackerを初期化する。
         */
        dbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// DB接続エラー時の例外を確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "CreateRangeAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        CreateRangeAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
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
            new OrderDetailRepository(
                errorContext,
                adapter);

        /*
         * 商品検索の時点でDB接続エラーになるため、
         * DBに存在しないProductでも構わない。
         */
        var product =
            new Product(
                Guid.NewGuid(),
                "DB接続エラー確認商品",
                100,
                "https://example.com/error.png",
                null,
                null,
                0);

        var orderDetail =
            new OrdersDetail(
                product,
                1);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .CreateRangeAsync(
                                1,
                                new[]
                                {
                                    orderDetail
                                });
                    });

        // Assert
        Assert.AreEqual(
            "注文明細の永続化中に予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);
    }

    /*
     * 共通処理
     */

    /// <summary>
    /// DBに存在する注文を1件取得する。
    /// </summary>
    private async Task<OrdersEntity>
        GetExistingOrderAsync()
    {
        return await dbContext.Orders
            .AsNoTracking()
            .OrderBy(
                order =>
                    order.Id)
            .FirstAsync();
    }

    /// <summary>
    /// DBに存在する未削除商品Entityを1件取得する。
    /// </summary>
    private async Task<ProductEntity>
        GetExistingProductEntityAsync()
    {
        return await dbContext.Products
            .AsNoTracking()
            .Include(
                product =>
                    product.ProductCategory)
            .Include(
                product =>
                    product.ProductStock)
            .Where(
                product =>
                    product.DeleteFlg == 0)
            .OrderBy(
                product =>
                    product.Id)
            .FirstAsync();
    }

    /// <summary>
    /// DBに存在する商品をドメインモデルとして取得する。
    /// </summary>
    private async Task<Product>
        GetExistingProductAsync()
    {
        var entity =
            await GetExistingProductEntityAsync();

        return await productFactory
            .RestoreAsync(
                entity);
    }

    /// <summary>
    /// 他のテストデータと重複しにくい注文数を生成する。
    /// </summary>
    private static int CreateUniqueCount()
    {
        /*
         * OrdersDetail側は0以上のみを制約としているため、
         * テスト識別用の値として時刻を利用する。
         */
        return Math.Abs(
            Environment.TickCount
            % 10_000) + 1;
    }

    /// <summary>
    /// テストで追加した注文明細を物理削除する。
    /// </summary>
    private async Task DeleteOrderDetailsAsync(
        IEnumerable<int> detailIds)
    {
        var ids =
            detailIds
                .Distinct()
                .ToArray();

        if (ids.Length == 0)
        {
            return;
        }

        dbContext.ChangeTracker.Clear();

        var deleteTargets =
            await dbContext.OrdersDetails
                .Where(
                    detail =>
                        ids.Contains(
                            detail.Id))
                .ToListAsync();

        if (deleteTargets.Count == 0)
        {
            return;
        }

        dbContext.OrdersDetails.RemoveRange(
            deleteTargets);

        await dbContext.SaveChangesAsync();
    }
}