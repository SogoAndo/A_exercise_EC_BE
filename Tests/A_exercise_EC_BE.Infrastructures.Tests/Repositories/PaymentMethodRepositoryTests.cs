using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Entities;
using A_exercise_EC_BE.Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_exercise_EC_BE.Infrastructures.Tests.Repositories;

/// <summary>
/// 支払い方法Repositoryのテスト。
/// </summary>
[TestClass]
[TestCategory("Infrastructure/Repositories")]
public class PaymentMethodRepositoryTests
{
    private AppDbContext _context =
        null!;

    private PaymentMethodRepository _repository =
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
            new PaymentMethodRepository(
                _context,
                new PaymentMethodEntityAdapter());
    }

    /// <summary>
    /// テスト用DBを破棄する。
    /// </summary>
    [TestCleanup]
    public void TestCleanup()
    {
        _context.Dispose();
    }

    /*
     * FindAllAsync
     */

    /// <summary>
    /// 支払い方法一覧をIDの昇順で取得できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "支払い方法が存在する場合はIDの昇順で一覧を返す")]
    public async Task
        FindAllAsync_WhenPaymentMethodsExist_ReturnsOrderedPaymentMethods()
    {
        // Arrange
        _context.PaymentMethods.AddRange(
            new PaymentMethodEntity
            {
                Id = 3,
                Name = "銀行振込"
            },
            new PaymentMethodEntity
            {
                Id = 1,
                Name = "クレジットカード"
            },
            new PaymentMethodEntity
            {
                Id = 2,
                Name = "コンビニ払い"
            });

        await _context.SaveChangesAsync();

        // Act
        var result =
            await _repository.FindAllAsync();

        // Assert
        Assert.IsNotNull(result);

        Assert.HasCount(
            3,
            result);

        Assert.AreEqual(
            1,
            result[0].Id);

        Assert.AreEqual(
            "クレジットカード",
            result[0].Name);

        Assert.AreEqual(
            2,
            result[1].Id);

        Assert.AreEqual(
            "コンビニ払い",
            result[1].Name);

        Assert.AreEqual(
            3,
            result[2].Id);

        Assert.AreEqual(
            "銀行振込",
            result[2].Name);
    }

    /// <summary>
    /// 支払い方法が存在しない場合は
    /// 空リストを返すこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "支払い方法が存在しない場合は空リストを返す")]
    public async Task
        FindAllAsync_WhenPaymentMethodsDoNotExist_ReturnsEmptyList()
    {
        // Act
        var result =
            await _repository.FindAllAsync();

        // Assert
        Assert.IsNotNull(result);

        Assert.IsEmpty(result);
    }

    /// <summary>
    /// 支払い方法一覧取得中のDBアクセス失敗を
    /// InternalExceptionへ変換すること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "支払い方法一覧取得時にDBアクセスが失敗した場合はInternalExceptionを送出する")]
    public async Task
        FindAllAsync_WhenDatabaseAccessFails_ThrowsInternalException()
    {
        // Arrange
        var errorContext =
            CreateContext();

        var errorRepository =
            new PaymentMethodRepository(
                errorContext,
                new PaymentMethodEntityAdapter());

        await errorContext.DisposeAsync();

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .FindAllAsync();
                    });

        // Assert
        Assert.AreEqual(
            "支払い方法一覧取得中に"
            + "予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);
    }

    /*
     * FindByIdAsync
     */

    /// <summary>
    /// 指定IDの支払い方法を取得できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在する支払い方法IDを指定した場合は支払い方法を返す")]
    public async Task
        FindByIdAsync_WhenPaymentMethodExists_ReturnsPaymentMethod()
    {
        // Arrange
        const int paymentMethodId =
            4;

        _context.PaymentMethods.Add(
            new PaymentMethodEntity
            {
                Id = paymentMethodId,
                Name = "銀行振込"
            });

        await _context.SaveChangesAsync();

        // Act
        var result =
            await _repository.FindByIdAsync(
                paymentMethodId);

        // Assert
        Assert.IsNotNull(result);

        Assert.AreEqual(
            paymentMethodId,
            result.Id);

        Assert.AreEqual(
            "銀行振込",
            result.Name);
    }

    /// <summary>
    /// 支払い方法が存在しない場合は
    /// nullを返すこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しない支払い方法IDを指定した場合はnullを返す")]
    public async Task
        FindByIdAsync_WhenPaymentMethodDoesNotExist_ReturnsNull()
    {
        // Act
        var result =
            await _repository.FindByIdAsync(
                999);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// DBアクセス失敗を
    /// InternalExceptionへ変換すること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "支払い方法取得時にDBアクセスが失敗した場合はInternalExceptionを送出する")]
    public async Task
        FindByIdAsync_WhenDatabaseAccessFails_ThrowsInternalException()
    {
        // Arrange
        const int paymentMethodId =
            4;

        var errorContext =
            CreateContext();

        var errorRepository =
            new PaymentMethodRepository(
                errorContext,
                new PaymentMethodEntityAdapter());

        await errorContext.DisposeAsync();

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    InternalException>(
                    async () =>
                    {
                        await errorRepository
                            .FindByIdAsync(
                                paymentMethodId);
                    });

        // Assert
        Assert.AreEqual(
            $"支払い方法ID:{paymentMethodId}"
            + "の支払い方法取得中に"
            + "予期しないエラーが発生しました。",
            exception.Message);

        Assert.IsNotNull(
            exception.InnerException);
    }

    /// <summary>
    /// テストごとに独立した
    /// InMemoryデータベースを生成する。
    /// </summary>
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