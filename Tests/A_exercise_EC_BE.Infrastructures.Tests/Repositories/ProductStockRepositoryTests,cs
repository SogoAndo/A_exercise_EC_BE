using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Entities;
using A_exercise_EC_BE.Infrastructures.Repositories;
using A_exercise_EC_BE.Presentations.Configs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace A_exercise_EC_BE.Infrastructures.Tests.Repositories;

/// <summary>
/// 商品在庫Repositoryのテスト。
/// </summary>
[TestClass]
[DoNotParallelize]
[TestCategory("Infrastructure/Repositories")]
public class ProductStockRepositoryTests
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
    private IProductStockRepository repository =
        null!;

    /// <summary>
    /// テストデータ操作用DbContext。
    /// </summary>
    private AppDbContext dbContext =
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

        repository =
            new ProductStockRepository(
                dbContext);
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
    /// 在庫が十分にある場合に、指定数量を減算できることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "在庫が十分にある場合は指定数量を減算してtrueを返す")]
    public async Task
        TryDecreaseAsync_WhenStockIsSufficient_ShouldDecreaseStockAndReturnTrue()
    {
        // Arrange
        var stockEntity =
            await GetStockWithPositiveQuantityAsync();

        var productUuid =
            stockEntity.Product.ProductUuid;

        var originalQuantity =
            stockEntity.Quantity;

        const int decreaseQuantity =
            1;

        try
        {
            // Act
            var result =
                await repository.TryDecreaseAsync(
                    productUuid,
                    decreaseQuantity);

            // Assert
            Assert.IsTrue(
                result);

            /*
             * ExecuteUpdateAsyncはChangeTrackerを経由しないため、
             * DBから最新値を再取得する。
             */
            dbContext.ChangeTracker.Clear();

            var updatedQuantity =
                await dbContext.ProductStocks
                    .AsNoTracking()
                    .Where(
                        stock =>
                            stock.Product.ProductUuid
                            == productUuid)
                    .Select(
                        stock =>
                            stock.Quantity)
                    .SingleAsync();

            Assert.AreEqual(
                originalQuantity
                - decreaseQuantity,
                updatedQuantity);
        }
        finally
        {
            /*
             * テスト前の在庫数へ戻す。
             */
            await RestoreStockQuantityAsync(
                productUuid,
                originalQuantity);
        }
    }

    /// <summary>
    /// 在庫数と同じ数量を減算した場合、在庫が0になることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "在庫数と同じ数量を指定した場合は在庫を0にしてtrueを返す")]
    public async Task
        TryDecreaseAsync_WhenQuantityEqualsStock_ShouldSetStockToZeroAndReturnTrue()
    {
        // Arrange
        var stockEntity =
            await GetStockWithPositiveQuantityAsync();

        var productUuid =
            stockEntity.Product.ProductUuid;

        var originalQuantity =
            stockEntity.Quantity;

        try
        {
            // Act
            var result =
                await repository.TryDecreaseAsync(
                    productUuid,
                    originalQuantity);

            // Assert
            Assert.IsTrue(
                result);

            dbContext.ChangeTracker.Clear();

            var updatedQuantity =
                await dbContext.ProductStocks
                    .AsNoTracking()
                    .Where(
                        stock =>
                            stock.Product.ProductUuid
                            == productUuid)
                    .Select(
                        stock =>
                            stock.Quantity)
                    .SingleAsync();

            Assert.AreEqual(
                0,
                updatedQuantity);
        }
        finally
        {
            await RestoreStockQuantityAsync(
                productUuid,
                originalQuantity);
        }
    }

    /*
     * 更新対象なし
     */

    /// <summary>
    /// 在庫が不足している場合に、更新せずfalseを返すことを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "在庫が不足している場合は在庫を変更せずfalseを返す")]
    public async Task
        TryDecreaseAsync_WhenStockIsInsufficient_ShouldNotChangeStockAndReturnFalse()
    {
        // Arrange
        var stockEntity =
            await GetExistingStockAsync();

        var productUuid =
            stockEntity.Product.ProductUuid;

        var originalQuantity =
            stockEntity.Quantity;

        var decreaseQuantity =
            originalQuantity + 1;

        // Act
        var result =
            await repository.TryDecreaseAsync(
                productUuid,
                decreaseQuantity);

        // Assert
        Assert.IsFalse(
            result);

        dbContext.ChangeTracker.Clear();

        var actualQuantity =
            await dbContext.ProductStocks
                .AsNoTracking()
                .Where(
                    stock =>
                        stock.Product.ProductUuid
                        == productUuid)
                .Select(
                    stock =>
                        stock.Quantity)
                .SingleAsync();

        Assert.AreEqual(
            originalQuantity,
            actualQuantity);
    }

    /// <summary>
    /// 存在しない商品UUIDを指定した場合にfalseを返すことを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しない商品UUIDを指定した場合はfalseを返す")]
    public async Task
        TryDecreaseAsync_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonexistentProductUuid =
            Guid.NewGuid();

        // Act
        var result =
            await repository.TryDecreaseAsync(
                nonexistentProductUuid,
                1);

        // Assert
        Assert.IsFalse(
            result);
    }

    /*
     * 入力値異常
     */

    /// <summary>
    /// 減算数量が0の場合にInternalExceptionが発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "減算数量が0の場合はInternalExceptionが発生する")]
    public async Task
        TryDecreaseAsync_WhenQuantityIsZero_ShouldThrowInternalException()
    {
        // Arrange
        var productUuid =
            Guid.NewGuid();

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await repository
                            .TryDecreaseAsync(
                                productUuid,
                                0);
                    });

        // Assert
        Assert.AreEqual(
            "在庫から減算する数量は1以上である必要があります。",
            exception.Message);
    }

    /// <summary>
    /// 減算数量が負数の場合にInternalExceptionが発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "減算数量が負数の場合はInternalExceptionが発生する")]
    public async Task
        TryDecreaseAsync_WhenQuantityIsNegative_ShouldThrowInternalException()
    {
        // Arrange
        var productUuid =
            Guid.NewGuid();

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await repository
                            .TryDecreaseAsync(
                                productUuid,
                                -1);
                    });

        // Assert
        Assert.AreEqual(
            "在庫から減算する数量は1以上である必要があります。",
            exception.Message);
    }

    /*
     * DB異常
     */

    /// <summary>
    /// DB接続エラー時にInternalExceptionが発生することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "TryDecreaseAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
        TryDecreaseAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
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
            new ProductStockRepository(
                errorContext);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .TryDecreaseAsync(
                                Guid.NewGuid(),
                                1);
                    });

        // Assert
        Assert.AreEqual(
            "商品在庫の更新中に予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);
    }

    /*
     * 共通処理
     */

    /// <summary>
    /// DBに存在する商品在庫を1件取得する。
    /// </summary>
    private async Task<ProductStockEntity>
        GetExistingStockAsync()
    {
        return await dbContext.ProductStocks
            .AsNoTracking()
            .Include(
                stock =>
                    stock.Product)
            .OrderBy(
                stock =>
                    stock.Id)
            .FirstAsync();
    }

    /// <summary>
    /// 在庫数が1以上の商品在庫を1件取得する。
    /// </summary>
    private async Task<ProductStockEntity>
        GetStockWithPositiveQuantityAsync()
    {
        return await dbContext.ProductStocks
            .AsNoTracking()
            .Include(
                stock =>
                    stock.Product)
            .Where(
                stock =>
                    stock.Quantity > 0)
            .OrderBy(
                stock =>
                    stock.Id)
            .FirstAsync();
    }

    /// <summary>
    /// 商品UUIDを指定して在庫数を元に戻す。
    /// </summary>
    private async Task RestoreStockQuantityAsync(
        Guid productUuid,
        int originalQuantity)
    {
        dbContext.ChangeTracker.Clear();

        await dbContext.ProductStocks
            .Where(
                stock =>
                    stock.Product.ProductUuid
                    == productUuid)
            .ExecuteUpdateAsync(
                setters =>
                    setters.SetProperty(
                        stock =>
                            stock.Quantity,
                        originalQuantity));
    }
}