using A_exercise_EC_BE.Applications.Usecases;
using A_exercise_EC_BE.Applications.Usecases.Purchases;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace A_exercise_EC_BE.Applications.Tests
    .Usecases.Purchases;

/// <summary>
/// ConfirmPurchaseUsecaseの単体テスト。
/// </summary>
[TestClass]
[TestCategory("Purchases")]
public class ConfirmPurchaseUsecaseTests
{
    private const int PaymentMethodId = 4;
    private const int CreatedOrderId = 10;

    private static readonly Guid CustomerUuid =
        Guid.Parse(
            "11111111-1111-1111-1111-111111111111");

    private static readonly Guid FirstProductUuid =
        Guid.Parse(
            "22222222-2222-2222-2222-222222222222");

    private static readonly Guid SecondProductUuid =
        Guid.Parse(
            "33333333-3333-3333-3333-333333333333");

    private Mock<ICustomerRepository>
        _customerRepositoryMock = null!;

    private Mock<IProductRepository>
        _productRepositoryMock = null!;

    private Mock<IProductStockRepository>
        _productStockRepositoryMock = null!;

    private Mock<IOrderRepository>
        _orderRepositoryMock = null!;

    private Mock<IOrderDetailRepository>
        _orderDetailRepositoryMock = null!;

    private Mock<IPaymentMethodRepository>
        _paymentMethodRepositoryMock = null!;

    private Mock<IOrderStatusRepository>
        _orderStatusRepositoryMock = null!;

    private Mock<IUnitOfWork>
        _unitOfWorkMock = null!;

    private ConfirmPurchaseUsecase
        _usecase = null!;

    /// <summary>
    /// テストの前処理。
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _customerRepositoryMock =
            new Mock<ICustomerRepository>();

        _productRepositoryMock =
            new Mock<IProductRepository>();

        _productStockRepositoryMock =
            new Mock<IProductStockRepository>();

        _orderRepositoryMock =
            new Mock<IOrderRepository>();

        _orderDetailRepositoryMock =
            new Mock<IOrderDetailRepository>();

        _paymentMethodRepositoryMock =
            new Mock<IPaymentMethodRepository>();

        _orderStatusRepositoryMock =
            new Mock<IOrderStatusRepository>();

        _unitOfWorkMock =
            new Mock<IUnitOfWork>();

        _customerRepositoryMock
            .Setup(repository =>
                repository
                    .FindByCustomerUuidAsync(
                        CustomerUuid))
            .ReturnsAsync(
                CreateCustomer());

        _paymentMethodRepositoryMock
            .Setup(repository =>
                repository.FindByIdAsync(
                    PaymentMethodId))
            .ReturnsAsync(
                new PaymentMethod(
                    PaymentMethodId,
                    "銀行振込"));

        _orderStatusRepositoryMock
            .Setup(repository =>
                repository.FindByNameAsync(
                    "受付"))
            .ReturnsAsync(
                new OrderStatus(
                    1,
                    "受付"));

        _productRepositoryMock
            .Setup(repository =>
                repository.FindByIdAsync(
                    FirstProductUuid))
            .ReturnsAsync(
                new Product(
                    FirstProductUuid,
                    "ボールペン",
                    100));

        _productRepositoryMock
            .Setup(repository =>
                repository.FindByIdAsync(
                    SecondProductUuid))
            .ReturnsAsync(
                new Product(
                    SecondProductUuid,
                    "ノート",
                    200));

        _productStockRepositoryMock
            .Setup(repository =>
                repository.TryDecreaseAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<int>()))
            .ReturnsAsync(true);

        _orderRepositoryMock
            .Setup(repository =>
                repository.CreateAsync(
                    It.IsAny<Orders>()))
            .ReturnsAsync(
                CreatedOrderId);

        _orderDetailRepositoryMock
            .Setup(repository =>
                repository.CreateRangeAsync(
                    It.IsAny<int>(),
                    It.IsAny<List<OrdersDetail>>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.BeginAsync())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.CommitAsync())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(unitOfWork =>
                unitOfWork.RollbackAsync())
            .Returns(Task.CompletedTask);

        _usecase =
            new ConfirmPurchaseUsecase(
                _customerRepositoryMock.Object,
                _productRepositoryMock.Object,
                _productStockRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _orderDetailRepositoryMock.Object,
                _paymentMethodRepositoryMock.Object,
                _orderStatusRepositoryMock.Object,
                new PurchaseAmountCalculator(),
                _unitOfWorkMock.Object);
    }

    /// <summary>
    /// 購入を確定できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ConfirmAsync_有効な購入情報の場合は"
            + "注文と注文明細を登録して在庫を減らす")]
    public async Task
        ConfirmAsync_WhenRequestIsValid_ConfirmsPurchase()
    {
        // Arrange
        var request =
            CreateValidRequest();

        Orders? createdOrder = null;
        List<OrdersDetail>? createdDetails = null;

        _orderRepositoryMock
            .Setup(repository =>
                repository.CreateAsync(
                    It.IsAny<Orders>()))
            .Callback<Orders>(
                order =>
                    createdOrder = order)
            .ReturnsAsync(
                CreatedOrderId);

        _orderDetailRepositoryMock
            .Setup(repository =>
                repository.CreateRangeAsync(
                    CreatedOrderId,
                    It.IsAny<List<OrdersDetail>>()))
            .Callback<
                int,
                List<OrdersDetail>>(
                (_, details) =>
                    createdDetails = details)
            .Returns(Task.CompletedTask);

        var before =
            DateTime.Now;

        // Act
        var result =
            await _usecase.ConfirmAsync(
                request);

        var after =
            DateTime.Now;

        // Assert
        Assert.IsNotNull(
            createdOrder);

        Assert.IsNotNull(
            createdDetails);

        Assert.AreEqual(
            CustomerUuid,
            createdOrder.Customer
                .CustomerUuid);

        Assert.AreEqual(
            "銀行振込",
            createdOrder.PaymentMethod
                .Name);

        Assert.AreEqual(
            "受付",
            createdOrder.OrderStatus
                .Name);

        Assert.AreEqual(
            800,
            createdOrder.AmountTotal);

        Assert.HasCount(
            2,
            createdDetails);

        Assert.AreEqual(
            createdOrder.OrderUuid,
            result.OrderUuid);

        Assert.AreEqual(
            createdOrder.OrderDate,
            result.OrderDate);

        Assert.AreEqual(
            800,
            result.AmountTotal);

        Assert.IsTrue(
            result.OrderDate >= before
            && result.OrderDate <= after);

        _productStockRepositoryMock
            .Verify(
                repository =>
                    repository.TryDecreaseAsync(
                        FirstProductUuid,
                        2),
                Times.Once);

        _productStockRepositoryMock
            .Verify(
                repository =>
                    repository.TryDecreaseAsync(
                        SecondProductUuid,
                        3),
                Times.Once);

        _orderDetailRepositoryMock
            .Verify(
                repository =>
                    repository.CreateRangeAsync(
                        CreatedOrderId,
                        It.IsAny<
                            List<OrdersDetail>>()),
                Times.Once);

        VerifyCommitted();
    }

    /// <summary>
    /// 購入情報がnullの場合は
    /// 処理を開始しないこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ConfirmAsync_購入情報がnullの場合は"
            + "DomainExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenRequestIsNull_ThrowsDomainException()
    {
        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    DomainException>(
                    async () =>
                    {
                        await _usecase
                            .ConfirmAsync(null!);
                    });

        // Assert
        Assert.AreEqual(
            "購入情報を入力してください",
            exception.Message);

        VerifyTransactionNotStarted();
    }

    [TestMethod(
        DisplayName =
            "ConfirmAsync_顧客UUIDが空の場合は"
            + "DomainExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenCustomerUuidIsEmpty_ThrowsDomainException()
    {
        var request = new ConfirmPurchaseRequest(
            Guid.Empty,
            PaymentMethodId,
            [
                new ConfirmPurchaseItemRequest(
                    FirstProductUuid,
                    1)
            ]);

        var exception = await Assert.ThrowsExactlyAsync<DomainException>(
            () => _usecase.ConfirmAsync(request));

        Assert.AreEqual(
            "顧客識別IDが不正です",
            exception.Message);
        VerifyTransactionNotStarted();
    }

    /// <summary>
    /// カートが空の場合は
    /// 処理を開始しないこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ConfirmAsync_カートが空の場合は"
            + "DomainExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenCartIsEmpty_ThrowsDomainException()
    {
        // Arrange
        var request =
            new ConfirmPurchaseRequest(
                CustomerUuid,
                PaymentMethodId,
                []);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    DomainException>(
                    async () =>
                    {
                        await _usecase
                            .ConfirmAsync(request);
                    });

        // Assert
        Assert.AreEqual(
            "カートに商品がありません",
            exception.Message);

        VerifyTransactionNotStarted();
    }

    [TestMethod(
        DisplayName =
            "ConfirmAsync_商品一覧がnullの場合は"
            + "DomainExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenItemsIsNull_ThrowsDomainException()
    {
        var request = new ConfirmPurchaseRequest(
            CustomerUuid,
            PaymentMethodId,
            null!);

        var exception = await Assert.ThrowsExactlyAsync<DomainException>(
            () => _usecase.ConfirmAsync(request));

        Assert.AreEqual(
            "カートに商品がありません",
            exception.Message);
        VerifyTransactionNotStarted();
    }

    [TestMethod(
        DisplayName =
            "ConfirmAsync_商品UUIDが空の場合は"
            + "DomainExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenProductUuidIsEmpty_ThrowsDomainException()
    {
        var request = new ConfirmPurchaseRequest(
            CustomerUuid,
            PaymentMethodId,
            [
                new ConfirmPurchaseItemRequest(
                    Guid.Empty,
                    1)
            ]);

        var exception = await Assert.ThrowsExactlyAsync<DomainException>(
            () => _usecase.ConfirmAsync(request));

        Assert.AreEqual(
            "商品識別IDが不正です",
            exception.Message);
        VerifyTransactionNotStarted();
    }

    /// <summary>
    /// 支払い方法が未選択の場合は
    /// 処理を開始しないこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ConfirmAsync_支払い方法が未選択の場合は"
            + "DomainExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenPaymentMethodIsNotSelected_ThrowsDomainException()
    {
        // Arrange
        var request =
            new ConfirmPurchaseRequest(
                CustomerUuid,
                0,
                [
                    new ConfirmPurchaseItemRequest(
                        FirstProductUuid,
                        1)
                ]);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    DomainException>(
                    async () =>
                    {
                        await _usecase
                            .ConfirmAsync(request);
                    });

        // Assert
        Assert.AreEqual(
            "支払い方法を選択してください",
            exception.Message);

        VerifyTransactionNotStarted();
    }

    /// <summary>
    /// 購入数量が0の場合は
    /// 処理を開始しないこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ConfirmAsync_購入数量が0の場合は"
            + "DomainExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenQuantityIsZero_ThrowsDomainException()
    {
        // Arrange
        var request =
            new ConfirmPurchaseRequest(
                CustomerUuid,
                PaymentMethodId,
                [
                    new ConfirmPurchaseItemRequest(
                        FirstProductUuid,
                        0)
                ]);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    DomainException>(
                    async () =>
                    {
                        await _usecase
                            .ConfirmAsync(request);
                    });

        // Assert
        Assert.AreEqual(
            "購入数量は1以上で入力してください。",
            exception.Message);

        VerifyTransactionNotStarted();
    }

    /// <summary>
    /// 顧客が存在しない場合は
    /// 処理を開始しないこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ConfirmAsync_顧客が存在しない場合は"
            + "NotFoundExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenCustomerDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _customerRepositoryMock
            .Setup(repository =>
                repository
                    .FindByCustomerUuidAsync(
                        CustomerUuid))
            .ReturnsAsync(
                (Customer?)null);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    NotFoundException>(
                    async () =>
                    {
                        await _usecase
                            .ConfirmAsync(
                                CreateValidRequest());
                    });

        // Assert
        Assert.AreEqual(
            "顧客アカウントが見つかりません",
            exception.Message);

        VerifyTransactionNotStarted();
    }

    /// <summary>
    /// 銀行振込以外の場合は
    /// 処理を開始しないこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ConfirmAsync_銀行振込以外の場合は"
            + "DomainExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenPaymentMethodIsUnsupported_ThrowsDomainException()
    {
        // Arrange
        _paymentMethodRepositoryMock
            .Setup(repository =>
                repository.FindByIdAsync(
                    PaymentMethodId))
            .ReturnsAsync(
                new PaymentMethod(
                    PaymentMethodId,
                    "クレジットカード"));

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    DomainException>(
                    async () =>
                    {
                        await _usecase
                            .ConfirmAsync(
                                CreateValidRequest());
                    });

        // Assert
        Assert.AreEqual(
            "支払い方法は銀行振込のみ選択できます",
            exception.Message);

        VerifyTransactionNotStarted();
    }

    [TestMethod(
        DisplayName =
            "ConfirmAsync_支払い方法が存在しない場合は"
            + "NotFoundExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenPaymentMethodDoesNotExist_ThrowsNotFoundException()
    {
        _paymentMethodRepositoryMock
            .Setup(repository =>
                repository.FindByIdAsync(
                    PaymentMethodId))
            .ReturnsAsync(
                (PaymentMethod?)null);

        var exception =
            await Assert.ThrowsExactlyAsync<NotFoundException>(
                () => _usecase.ConfirmAsync(
                    CreateValidRequest()));

        Assert.AreEqual(
            "支払い方法が見つかりません",
            exception.Message);
        VerifyTransactionNotStarted();
    }

    [TestMethod(
        DisplayName =
            "ConfirmAsync_初期注文ステータスがない場合は"
            + "InternalExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenInitialOrderStatusDoesNotExist_ThrowsInternalException()
    {
        _orderStatusRepositoryMock
            .Setup(repository =>
                repository.FindByNameAsync(
                    "受付"))
            .ReturnsAsync(
                (OrderStatus?)null);

        var exception =
            await Assert.ThrowsExactlyAsync<InternalException>(
                () => _usecase.ConfirmAsync(
                    CreateValidRequest()));

        Assert.AreEqual(
            "初期注文ステータスが登録されていません",
            exception.Message);
        VerifyTransactionNotStarted();
    }

    /// <summary>
    /// 商品が存在しない場合は
    /// 処理を開始しないこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ConfirmAsync_商品が存在しない場合は"
            + "NotFoundExceptionをスローする")]
    public async Task
        ConfirmAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _productRepositoryMock
            .Setup(repository =>
                repository.FindByIdAsync(
                    FirstProductUuid))
            .ReturnsAsync(
                (Product?)null);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    NotFoundException>(
                    async () =>
                    {
                        await _usecase
                            .ConfirmAsync(
                                CreateValidRequest());
                    });

        // Assert
        Assert.AreEqual(
            $"商品ID:{FirstProductUuid}"
            + "の商品が見つかりません",
            exception.Message);

        VerifyTransactionNotStarted();
    }

    /// <summary>
    /// 在庫不足の場合は
    /// トランザクションをロールバックすること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ConfirmAsync_在庫不足の場合は"
            + "ロールバックする")]
    public async Task
        ConfirmAsync_WhenStockIsInsufficient_RollsBack()
    {
        // Arrange
        _productStockRepositoryMock
            .Setup(repository =>
                repository.TryDecreaseAsync(
                    SecondProductUuid,
                    3))
            .ReturnsAsync(false);

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    DomainException>(
                    async () =>
                    {
                        await _usecase
                            .ConfirmAsync(
                                CreateValidRequest());
                    });

        // Assert
        Assert.AreEqual(
            "申し訳ありませんが、"
            + "商品「ノート」"
            + "の在庫が不足しています",
            exception.Message);

        _productStockRepositoryMock.Verify(
            repository =>
                repository.TryDecreaseAsync(
                    FirstProductUuid,
                    2),
            Times.Once);

        _orderRepositoryMock.Verify(
            repository =>
                repository.CreateAsync(
                    It.IsAny<Orders>()),
            Times.Never);

        VerifyRolledBack();
    }

    /// <summary>
    /// 注文登録に失敗した場合は
    /// トランザクションをロールバックすること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ConfirmAsync_注文登録に失敗した場合は"
            + "ロールバックする")]
    public async Task
        ConfirmAsync_WhenCreatingOrderFails_RollsBack()
    {
        // Arrange
        _orderRepositoryMock
            .Setup(repository =>
                repository.CreateAsync(
                    It.IsAny<Orders>()))
            .ThrowsAsync(
                new InternalException(
                    "注文登録エラー"));

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    InternalException>(
                    async () =>
                    {
                        await _usecase
                            .ConfirmAsync(
                                CreateValidRequest());
                    });

        // Assert
        Assert.AreEqual(
            "注文登録エラー",
            exception.Message);

        _orderDetailRepositoryMock.Verify(
            repository =>
                repository.CreateRangeAsync(
                    It.IsAny<int>(),
                    It.IsAny<
                        List<OrdersDetail>>()),
            Times.Never);

        VerifyRolledBack();
    }

    /// <summary>
    /// 注文明細登録に失敗した場合は
    /// トランザクションをロールバックすること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ConfirmAsync_注文明細登録に失敗した場合は"
            + "ロールバックする")]
    public async Task
        ConfirmAsync_WhenCreatingOrderDetailsFails_RollsBack()
    {
        // Arrange
        _orderDetailRepositoryMock
            .Setup(repository =>
                repository.CreateRangeAsync(
                    CreatedOrderId,
                    It.IsAny<
                        List<OrdersDetail>>()))
            .ThrowsAsync(
                new InternalException(
                    "注文明細登録エラー"));

        // Act
        var exception =
            await Assert
                .ThrowsExactlyAsync<
                    InternalException>(
                    async () =>
                    {
                        await _usecase
                            .ConfirmAsync(
                                CreateValidRequest());
                    });

        // Assert
        Assert.AreEqual(
            "注文明細登録エラー",
            exception.Message);

        VerifyRolledBack();
    }

    private static ConfirmPurchaseRequest
        CreateValidRequest()
    {
        return new ConfirmPurchaseRequest(
            CustomerUuid,
            PaymentMethodId,
            [
                new ConfirmPurchaseItemRequest(
                    FirstProductUuid,
                    2),
                new ConfirmPurchaseItemRequest(
                    SecondProductUuid,
                    3)
            ]);
    }

    private static Customer CreateCustomer()
    {
        return new Customer(
            CustomerUuid,
            "山田太郎",
            "ヤマダタロウ",
            "東京都千代田区1-1",
            null,
            "090-1234-5678",
            "yamada@example.com",
            "yamada",
            new string('h', 64),
            new DateTime(
                2026,
                7,
                1,
                9,
                0,
                0));
    }

    private void VerifyCommitted()
    {
        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.CommitAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.RollbackAsync(),
            Times.Never);
    }

    private void VerifyRolledBack()
    {
        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.RollbackAsync(),
            Times.Once);
    }

    private void VerifyTransactionNotStarted()
    {
        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.BeginAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            unitOfWork =>
                unitOfWork.RollbackAsync(),
            Times.Never);
    }
}
