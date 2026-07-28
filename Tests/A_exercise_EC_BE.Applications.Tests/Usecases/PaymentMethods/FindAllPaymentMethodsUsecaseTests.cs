using A_exercise_EC_BE.Applications.Usecases.PaymentMethods;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace A_exercise_EC_BE.Applications.Tests.Usecases.PaymentMethods;

/// <summary>
/// FindAllPaymentMethodsUsecaseの単体テスト。
/// </summary>
[TestClass]
[TestCategory("Applications/Usecases/PaymentMethods")]
public class FindAllPaymentMethodsUsecaseTests
{
    /// <summary>
    /// 支払い方法Repositoryのモック。
    /// </summary>
    private Mock<IPaymentMethodRepository>
        _paymentMethodRepositoryMock = null!;

    /// <summary>
    /// テスト対象。
    /// </summary>
    private FindAllPaymentMethodsUsecase
        _usecase = null!;

    /// <summary>
    /// 各テスト実行前の初期化処理。
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _paymentMethodRepositoryMock =
            new Mock<IPaymentMethodRepository>();

        _usecase =
            new FindAllPaymentMethodsUsecase(
                _paymentMethodRepositoryMock.Object);
    }

    /// <summary>
    /// Repositoryから取得した支払い方法一覧を
    /// そのまま返すこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExecuteAsync_支払い方法一覧を取得できる")]
    public async Task
        ExecuteAsync_WhenPaymentMethodsExist_ReturnsPaymentMethods()
    {
        // Arrange
        var expected =
            new List<PaymentMethod>();

        _paymentMethodRepositoryMock
            .Setup(
                repository =>
                    repository.FindAllAsync())
            .ReturnsAsync(expected);

        // Act
        var actual =
            await _usecase.ExecuteAsync();

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _paymentMethodRepositoryMock.Verify(
            repository =>
                repository.FindAllAsync(),
            Times.Once);
    }

    /// <summary>
    /// Repositoryが空の一覧を返した場合、
    /// 空の一覧をそのまま返すこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExecuteAsync_支払い方法が存在しない場合は空リストを返す")]
    public async Task
        ExecuteAsync_WhenPaymentMethodsDoNotExist_ReturnsEmptyList()
    {
        // Arrange
        var expected =
            new List<PaymentMethod>();

        _paymentMethodRepositoryMock
            .Setup(
                repository =>
                    repository.FindAllAsync())
            .ReturnsAsync(expected);

        // Act
        var actual =
            await _usecase.ExecuteAsync();

        // Assert
        Assert.IsNotNull(
            actual);

        Assert.IsEmpty(
            actual);

        Assert.AreSame(
            expected,
            actual);

        _paymentMethodRepositoryMock.Verify(
            repository =>
                repository.FindAllAsync(),
            Times.Once);
    }

    /// <summary>
    /// Repositoryで例外が発生した場合、
    /// 例外をそのまま呼び出し元へ伝えること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExecuteAsync_Repositoryで例外が発生した場合は例外を再スローする")]
    public async Task
        ExecuteAsync_WhenRepositoryThrowsException_ThrowsSameException()
    {
        // Arrange
        var expected =
            new InvalidOperationException(
                "支払い方法一覧の取得に失敗しました。");

        _paymentMethodRepositoryMock
            .Setup(
                repository =>
                    repository.FindAllAsync())
            .ThrowsAsync(expected);

        // Act
        var actual =
            await Assert
                .ThrowsExactlyAsync<
                    InvalidOperationException>(
                    async () =>
                    {
                        await _usecase
                            .ExecuteAsync();
                    });

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _paymentMethodRepositoryMock.Verify(
            repository =>
                repository.FindAllAsync(),
            Times.Once);
    }
}