using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Entities;
using A_exercise_EC_BE.Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructures.Tests.Repositories;

/// <summary>
/// 注文ステータスRepositoryのテスト。
/// </summary>
[TestClass]
[TestCategory("Infrastructure/Repositories")]
public class OrderStatusRepositoryTests
{
    private AppDbContext _context =
        null!;

    private OrderStatusRepository _repository =
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
            new OrderStatusRepository(
                _context,
                new OrderStatusEntityAdapter());
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
    /// 指定名の注文ステータスを取得できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在する注文ステータス名を指定した場合は注文ステータスを返す")]
    public async Task
        FindByNameAsync_WhenOrderStatusExists_ReturnsOrderStatus()
    {
        // Arrange
        _context.OrderStatuses.Add(
            new OrderStatusEntity
            {
                Id = 1,
                Name = "受付"
            });

        await _context.SaveChangesAsync();

        // Act
        var result =
            await _repository.FindByNameAsync(
                "受付");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(
            1,
            result.Id);
        Assert.AreEqual(
            "受付",
            result.Name);
    }

    /// <summary>
    /// 注文ステータスが存在しない場合はnullを返すこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しない注文ステータス名を指定した場合はnullを返す")]
    public async Task
        FindByNameAsync_WhenOrderStatusDoesNotExist_ReturnsNull()
    {
        // Act
        var result =
            await _repository.FindByNameAsync(
                "存在しないステータス");

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// DBアクセス失敗をInternalExceptionへ変換すること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "注文ステータス取得時にDBアクセスが失敗した場合はInternalExceptionを送出する")]
    public async Task
        FindByNameAsync_WhenDatabaseAccessFails_ThrowsInternalException()
    {
        // Arrange
        const string orderStatusName =
            "受付";

        var errorContext =
            CreateContext();

        var errorRepository =
            new OrderStatusRepository(
                errorContext,
                new OrderStatusEntityAdapter());

        await errorContext.DisposeAsync();

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    InternalException>(
                    () =>
                        errorRepository
                            .FindByNameAsync(
                                orderStatusName));

        // Assert
        Assert.AreEqual(
            $"注文ステータス名:{orderStatusName}"
            + "の注文ステータス取得中に"
            + "予期しないエラーが発生しました。",
            exception.Message);
        Assert.IsNotNull(
            exception.InnerException);
    }

    private static AppDbContext CreateContext()
    {
        var options =
            new DbContextOptionsBuilder<
                AppDbContext>()
                .UseInMemoryDatabase(
                    Guid.NewGuid()
                        .ToString("D"))
                .Options;

        return new AppDbContext(
            options);
    }
}
