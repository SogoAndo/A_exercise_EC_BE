using System.Reflection;
using System.Runtime.CompilerServices;
using A_exercise_EC_BE.Applications.Usecases.PaymentMethods;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Presentations.Controllers;
using A_exercise_EC_BE.Presentations.ViewModels.PaymentMethods;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace A_exercise_EC_BE.Presentations.Tests.Controllers;

/// <summary>
/// PaymentMethodControllerの単体テスト。
/// </summary>
[TestClass]
[TestCategory("Presentations/Controllers")]
public class PaymentMethodControllerTests
{
    /// <summary>
    /// 支払い方法一覧取得ユースケースのモック。
    /// </summary>
    private Mock<IFindAllPaymentMethodsUsecase>
        _findAllPaymentMethodsUsecaseMock = null!;

    /// <summary>
    /// テスト対象。
    /// </summary>
    private PaymentMethodController
        _controller = null!;

    /// <summary>
    /// 各テスト実行前の初期化処理。
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _findAllPaymentMethodsUsecaseMock =
            new Mock<IFindAllPaymentMethodsUsecase>();

        _controller =
            new PaymentMethodController(
                _findAllPaymentMethodsUsecaseMock.Object);
    }

    /// <summary>
    /// 支払い方法一覧を
    /// ViewModelへ変換して返すこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindAllAsync_支払い方法一覧をViewModelへ変換して200で返す")]
    public async Task
        FindAllAsync_WhenPaymentMethodsExist_ReturnsOkWithViewModels()
    {
        // Arrange
        var paymentMethods =
            new List<PaymentMethod>
            {
                CreatePaymentMethod(
                    1,
                    "クレジットカード"),

                CreatePaymentMethod(
                    2,
                    "銀行振込"),

                CreatePaymentMethod(
                    3,
                    "コンビニ払い")
            };

        _findAllPaymentMethodsUsecaseMock
            .Setup(
                usecase =>
                    usecase.ExecuteAsync())
            .ReturnsAsync(
                paymentMethods);

        // Act
        var actionResult =
            await _controller.FindAllAsync();

        // Assert
        var okResult =
            actionResult.Result
                as OkObjectResult;

        Assert.IsNotNull(
            okResult);

        Assert.AreEqual(
            200,
            okResult.StatusCode);

        var actual =
            okResult.Value
                as List<
                    PaymentMethodOptionViewModel>;

        Assert.IsNotNull(
            actual);

        Assert.HasCount(
            3,
            actual);

        Assert.AreEqual(
            1,
            actual[0].Value);

        Assert.AreEqual(
            "クレジットカード",
            actual[0].Label);

        Assert.AreEqual(
            2,
            actual[1].Value);

        Assert.AreEqual(
            "銀行振込",
            actual[1].Label);

        Assert.AreEqual(
            3,
            actual[2].Value);

        Assert.AreEqual(
            "コンビニ払い",
            actual[2].Label);

        _findAllPaymentMethodsUsecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(),
            Times.Once);
    }

    /// <summary>
    /// 支払い方法が存在しない場合、
    /// 空の一覧を200で返すこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindAllAsync_支払い方法が存在しない場合は空リストを200で返す")]
    public async Task
        FindAllAsync_WhenPaymentMethodsDoNotExist_ReturnsOkWithEmptyList()
    {
        // Arrange
        _findAllPaymentMethodsUsecaseMock
            .Setup(
                usecase =>
                    usecase.ExecuteAsync())
            .ReturnsAsync(
                new List<PaymentMethod>());

        // Act
        var actionResult =
            await _controller.FindAllAsync();

        // Assert
        var okResult =
            actionResult.Result
                as OkObjectResult;

        Assert.IsNotNull(
            okResult);

        Assert.AreEqual(
            200,
            okResult.StatusCode);

        var actual =
            okResult.Value
                as List<
                    PaymentMethodOptionViewModel>;

        Assert.IsNotNull(
            actual);

        Assert.IsEmpty(
            actual);

        _findAllPaymentMethodsUsecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(),
            Times.Once);
    }

    /// <summary>
    /// ユースケースで例外が発生した場合、
    /// 例外を呼び出し元へ伝えること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindAllAsync_ユースケースで例外が発生した場合は同じ例外を再スローする")]
    public async Task
        FindAllAsync_WhenUsecaseThrowsException_ThrowsSameException()
    {
        // Arrange
        var expected =
            new InvalidOperationException(
                "支払い方法一覧の取得に失敗しました。");

        _findAllPaymentMethodsUsecaseMock
            .Setup(
                usecase =>
                    usecase.ExecuteAsync())
            .ThrowsAsync(
                expected);

        // Act
        var actual =
            await Assert
                .ThrowsExactlyAsync<
                    InvalidOperationException>(
                    async () =>
                    {
                        await _controller
                            .FindAllAsync();
                    });

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _findAllPaymentMethodsUsecaseMock.Verify(
            usecase =>
                usecase.ExecuteAsync(),
            Times.Once);
    }

    /// <summary>
    /// テスト用のPaymentMethodを生成する。
    /// </summary>
    private static PaymentMethod
        CreatePaymentMethod(
            int id,
            string name)
    {
        var paymentMethod =
            (PaymentMethod)RuntimeHelpers
                .GetUninitializedObject(
                    typeof(PaymentMethod));

        SetPrivateProperty(
            paymentMethod,
            nameof(PaymentMethod.Id),
            id);

        SetPrivateProperty(
            paymentMethod,
            nameof(PaymentMethod.Name),
            name);

        return paymentMethod;
    }

    /// <summary>
    /// private setのプロパティへ
    /// テスト用の値を設定する。
    /// </summary>
    private static void SetPrivateProperty<T>(
        T target,
        string propertyName,
        object? value)
    {
        var field =
            typeof(T).GetField(
                $"<{propertyName}>k__BackingField",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        if (field is null)
        {
            throw new InvalidOperationException(
                $"{propertyName}の"
                + "バッキングフィールドが"
                + "見つかりません。");
        }

        field.SetValue(
            target,
            value);
    }
}