using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Entities;
using A_exercise_EC_BE.Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_exercise_EC_BE.Infrastructures.Tests.Repositories;

/// <summary>
/// ProductCategoryRepositoryの単体テスト。
/// </summary>
[TestClass]
[TestCategory("Infrastructure/Repositories")]
public class ProductCategoryRepositoryTests
{
    private AppDbContext _context =
        null!;

    private ProductCategoryRepository _repository =
        null!;

    /// <summary>
    /// テストごとに独立したDBとRepositoryを生成する。
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _context =
            CreateContext();

        _repository =
            new ProductCategoryRepository(
                _context,
                new ProductCategoryEntityAdapter());
    }

    /// <summary>
    /// テスト用DBを破棄する。
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        _context.Dispose();
    }

    /// <summary>
    /// 商品カテゴリが存在しない場合、
    /// 空のリストを返すこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindAllAsync_商品カテゴリが存在しない場合は空のリストを返す")]
    public async Task
        FindAllAsync_WhenProductCategoriesDoNotExist_ReturnsEmptyList()
    {
        // Act
        var actual =
            await _repository.FindAllAsync();

        // Assert
        Assert.IsNotNull(actual);

        Assert.IsEmpty(
            actual);
    }

    /// <summary>
    /// すべての商品カテゴリを
    /// IDの昇順で取得できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindAllAsync_すべての商品カテゴリをIDの昇順で取得できる")]
    public async Task
        FindAllAsync_WhenProductCategoriesExist_ReturnsAllOrderedById()
    {
        // Arrange
        var firstCategoryUuid =
            Guid.NewGuid();

        var secondCategoryUuid =
            Guid.NewGuid();

        /*
         * IDが大きいデータを先に登録し、
         * Repository内のOrderByが機能することを確認する。
         */
        var secondEntity =
            new ProductCategoryEntity
            {
                Id = 20,
                CategoryUuid = secondCategoryUuid,
                Name = "文房具"
            };

        var firstEntity =
            new ProductCategoryEntity
            {
                Id = 10,
                CategoryUuid = firstCategoryUuid,
                Name = "食品"
            };

        await _context.ProductCategories.AddAsync(
            secondEntity);

        await _context.ProductCategories.AddAsync(
            firstEntity);

        await _context.SaveChangesAsync();

        /*
         * AsNoTrackingの確認を行えるように、
         * 登録時の追跡情報を削除する。
         */
        _context.ChangeTracker.Clear();

        // Act
        var actual =
            await _repository.FindAllAsync();

        // Assert
        Assert.HasCount(
            2,
            actual);

        Assert.AreEqual(
            "食品",
            actual[0].Name);

        Assert.AreEqual(
            "文房具",
            actual[1].Name);

        /*
         * AsNoTrackingが指定されているため、
         * 取得後もエンティティは追跡されない。
         */
        Assert.AreEqual(
            0,
            _context.ChangeTracker
                .Entries<ProductCategoryEntity>()
                .Count());
    }

    /// <summary>
    /// 商品カテゴリを1件取得できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindAllAsync_商品カテゴリを1件取得できる")]
    public async Task
        FindAllAsync_WhenOneProductCategoryExists_ReturnsOneCategory()
    {
        // Arrange
        var categoryUuid =
            Guid.NewGuid();

        var entity =
            new ProductCategoryEntity
            {
                Id = 1,
                CategoryUuid = categoryUuid,
                Name = "家電"
            };

        await _context.ProductCategories.AddAsync(
            entity);

        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Act
        var actual =
            await _repository.FindAllAsync();

        // Assert
        Assert.HasCount(
            1,
            actual);

        Assert.AreEqual(
            "家電",
            actual[0].Name);
    }

    /// <summary>
    /// DBアクセス時に例外が発生した場合、
    /// InternalExceptionにラップしてスローすること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindAllAsync_DBアクセス時に例外が発生した場合はInternalExceptionにラップする")]
    public async Task
        FindAllAsync_WhenDatabaseAccessThrowsException_ThrowsInternalException()
    {
        // Arrange
        _context.Dispose();

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<
                InternalException>(
                async () =>
                {
                    await _repository.FindAllAsync();
                });

        // Assert
        Assert.AreEqual(
            "すべての商品カテゴリ取得時に予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);

        Assert.IsInstanceOfType<
            ObjectDisposedException>(
                exception.InnerException);
    }

    /// <summary>
    /// テストごとに独立した
    /// InMemoryデータベースを生成する。
    /// </summary>
    /// <returns>
    /// テスト用AppDbContext。
    /// </returns>
    private static AppDbContext CreateContext()
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(
                    $"ProductCategoryRepositoryTests_" +
                    $"{Guid.NewGuid()}")
                .Options;

        return new AppDbContext(options);
    }
}