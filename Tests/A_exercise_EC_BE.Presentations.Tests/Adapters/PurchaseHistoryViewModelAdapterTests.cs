using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Presentations.Adapters;

namespace A_exercise_EC_BE.Presentations.Tests.Adapters;

[TestClass]
[TestCategory("Presentations/Adapters")]
public class PurchaseHistoryViewModelAdapterTests
{
    private readonly PurchaseHistoryViewModelAdapter adapter = new();

    [TestMethod(DisplayName = "購入履歴一覧がnullの場合は内部例外にする")]
    public void ConvertToResultViewModel_WhenOrdersIsNull_ThrowsInternalException()
    {
        var exception = Assert.ThrowsExactly<InternalException>(
            () => adapter.ConvertToResultViewModel(null!));

        Assert.AreEqual(
            "引数ordersがnullです。",
            exception.Message);
    }

    [TestMethod(DisplayName = "購入履歴一覧にnullの注文がある場合は内部例外にする")]
    public void ConvertToResultViewModel_WhenOrderIsNull_ThrowsInternalException()
    {
        var exception = Assert.ThrowsExactly<InternalException>(
            () => adapter.ConvertToResultViewModel([null!]));

        Assert.AreEqual(
            "注文情報がnullです。",
            exception.Message);
    }

    [TestMethod(DisplayName = "購入履歴詳細がnullの場合は内部例外にする")]
    public void ConvertToDetailViewModel_WhenOrderIsNull_ThrowsInternalException()
    {
        var exception = Assert.ThrowsExactly<InternalException>(
            () => adapter.ConvertToDetailViewModel(null!));

        Assert.AreEqual(
            "引数orderがnullです。",
            exception.Message);
    }

    [TestMethod(DisplayName = "注文明細がnullの場合は内部例外にする")]
    public void ConvertToDetailViewModel_WhenOrderDetailIsNull_ThrowsInternalException()
    {
        var order = CreateOrder([null!]);

        var exception = Assert.ThrowsExactly<InternalException>(
            () => adapter.ConvertToDetailViewModel(order));

        Assert.AreEqual(
            "注文明細がnullです。",
            exception.Message);
    }

    [TestMethod(DisplayName = "注文明細の小計がint上限を超える場合は内部例外にする")]
    public void ConvertToDetailViewModel_WhenSubtotalOverflows_ThrowsInternalException()
    {
        var product = new Product(
            Guid.NewGuid(),
            "高額商品",
            1_000_000);
        var order = CreateOrder(
            [
                new OrdersDetail(
                    1,
                    product,
                    int.MaxValue)
            ]);

        var exception = Assert.ThrowsExactly<InternalException>(
            () => adapter.ConvertToDetailViewModel(order));

        Assert.AreEqual(
            "注文明細の小計を計算できません。",
            exception.Message);
        Assert.IsInstanceOfType<OverflowException>(
            exception.InnerException);
    }

    private static Orders CreateOrder(
        List<OrdersDetail> orderDetails)
    {
        var customer = new Customer(
            Guid.NewGuid(),
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
            orderDetails);
    }
}
