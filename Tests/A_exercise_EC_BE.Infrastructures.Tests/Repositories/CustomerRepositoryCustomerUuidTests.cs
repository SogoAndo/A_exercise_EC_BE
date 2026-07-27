using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Entities;
using A_exercise_EC_BE.Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructures.Tests.Repositories;

/// <summary>
/// 顧客UUID検索のテスト。
/// </summary>
[TestClass]
[TestCategory("Infrastructure/Repositories")]
public class CustomerRepositoryCustomerUuidTests
{
    private static readonly Guid CustomerUuid =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    private AppDbContext _context =
        null!;

    private CustomerRepository _repository =
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
            new CustomerRepository(
                _context,
                new CustomerEntityAdapter());
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
    /// UUIDに一致する顧客を取得できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在する顧客UUIDを指定した場合は顧客を返す")]
    public async Task
        FindByCustomerUuidAsync_WhenCustomerExists_ReturnsCustomer()
    {
        // Arrange
        _context.Customers.Add(
            CreateCustomerEntity());

        await _context.SaveChangesAsync();

        // Act
        var result =
            await _repository
                .FindByCustomerUuidAsync(
                    CustomerUuid);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(
            CustomerUuid,
            result.CustomerUuid);
        Assert.AreEqual(
            "ando@example.com",
            result.MailAddress);
    }

    /// <summary>
    /// UUIDに一致する顧客がいなければnullを返すこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "存在しない顧客UUIDを指定した場合はnullを返す")]
    public async Task
        FindByCustomerUuidAsync_WhenCustomerDoesNotExist_ReturnsNull()
    {
        // Act
        var result =
            await _repository
                .FindByCustomerUuidAsync(
                    CustomerUuid);

        // Assert
        Assert.IsNull(result);
    }

    /// <summary>
    /// DBアクセス失敗をInternalExceptionへ変換すること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "顧客UUID検索時にDBアクセスが失敗した場合はInternalExceptionを送出する")]
    public async Task
        FindByCustomerUuidAsync_WhenDatabaseAccessFails_ThrowsInternalException()
    {
        // Arrange
        await _context.DisposeAsync();

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    InternalException>(
                    () =>
                        _repository
                            .FindByCustomerUuidAsync(
                                CustomerUuid));

        // Assert
        Assert.AreEqual(
            $"顧客UUID:{CustomerUuid}"
            + "の顧客取得中に予期しないエラーが発生しました。",
            exception.Message);
        Assert.IsNotNull(
            exception.InnerException);
    }

    private static CustomerEntity
        CreateCustomerEntity()
    {
        return new CustomerEntity
        {
            CustomerUuid =
                CustomerUuid,
            Name =
                "安藤太郎",
            Kana =
                "アンドウタロウ",
            Address1 =
                "東京都千代田区1-1",
            PhoneNumber =
                "090-1234-5678",
            MailAddress =
                "ando@example.com",
            Username =
                "ando",
            Password =
                new string('h', 64),
            CreatedAt =
                new DateTime(
                    2026,
                    7,
                    1,
                    9,
                    0,
                    0)
        };
    }

    private static AppDbContext
        CreateContext()
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
