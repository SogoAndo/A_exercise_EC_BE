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
/// 商品Repositoryのテスト。
/// </summary>
[TestClass]
[DoNotParallelize]
[TestCategory("Infrastructure/Repositories")]
public class ProductRepositoryTests
{
    /// <summary>
    /// テスト対象Repository取得用のサービススコープ。
    /// </summary>
    private IServiceScope scope = null!;

    /// <summary>
    /// テスト対象Repository。
    /// </summary>
    private IProductRepository repository = null!;

    /// <summary>
    /// テストデータ確認・登録用DbContext。
    /// </summary>
    private AppDbContext dbContext = null!;

    /// <summary>
    /// DI設定済みServiceProvider。
    ///
    /// プロジェクトですでに共通ServiceProviderを構築している場合は、
    /// そのフィールドを使用してください。
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
    /// 各テスト実行前の初期化。
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        scope =
            provider.CreateScope();

        repository =
            scope.ServiceProvider
                .GetRequiredService<IProductRepository>();

        dbContext =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();
    }

    /// <summary>
    /// 各テスト実行後の終了処理。
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        scope.Dispose();
    }

    /*
     * FindAllAsync
     */

    /// <summary>
    /// 未削除の商品一覧を正常に取得できることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "未削除の商品一覧を正常に取得できる")]
    public async Task
        FindAllAsync_WhenActiveProductsExist_ShouldReturnProducts()
    {
        // Act
        var result =
            await repository.FindAllAsync();

        // Assert
        Assert.IsNotNull(
            result);

        Assert.IsNotEmpty(
            result);

        Assert.IsTrue(
            result.All(
                product =>
                    product.DeleteFlg == 0));

        Assert.IsTrue(
            result.All(
                product =>
                    product.ProductCategory
                    is not null));

        Assert.IsTrue(
            result.All(
                product =>
                    product.ProductStock
                    is not null));
    }

    /// <summary>
    /// 商品がIDの昇順で取得されることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "商品一覧がIDの昇順で取得される")]
    public async Task
        FindAllAsync_WhenProductsExist_ShouldReturnProductsInIdOrder()
    {
        // Arrange
        var expectedEntities =
            await dbContext.Products
                .AsNoTracking()
                .Where(
                    product =>
                        product.DeleteFlg == 0)
                .OrderBy(
                    product =>
                        product.Id)
                .ToListAsync();

        // Act
        var result =
            await repository.FindAllAsync();

        // Assert
        Assert.AreEqual(
            expectedEntities.Count,
            result.Count);

        var expectedUuids =
            expectedEntities
                .Select(
                    product =>
                        product.ProductUuid)
                .ToList();

        var actualUuids =
            result
                .Select(
                    product =>
                        product.ProductUuid)
                .ToList();

        CollectionAssert.AreEqual(
            expectedUuids,
            actualUuids);
    }

    /// <summary>
    /// 削除済み商品が一覧に含まれないことを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "削除済み商品は商品一覧に含まれない")]
    public async Task
        FindAllAsync_WhenDeletedProductsExist_ShouldExcludeDeletedProducts()
    {
        // Arrange
        var category =
            await GetExistingCategoryAsync();

        var productUuid =
            Guid.NewGuid();

        var product =
            CreateProductEntity(
                productUuid,
                "削除済み一覧確認商品",
                category,
                deleteFlg: 1);

        dbContext.Products.Add(
            product);

        await dbContext.SaveChangesAsync();

        try
        {
            // Act
            var result =
                await repository.FindAllAsync();

            // Assert
            Assert.IsFalse(
                result.Any(
                    item =>
                        item.ProductUuid
                        == productUuid));
        }
        finally
        {
            await DeleteProductAsync(
                productUuid);
        }
    }

    /// <summary>
    /// DB接続エラーをInternalExceptionへ変換することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "商品一覧取得時にDB接続エラーが発生した場合はInternalExceptionを送出する")]
    public async Task
        FindAllAsync_WhenDatabaseConnectionFails_ShouldThrowInternalException()
    {
        // Arrange
        await using var errorContext =
            CreateConnectionErrorContext();

        var factory =
            scope.ServiceProvider
                .GetRequiredService<ProductFactory>();

        var errorRepository =
            new ProductRepository(
                errorContext,
                factory);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .FindAllAsync();
                    });

        // Assert
        Assert.AreEqual(
            "商品一覧の取得中に予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);
    }

    /*
     * SelectByProductCategoryIdAsync
     */

    /// <summary>
    /// 指定したカテゴリの商品を取得できることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "指定したカテゴリの未削除商品一覧を取得できる")]
    public async Task
        SelectByProductCategoryIdAsync_WhenProductsExist_ShouldReturnProducts()
    {
        // Arrange
        var category =
            await GetExistingCategoryAsync();

        // Act
        var result =
            await repository
                .SelectByProductCategoryIdAsync(
                    category.CategoryUuid);

        // Assert
        Assert.IsNotNull(
            result);

        Assert.IsTrue(
            result.All(
                product =>
                    product.DeleteFlg == 0));

        Assert.IsTrue(
            result.All(
                product =>
                    product.ProductCategory
                    is not null));

        Assert.IsTrue(
            result.All(
                product =>
                    product.ProductCategory!
                        .CategoryUuid
                    == category.CategoryUuid));
    }

    /// <summary>
    /// 指定したカテゴリの商品だけが取得されることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "指定カテゴリ以外の商品は取得されない")]
    public async Task
        SelectByProductCategoryIdAsync_WhenOtherCategoryProductsExist_ShouldExcludeThem()
    {
        // Arrange
        var categories =
            await dbContext.ProductCategories
                .AsNoTracking()
                .OrderBy(
                    category =>
                        category.Id)
                .Take(2)
                .ToListAsync();

        Assert.HasCount(
            2,
            categories,
            "テストには2件以上の商品カテゴリが必要です。");

        var targetCategory =
            categories[0];

        var otherCategory =
            categories[1];

        // Act
        var result =
            await repository
                .SelectByProductCategoryIdAsync(
                    targetCategory.CategoryUuid);

        // Assert
        Assert.IsFalse(
            result.Any(
                product =>
                    product.ProductCategory
                        ?.CategoryUuid
                    == otherCategory.CategoryUuid));

        Assert.IsTrue(
            result.All(
                product =>
                    product.ProductCategory
                        ?.CategoryUuid
                    == targetCategory.CategoryUuid));
    }

    /// <summary>
    /// 指定カテゴリの削除済み商品が除外されることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "指定カテゴリの削除済み商品は取得されない")]
    public async Task
        SelectByProductCategoryIdAsync_WhenDeletedProductExists_ShouldExcludeDeletedProduct()
    {
        // Arrange
        var category =
            await GetExistingCategoryAsync();

        var productUuid =
            Guid.NewGuid();

        var product =
            CreateProductEntity(
                productUuid,
                "カテゴリ削除済み確認商品",
                category,
                deleteFlg: 1);

        dbContext.Products.Add(
            product);

        await dbContext.SaveChangesAsync();

        try
        {
            // Act
            var result =
                await repository
                    .SelectByProductCategoryIdAsync(
                        category.CategoryUuid);

            // Assert
            Assert.IsFalse(
                result.Any(
                    item =>
                        item.ProductUuid
                        == productUuid));
        }
        finally
        {
            await DeleteProductAsync(
                productUuid);
        }
    }

    /// <summary>
    /// 存在しないカテゴリUUIDの場合に空リストが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しないカテゴリUUIDを指定した場合は空リストを返す")]
    public async Task
        SelectByProductCategoryIdAsync_WhenCategoryDoesNotExist_ShouldReturnEmptyList()
    {
        // Arrange
        var nonexistentCategoryUuid =
            Guid.NewGuid();

        // Act
        var result =
            await repository
                .SelectByProductCategoryIdAsync(
                    nonexistentCategoryUuid);

        // Assert
        Assert.IsNotNull(
            result);

        Assert.IsEmpty(
            result);
    }

    /// <summary>
    /// DB接続エラーをInternalExceptionへ変換することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "カテゴリ別商品取得時にDB接続エラーが発生した場合はInternalExceptionを送出する")]
    public async Task
        SelectByProductCategoryIdAsync_WhenDatabaseConnectionFails_ShouldThrowInternalException()
    {
        // Arrange
        await using var errorContext =
            CreateConnectionErrorContext();

        var factory =
            scope.ServiceProvider
                .GetRequiredService<ProductFactory>();

        var errorRepository =
            new ProductRepository(
                errorContext,
                factory);

        var categoryUuid =
            Guid.NewGuid();

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .SelectByProductCategoryIdAsync(
                                categoryUuid);
                    });

        // Assert
        Assert.AreEqual(
            $"商品カテゴリID:{categoryUuid}の商品取得中に予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);
    }

    /*
     * FindByIdAsync
     */

    /// <summary>
    /// 存在する未削除商品をUUIDで取得できることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在する商品UUIDを指定した場合は商品を取得できる")]
    public async Task
        FindByIdAsync_WhenActiveProductExists_ShouldReturnProduct()
    {
        // Arrange
        var category =
            await GetExistingCategoryAsync();

        var productUuid =
            Guid.NewGuid();

        var product =
            CreateProductEntity(
                productUuid,
                "商品ID検索確認商品",
                category,
                deleteFlg: 0);

        dbContext.Products.Add(
            product);

        await dbContext.SaveChangesAsync();

        try
        {
            // Act
            var result =
                await repository
                    .FindByIdAsync(
                        productUuid);

            // Assert
            Assert.IsNotNull(
                result);

            Assert.AreEqual(
                productUuid,
                result.ProductUuid);

            Assert.AreEqual(
                "商品ID検索確認商品",
                result.Name);

            Assert.AreEqual(
                1_000,
                result.Price);

            Assert.AreEqual(
                0,
                result.DeleteFlg);

            Assert.IsNotNull(
                result.ProductCategory);

            Assert.AreEqual(
                category.CategoryUuid,
                result.ProductCategory
                    .CategoryUuid);

            Assert.IsNotNull(
                result.ProductStock);

            Assert.AreEqual(
                10,
                result.ProductStock
                    .Quantity);
        }
        finally
        {
            await DeleteProductAsync(
                productUuid);
        }
    }

    /// <summary>
    /// 存在しない商品UUIDの場合にnullが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しない商品UUIDを指定した場合はnullを返す")]
    public async Task
        FindByIdAsync_WhenProductDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonexistentProductUuid =
            Guid.NewGuid();

        // Act
        var result =
            await repository
                .FindByIdAsync(
                    nonexistentProductUuid);

        // Assert
        Assert.IsNull(
            result);
    }

    /// <summary>
    /// 削除済み商品をUUIDで検索した場合にnullが返ることを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "削除済みの商品UUIDを指定した場合はnullを返す")]
    public async Task
        FindByIdAsync_WhenProductIsDeleted_ShouldReturnNull()
    {
        // Arrange
        var category =
            await GetExistingCategoryAsync();

        var productUuid =
            Guid.NewGuid();

        var product =
            CreateProductEntity(
                productUuid,
                "削除済みID検索確認商品",
                category,
                deleteFlg: 1);

        dbContext.Products.Add(
            product);

        await dbContext.SaveChangesAsync();

        try
        {
            // Act
            var result =
                await repository
                    .FindByIdAsync(
                        productUuid);

            // Assert
            Assert.IsNull(
                result);
        }
        finally
        {
            await DeleteProductAsync(
                productUuid);
        }
    }

    /// <summary>
    /// DB接続エラーをInternalExceptionへ変換することを確認する。
    /// </summary>
    [TestMethod(
        DisplayName =
            "商品ID検索時にDB接続エラーが発生した場合はInternalExceptionを送出する")]
    public async Task
        FindByIdAsync_WhenDatabaseConnectionFails_ShouldThrowInternalException()
    {
        // Arrange
        await using var errorContext =
            CreateConnectionErrorContext();

        var factory =
            scope.ServiceProvider
                .GetRequiredService<ProductFactory>();

        var errorRepository =
            new ProductRepository(
                errorContext,
                factory);

        var productUuid =
            Guid.NewGuid();

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .FindByIdAsync(
                                productUuid);
                    });

        // Assert
        Assert.AreEqual(
            $"商品ID:{productUuid}の商品取得中に予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);
    }

    /*
     * 共通処理
     */

    /// <summary>
    /// DB接続エラーを発生させるDbContextを生成する。
    /// </summary>
    private static AppDbContext
        CreateConnectionErrorContext()
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(
                    "Host=localhost;"
                    + "Port=9999;"
                    + "Database=A_exercise_EC_BE;"
                    + "Username=postgres;"
                    + "Password=postgres;"
                    + "Timeout=1;"
                    + "Command Timeout=1")
                .Options;

        return new AppDbContext(
            options);
    }

    /// <summary>
    /// テストDBに存在する商品カテゴリを1件取得する。
    /// </summary>
    private async Task<ProductCategoryEntity>
        GetExistingCategoryAsync()
    {
        return await dbContext.ProductCategories
            .AsNoTracking()
            .OrderBy(
                category =>
                    category.Id)
            .FirstAsync();
    }

    /// <summary>
    /// 商品Entityを生成する。
    /// </summary>
    private static ProductEntity
        CreateProductEntity(
            Guid productUuid,
            string productName,
            ProductCategoryEntity category,
            int deleteFlg)
    {
        var product =
            new ProductEntity
            {
                ProductUuid =
                    productUuid,

                Name =
                    productName,

                Price =
                    1_000,

                ImageUrl =
                    "https://example.com/product.png",

                DeleteFlg =
                    deleteFlg,

                ProductCategoryId =
                    category.Id,

            };

        product.ProductStock =
            new ProductStockEntity
            {
                StockUuid =
                    Guid.NewGuid(),

                Quantity =
                    10,

                Product =
                    product
            };

        return product;
    }

    /// <summary>
    /// テストで登録した商品を物理削除する。
    /// </summary>
    private async Task DeleteProductAsync(
        Guid productUuid)
    {
        var product =
            await dbContext.Products
                .Include(
                    entity =>
                        entity.ProductStock)
                .SingleOrDefaultAsync(
                    entity =>
                        entity.ProductUuid
                        == productUuid);

        if (product is null)
        {
            return;
        }

        if (product.ProductStock
            is not null)
        {
            dbContext.ProductStocks.Remove(
                product.ProductStock);
        }

        dbContext.Products.Remove(
            product);

        await dbContext.SaveChangesAsync();
    }

    [TestMethod(
    DisplayName =
        "FindAllAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
    FindAllAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
    {
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

        var factory =
            scope.ServiceProvider
                .GetRequiredService<ProductFactory>();

        var repository =
            new ProductRepository(
                context,
                factory);

        await Assert.ThrowsExactlyAsync<InternalException>(
            async () =>
            {
                await repository.FindAllAsync();
            });
    }

    [TestMethod(
    DisplayName =
        "SelectByProductCategoryIdAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
    SelectByProductCategoryIdAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
    {
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

        var factory =
            scope.ServiceProvider
                .GetRequiredService<ProductFactory>();

        var repository =
            new ProductRepository(
                context,
                factory);

        await Assert.ThrowsExactlyAsync<InternalException>(
            async () =>
            {
                await repository
                    .SelectByProductCategoryIdAsync(
                        Guid.NewGuid());
            });
    }

    [TestMethod(
    DisplayName =
        "FindByIdAsyncでDB接続エラー時にInternalExceptionが発生する")]
    public async Task
    FindByIdAsync_WhenDatabaseConnectionError_ShouldThrowInternalException()
    {
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

        var factory =
            scope.ServiceProvider
                .GetRequiredService<ProductFactory>();

        var repository =
            new ProductRepository(
                context,
                factory);

        await Assert.ThrowsExactlyAsync<InternalException>(
            async () =>
            {
                await repository.FindByIdAsync(
                    Guid.NewGuid());
            });
    }
}