using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Tests.Models;

/// <summary>
/// OrdersDetailクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class OrdersDetailTests
{
    /// <summary>
    /// ヘルパー：有効な顧客を作成する
    /// </summary>
    private Customer CreateCustomer(string customerName = "山田太郎")
    {
        return new Customer(
            Guid.NewGuid(),
            customerName,
            "ヤマダタロウ",
            "東京都千代田区",
            "1-1-1",
            "09012345678",
            "test@example.com",
            "yamada",
            "password123",
            DateTime.Now.AddDays(-1));
    }

    /// <summary>
    /// ヘルパー：有効な注文ステータスを作成する
    /// </summary>
    private OrderStatus CreateOrderStatus(string statusName = "注文受付")
    {
        return new OrderStatus(1, statusName);
    }

    /// <summary>
    /// ヘルパー：有効な支払い方法を作成する
    /// </summary>
    private PaymentMethod CreatePaymentMethod(string paymentName = "クレジットカード")
    {
        return new PaymentMethod(1, paymentName);
    }

    /// <summary>
    /// ヘルパー：有効な注文を作成する
    /// </summary>
    private Orders CreateOrders(int amountTotal = 1000)
    {
        return new Orders(
            Guid.NewGuid(),
            DateTime.Now.AddDays(-1),
            amountTotal,
            CreateCustomer(),
            CreateOrderStatus(),
            CreatePaymentMethod(),
            new List<OrdersDetail>());
    }

    /// <summary>
    /// ヘルパー：有効な商品カテゴリを作成する
    /// </summary>
    private ProductCategory CreateProductCategory(string categoryName = "食品")
    {
        return new ProductCategory(categoryName);
    }

    /// <summary>
    /// ヘルパー：有効な商品在庫を作成する
    /// </summary>
    private ProductStock CreateProductStock(int quantity = 10)
    {
        return new ProductStock(quantity);
    }

    /// <summary>
    /// ヘルパー：有効な商品を作成する
    /// </summary>
    private Product CreateProduct(string productName = "商品A", int productPrice = 1000)
    {
        return new Product(
            Guid.NewGuid(),
            productName,
            productPrice,
            "https://example.com/image.png",
            CreateProductCategory(),
            CreateProductStock(),
            0);
    }

    /// <summary>
    /// ヘルパー：有効な注文明細を作成する
    /// </summary>
    private OrdersDetail CreateOrdersDetail(
        int detailId = 1,
        Product? product = null,
        int count = 1)
    {
        return new OrdersDetail(
            detailId,
            product ?? CreateProduct(),
            count);
    }

    [TestMethod(DisplayName = "コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var detailId = 1;
        var product = CreateProduct();
        var count = 3;

        // インスタンスを生成する
        var ordersDetail = new OrdersDetail(
            detailId,
            product,
            count);

        // 注文明細IDを検証する
        Assert.AreEqual(detailId, ordersDetail.Id);

        // 商品を検証する
        Assert.AreEqual(product, ordersDetail.Product);

        // 数量を検証する
        Assert.AreEqual(count, ordersDetail.Count);
    }

    [TestMethod(DisplayName = "注文明細IDが1の場合、インスタンス生成される")]
    public void DetailId_WithOne_ShouldCreateInstance()
    {
        // データを用意する
        var detailId = 1;

        // インスタンスを生成する
        var ordersDetail = CreateOrdersDetail(detailId: detailId);

        // 注文明細IDを検証する
        Assert.AreEqual(detailId, ordersDetail.Id);
    }

    [TestMethod(DisplayName = "注文明細IDが0の場合、DomainExceptionがスローされる")]
    public void DetailId_WithZero_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrdersDetail(detailId: 0);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文明細IDが不正です。", ex.Message);
    }

    [TestMethod(DisplayName = "注文明細IDが負数の場合、DomainExceptionがスローされる")]
    public void DetailId_WithNegativeValue_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrdersDetail(detailId: -1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文明細IDが不正です。", ex.Message);
    }

    [TestMethod(DisplayName = "商品がnullの場合、DomainExceptionがスローされる")]
    public void NullProduct_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new OrdersDetail(
                1,
                null!,
                1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品は必須です。", ex.Message);
    }

    [TestMethod(DisplayName = "数量が0の場合、インスタンス生成される")]
    public void Count_WithZero_ShouldCreateInstance()
    {
        // データを用意する
        var count = 0;

        // インスタンスを生成する
        var ordersDetail = CreateOrdersDetail(count: count);

        // 数量を検証する
        Assert.AreEqual(count, ordersDetail.Count);
    }

    [TestMethod(DisplayName = "数量が正の値の場合、インスタンス生成される")]
    public void Count_WithPositiveValue_ShouldCreateInstance()
    {
        // データを用意する
        var count = 3;

        // インスタンスを生成する
        var ordersDetail = CreateOrdersDetail(count: count);

        // 数量を検証する
        Assert.AreEqual(count, ordersDetail.Count);
    }

    [TestMethod(DisplayName = "数量が負数の場合、DomainExceptionがスローされる")]
    public void Count_WithNegativeValue_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrdersDetail(count: -1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("合計金額は0以上で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "ID未定の注文明細を作成すると商品・数量が設定される")]
    public void Constructor_WithoutId_ShouldSetProductAndCount()
    {
        // データを用意する
        var product = CreateProduct();
        var count = 3;

        // インスタンスを生成する
        var ordersDetail = new OrdersDetail(
            product,
            count);

        // 商品を検証する
        Assert.AreEqual(product, ordersDetail.Product);

        // 数量を検証する
        Assert.AreEqual(count, ordersDetail.Count);
    }

    [TestMethod(DisplayName = "ID未定の注文明細で商品がnullの場合、DomainExceptionがスローされる")]
    public void Constructor_WithoutId_NullProduct_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new OrdersDetail(
                null!,
                1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品は必須です。", ex.Message);
    }

    [TestMethod(DisplayName = "ID未定の注文明細で数量が負数の場合、DomainExceptionがスローされる")]
    public void Constructor_WithoutId_NegativeCount_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new OrdersDetail(
                CreateProduct(),
                -1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("合計金額は0以上で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "同一インスタンスの場合、等価と判定される")]
    public void Equals_WithSameInstance_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var ordersDetail = CreateOrdersDetail();

        // 等価性を検証する
        var result = ordersDetail.Equals(ordersDetail);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "同じIDで等価と判定される")]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var ordersDetail1 = CreateOrdersDetail(detailId: 1, count: 1);
        var ordersDetail2 = CreateOrdersDetail(detailId: 1, count: 2);

        // 等価性を検証する
        var result = ordersDetail1.Equals(ordersDetail2);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "異なるIDで非等価と判定される")]
    public void Equals_WithDifferentId_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var ordersDetail1 = CreateOrdersDetail(detailId: 1);
        var ordersDetail2 = CreateOrdersDetail(detailId: 2);

        // 等価性を検証する
        var result = ordersDetail1.Equals(ordersDetail2);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "nullと比較した場合、非等価と判定される")]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var ordersDetail = CreateOrdersDetail();

        // 等価性を検証する
        var result = ordersDetail.Equals(null);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "異なる型と比較した場合、非等価と判定される")]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var ordersDetail = CreateOrdersDetail();

        // 等価性を検証する
        var result = ordersDetail.Equals("ordersDetail");

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "ToStringで注文明細情報が文字列化される")]
    public void ToString_ShouldContainOrdersDetailProperties()
    {
        // データを用意する
        var detailId = 1;
        var product = CreateProduct("商品A");
        var count = 3;

        // インスタンスを生成する
        var ordersDetail = new OrdersDetail(
            detailId,
            product,
            count);

        // 文字列化する
        var result = ordersDetail.ToString();

        // 文字列に注文明細情報が含まれることを検証する
        StringAssert.Contains(result, detailId.ToString());
        StringAssert.Contains(result, count.ToString());
        StringAssert.Contains(result, "商品A");
    }

    [TestMethod(DisplayName = "GetHashCodeは注文明細IDのハッシュコードを返す")]
    public void GetHashCode_WithValidId_ShouldReturnIdHashCode()
    {
        // Arrange
        const int detailId = 1;
        var ordersDetail = CreateOrdersDetail(detailId: detailId);

        // Act
        var result = ordersDetail.GetHashCode();

        // Assert
        Assert.AreEqual(detailId.GetHashCode(), result);
    }
}