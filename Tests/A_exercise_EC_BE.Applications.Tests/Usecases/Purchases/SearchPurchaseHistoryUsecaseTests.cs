using A_exercise_EC_BE.Applications.Usecases.Purchases;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using Moq;

namespace A_exercise_EC_BE.Applications.Tests.Usecases.Purchases;

[TestClass]
[TestCategory("Applications/Usecases/Purchases")]
public class SearchPurchaseHistoryUsecaseTests
{
    [TestMethod(DisplayName = "顧客UUIDに紐づく購入履歴一覧を取得する")]
    public async Task SearchAsync_WhenOrdersExist_ReturnsOrders()
    {
        var customerUuid = Guid.NewGuid();
        var orders = new List<Orders>
        {
            CreateOrder(customerUuid)
        };
        var repository = new Mock<IOrderRepository>();
        repository
            .Setup(x => x.FindByCustomerUuidAsync(customerUuid))
            .ReturnsAsync(orders);
        var usecase = new SearchPurchaseHistoryUsecase(
            repository.Object);

        var result = await usecase.SearchAsync(customerUuid);

        Assert.AreSame(orders, result);
        repository.Verify(
            x => x.FindByCustomerUuidAsync(customerUuid),
            Times.Once);
    }

    [TestMethod(DisplayName = "購入履歴がない場合は空の一覧を返す")]
    public async Task SearchAsync_WhenOrdersDoNotExist_ReturnsEmptyList()
    {
        var customerUuid = Guid.NewGuid();
        var repository = new Mock<IOrderRepository>();
        repository
            .Setup(x => x.FindByCustomerUuidAsync(customerUuid))
            .ReturnsAsync([]);
        var usecase = new SearchPurchaseHistoryUsecase(
            repository.Object);

        var result = await usecase.SearchAsync(customerUuid);

        Assert.IsEmpty(result);
    }

    [TestMethod(DisplayName = "顧客自身の購入履歴詳細を取得する")]
    public async Task FindDetailAsync_WhenOwnedOrderExists_ReturnsOrder()
    {
        var customerUuid = Guid.NewGuid();
        var order = CreateOrder(customerUuid);
        var repository = new Mock<IOrderRepository>();
        repository
            .Setup(x => x.FindByOrderUuidAsync(order.OrderUuid))
            .ReturnsAsync(order);
        var usecase = new SearchPurchaseHistoryUsecase(
            repository.Object);

        var result = await usecase.FindDetailAsync(
            customerUuid,
            order.OrderUuid);

        Assert.AreSame(order, result);
    }

    [TestMethod(DisplayName = "別顧客の購入履歴詳細は返さない")]
    public async Task FindDetailAsync_WhenOrderBelongsToAnotherCustomer_ReturnsNull()
    {
        var customerUuid = Guid.NewGuid();
        var order = CreateOrder(Guid.NewGuid());
        var repository = new Mock<IOrderRepository>();
        repository
            .Setup(x => x.FindByOrderUuidAsync(order.OrderUuid))
            .ReturnsAsync(order);
        var usecase = new SearchPurchaseHistoryUsecase(
            repository.Object);

        var result = await usecase.FindDetailAsync(
            customerUuid,
            order.OrderUuid);

        Assert.IsNull(result);
    }

    [TestMethod(DisplayName = "存在しない購入履歴詳細はnullを返す")]
    public async Task FindDetailAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        var customerUuid = Guid.NewGuid();
        var orderUuid = Guid.NewGuid();
        var repository = new Mock<IOrderRepository>();
        repository
            .Setup(x => x.FindByOrderUuidAsync(orderUuid))
            .ReturnsAsync((Orders?)null);
        var usecase = new SearchPurchaseHistoryUsecase(
            repository.Object);

        var result = await usecase.FindDetailAsync(
            customerUuid,
            orderUuid);

        Assert.IsNull(result);
    }

    [TestMethod(DisplayName = "空の顧客UUIDでは購入履歴を検索しない")]
    public async Task SearchAsync_WhenCustomerUuidIsEmpty_ThrowsDomainException()
    {
        var repository = new Mock<IOrderRepository>();
        var usecase = new SearchPurchaseHistoryUsecase(
            repository.Object);

        var exception = await Assert.ThrowsExactlyAsync<DomainException>(
            () => usecase.SearchAsync(Guid.Empty));

        Assert.AreEqual(
            "顧客識別IDが不正です。",
            exception.Message);
        repository.Verify(
            x => x.FindByCustomerUuidAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [TestMethod(DisplayName = "空の注文UUIDでは購入履歴詳細を検索しない")]
    public async Task FindDetailAsync_WhenOrderUuidIsEmpty_ThrowsDomainException()
    {
        var repository = new Mock<IOrderRepository>();
        var usecase = new SearchPurchaseHistoryUsecase(
            repository.Object);

        var exception = await Assert.ThrowsExactlyAsync<DomainException>(
            () => usecase.FindDetailAsync(
                Guid.NewGuid(),
                Guid.Empty));

        Assert.AreEqual(
            "注文識別IDが不正です。",
            exception.Message);
        repository.Verify(
            x => x.FindByOrderUuidAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [TestMethod(DisplayName = "リポジトリ例外を呼び出し元へ伝える")]
    public async Task SearchAsync_WhenRepositoryFails_PropagatesException()
    {
        var customerUuid = Guid.NewGuid();
        var expected = new InternalException(
            "購入履歴一覧の取得中に予期しないエラーが発生しました。");
        var repository = new Mock<IOrderRepository>();
        repository
            .Setup(x => x.FindByCustomerUuidAsync(customerUuid))
            .ThrowsAsync(expected);
        var usecase = new SearchPurchaseHistoryUsecase(
            repository.Object);

        var actual = await Assert.ThrowsExactlyAsync<InternalException>(
            () => usecase.SearchAsync(customerUuid));

        Assert.AreSame(expected, actual);
    }

    private static Orders CreateOrder(
        Guid customerUuid)
    {
        var customer = new Customer(
            customerUuid,
            "顧客太郎",
            "コキャクタロウ",
            "東京都千代田区",
            null,
            "09012345678",
            "customer@example.com",
            "customer",
            "hashed-password",
            DateTime.Now.AddDays(-2));

        return new Orders(
            Guid.NewGuid(),
            DateTime.Now.AddDays(-1),
            100,
            customer,
            new OrderStatus(1, "受付済み"),
            new PaymentMethod(1, "クレジットカード"),
            []);
    }
}
