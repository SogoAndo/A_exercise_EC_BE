using A_exercise_EC_BE.Applications.Usecases.Purchases;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Applications.Tests.Usecases.Purchases;

[TestClass]
[TestCategory("Applications/Usecases/Purchases")]
public class PurchaseAmountCalculatorTests
{
    private readonly PurchaseAmountCalculator _calculator =
        new();

    [TestMethod]
    public void Calculate_WithMultipleItems_ReturnsTotalAmount()
    {
        OrdersDetail[] orderDetails =
        [
            CreateOrderDetail(
                productName: "商品A",
                price: 1_000,
                quantity: 2),
            CreateOrderDetail(
                productName: "商品B",
                price: 500,
                quantity: 3)
        ];

        var result = _calculator.Calculate(
            orderDetails);

        Assert.AreEqual(
            3_500,
            result);
    }

    [TestMethod]
    public void Calculate_WithNullCart_ThrowsDomainException()
    {
        var exception = Assert.ThrowsExactly<
            DomainException>(
            () => _calculator.Calculate(null));

        Assert.AreEqual(
            "カートに商品がありません",
            exception.Message);
    }

    [TestMethod]
    public void Calculate_WithEmptyCart_ThrowsDomainException()
    {
        var exception = Assert.ThrowsExactly<
            DomainException>(
            () => _calculator.Calculate([]));

        Assert.AreEqual(
            "カートに商品がありません",
            exception.Message);
    }

    [TestMethod]
    public void Calculate_WithZeroQuantity_ThrowsDomainException()
    {
        OrdersDetail[] orderDetails =
        [
            CreateOrderDetail(
                productName: "商品A",
                price: 1_000,
                quantity: 0)
        ];

        var exception = Assert.ThrowsExactly<
            DomainException>(
            () => _calculator.Calculate(
                orderDetails));

        Assert.AreEqual(
            "購入数量は1以上で入力してください。",
            exception.Message);
    }

    [TestMethod]
    public void Calculate_WhenAmountOverflows_ThrowsDomainException()
    {
        OrdersDetail[] orderDetails =
        [
            CreateOrderDetail(
                productName: "商品A",
                price: 1_000_000,
                quantity: int.MaxValue)
        ];

        var exception = Assert.ThrowsExactly<
            DomainException>(
            () => _calculator.Calculate(
                orderDetails));

        Assert.AreEqual(
            "合計金額が計算可能な範囲を超えています。",
            exception.Message);
        Assert.IsInstanceOfType<
            OverflowException>(
            exception.InnerException);
    }

    private static OrdersDetail CreateOrderDetail(
        string productName,
        int price,
        int quantity)
        => new(
            new Product(
                Guid.NewGuid(),
                productName,
                price),
            quantity);
}
