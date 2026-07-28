using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Tests.Models;

/// <summary>
/// Ordersクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class OrdersTests
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
    private Orders CreateOrders(
        Guid? orderUuid = null,
        DateTime? orderDate = null,
        int amountTotal = 1000,
        Customer? customer = null,
        OrderStatus? orderStatus = null,
        PaymentMethod? paymentMethod = null,
        List<OrdersDetail>? ordersDetails = null)
    {
        return new Orders(
            orderUuid ?? Guid.NewGuid(),
            orderDate ?? DateTime.Now.AddDays(-1),
            amountTotal,
            customer ?? CreateCustomer(),
            orderStatus ?? CreateOrderStatus(),
            paymentMethod ?? CreatePaymentMethod(),
            ordersDetails ?? new List<OrdersDetail>());
    }

    [TestMethod(DisplayName = "コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var orderUuid = Guid.NewGuid();
        var orderDate = DateTime.Now.AddDays(-1);
        var amountTotal = 1000;
        var customer = CreateCustomer();
        var orderStatus = CreateOrderStatus();
        var paymentMethod = CreatePaymentMethod();

        // インスタンスを生成する
        var orders = new Orders(
            orderUuid,
            orderDate,
            amountTotal,
            customer,
            orderStatus,
            paymentMethod,
            new List<OrdersDetail>());

        // 注文識別IDを検証する
        Assert.AreEqual(orderUuid, orders.OrderUuid);

        // 注文日を検証する
        Assert.AreEqual(orderDate, orders.OrderDate);

        // 合計金額を検証する
        Assert.AreEqual(amountTotal, orders.AmountTotal);

        // 顧客を検証する
        Assert.AreEqual(customer, orders.Customer);

        // 注文ステータスを検証する
        Assert.AreEqual(orderStatus, orders.OrderStatus);

        // 支払い方法を検証する
        Assert.AreEqual(paymentMethod, orders.PaymentMethod);
    }

    [TestMethod(DisplayName = "新規作成の場合UUIDが自動生成される")]
    public void NewInstance_ShouldGenerateUuidAutomatically()
    {
        // データを用意する
        var orderDate = DateTime.Now.AddDays(-1);
        var amountTotal = 1000;
        var customer = CreateCustomer();
        var orderStatus = CreateOrderStatus();
        var paymentMethod = CreatePaymentMethod();

        // インスタンスを生成する
        var orders = new Orders(
            orderDate,
            amountTotal,
            customer,
            orderStatus,
            paymentMethod,
            new List<OrdersDetail>());

        // 注文識別IDが自動生成されていることを検証する
        Assert.AreNotEqual(Guid.Empty, orders.OrderUuid);

        // 注文日を検証する
        Assert.AreEqual(orderDate, orders.OrderDate);

        // 合計金額を検証する
        Assert.AreEqual(amountTotal, orders.AmountTotal);

        // 顧客を検証する
        Assert.AreEqual(customer, orders.Customer);

        // 注文ステータスを検証する
        Assert.AreEqual(orderStatus, orders.OrderStatus);

        // 支払い方法を検証する
        Assert.AreEqual(paymentMethod, orders.PaymentMethod);
    }

    [TestMethod(DisplayName = "注文識別IDが空の場合、DomainExceptionがスローされる")]
    public void EmptyOrderUuid_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrders(orderUuid: Guid.Empty);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文識別IDが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "注文日が初期値の場合、DomainExceptionがスローされる")]
    public void DefaultOrderDate_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrders(orderDate: default(DateTime));
        });

        Assert.AreEqual("注文日が不正です", ex.Message);
    }

    [TestMethod(DisplayName = "注文日が未来日の場合、DomainExceptionがスローされる")]
    public void FutureOrderDate_ShouldThrowDomainException()
    {
        // データを用意する
        var orderDate = DateTime.Now.AddDays(1);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrders(orderDate: orderDate);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文日に未来日は指定できません", ex.Message);
    }

    [TestMethod(DisplayName = "注文日が過去日の場合、インスタンス生成される")]
    public void PastOrderDate_ShouldCreateInstance()
    {
        // データを用意する
        var orderDate = DateTime.Now.AddDays(-1);

        // インスタンスを生成する
        var orders = CreateOrders(orderDate: orderDate);

        // 注文日を検証する
        Assert.AreEqual(orderDate, orders.OrderDate);
    }

    [TestMethod(DisplayName = "合計金額が0円の場合、インスタンス生成される")]
    public void AmountTotal_WithZero_ShouldCreateInstance()
    {
        // データを用意する
        var amountTotal = 0;

        // インスタンスを生成する
        var orders = CreateOrders(amountTotal: amountTotal);

        // 合計金額を検証する
        Assert.AreEqual(amountTotal, orders.AmountTotal);
    }

    [TestMethod(DisplayName = "合計金額が正の値の場合、インスタンス生成される")]
    public void AmountTotal_WithPositiveValue_ShouldCreateInstance()
    {
        // データを用意する
        var amountTotal = 1000;

        // インスタンスを生成する
        var orders = CreateOrders(amountTotal: amountTotal);

        // 合計金額を検証する
        Assert.AreEqual(amountTotal, orders.AmountTotal);
    }

    [TestMethod(DisplayName = "合計金額が負数の場合、DomainExceptionがスローされる")]
    public void NegativeAmountTotal_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrders(amountTotal: -1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("合計金額は0以上で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "顧客がnullの場合、DomainExceptionがスローされる")]
    public void NullCustomer_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new Orders(
                Guid.NewGuid(),
                DateTime.Now.AddDays(-1),
                1000,
                null,
                CreateOrderStatus(),
                CreatePaymentMethod(),
                new List<OrdersDetail>());
        });

        // 例外メッセージを検証する
        Assert.AreEqual("顧客は必須です。", ex.Message);
    }

    [TestMethod(DisplayName = "注文ステータスがnullの場合、DomainExceptionがスローされる")]
    public void NullOrderStatus_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new Orders(
                Guid.NewGuid(),
                DateTime.Now.AddDays(-1),
                1000,
                CreateCustomer(),
                null,
                CreatePaymentMethod(),
                new List<OrdersDetail>());
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文ステータスは必須です。", ex.Message);
    }

    [TestMethod(DisplayName = "支払い方法がnullの場合、DomainExceptionがスローされる")]
    public void NullPaymentMethod_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new Orders(
                Guid.NewGuid(),
                DateTime.Now.AddDays(-1),
                1000,
                CreateCustomer(),
                CreateOrderStatus(),
                null,
                new List<OrdersDetail>());
        });

        // 例外メッセージを検証する
        Assert.AreEqual("支払い方法は必須です。", ex.Message);
    }

    [TestMethod(DisplayName = "同一インスタンスの場合、等価と判定される")]
    public void Equals_WithSameInstance_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var orders = CreateOrders();

        // 等価性を検証する
        var result = orders.Equals(orders);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "UUIDで等価と判定される")]
    public void Equals_WithSameUuid_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid();
        var orders1 = CreateOrders(orderUuid: uuid, amountTotal: 1000);
        var orders2 = CreateOrders(orderUuid: uuid, amountTotal: 2000);

        // 等価性を検証する
        var result = orders1.Equals(orders2);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "異なるUUIDで非等価と判定される")]
    public void Equals_WithDifferentUuid_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var orders1 = CreateOrders(amountTotal: 1000);
        var orders2 = CreateOrders(amountTotal: 2000);

        // 等価性を検証する
        var result = orders1.Equals(orders2);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "nullと比較した場合、非等価と判定される")]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var orders = CreateOrders();

        // 等価性を検証する
        var result = orders.Equals(null);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "異なる型と比較した場合、非等価と判定される")]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var orders = CreateOrders();

        // 等価性を検証する
        var result = orders.Equals("orders");

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "同じUUIDの場合、同じハッシュコードが返される")]
    public void GetHashCode_WithSameUuid_ShouldReturnSameHashCode()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid();
        var orders1 = CreateOrders(orderUuid: uuid, amountTotal: 1000);
        var orders2 = CreateOrders(orderUuid: uuid, amountTotal: 2000);

        // ハッシュコードを取得する
        var hashCode1 = orders1.GetHashCode();
        var hashCode2 = orders2.GetHashCode();

        // ハッシュコードを検証する
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [TestMethod(DisplayName = "ToStringで注文情報が文字列化される")]
    public void ToString_ShouldContainOrdersProperties()
    {
        // データを用意する
        var orderUuid = Guid.NewGuid();
        var orderDate = DateTime.Now.AddDays(-1);
        var amountTotal = 1000;
        var customer = CreateCustomer("山田太郎");
        var orderStatus = CreateOrderStatus("注文受付");
        var paymentMethod = CreatePaymentMethod("クレジットカード");

        // インスタンスを生成する
        var orders = CreateOrders(
            orderUuid: orderUuid,
            orderDate: orderDate,
            amountTotal: amountTotal,
            customer: customer,
            orderStatus: orderStatus,
            paymentMethod: paymentMethod);

        // 文字列化する
        var result = orders.ToString();

        // 文字列に注文情報が含まれることを検証する
        StringAssert.Contains(result, orderUuid.ToString());
        StringAssert.Contains(result, orderDate.ToString());
        StringAssert.Contains(result, amountTotal.ToString());
        StringAssert.Contains(result, "山田太郎");
        StringAssert.Contains(result, "注文受付");
        StringAssert.Contains(result, "クレジットカード");
    }

    [TestMethod(DisplayName = "注文明細がnullの場合、DomainExceptionがスローされる")]
    public void NullOrdersDetails_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new Orders(
                Guid.NewGuid(),
                DateTime.Now.AddDays(-1),
                1000,
                CreateCustomer(),
                CreateOrderStatus(),
                CreatePaymentMethod(),
                null!);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文明細は必須です。", ex.Message);
    }

    [TestMethod(DisplayName = "注文ステータスを変更できる")]
    public void ChangeOrderStatus_WithValidValue_ShouldChangeOrderStatus()
    {
        // インスタンスを生成する
        var orders = CreateOrders();

        // 変更後の注文ステータスを用意する
        var newOrderStatus = new OrderStatus(2, "発送済");

        // 注文ステータスを変更する
        orders.ChangeOrderStatus(newOrderStatus);

        // 注文ステータスを検証する
        Assert.AreEqual(newOrderStatus, orders.OrderStatus);
    }

    [TestMethod(DisplayName = "注文ステータス変更時にnullの場合、DomainExceptionがスローされる")]
    public void ChangeOrderStatus_WithNull_ShouldThrowDomainException()
    {
        // インスタンスを生成する
        var orders = CreateOrders();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            orders.ChangeOrderStatus(null!);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文ステータスは必須です。", ex.Message);
    }

    [TestMethod(DisplayName = "コンストラクタで注文明細リストが設定される")]
    public void Constructor_WithOrdersDetails_ShouldSetOrdersDetails()
    {
        // データを用意する
        var ordersDetails = new List<OrdersDetail>
    {
        CreateOrdersDetail(1, "商品A", 3)
    };

        // インスタンスを生成する
        var orders = CreateOrders(ordersDetails: ordersDetails);

        // 注文明細リストを検証する
        Assert.AreEqual(ordersDetails, orders.OrdersDetails);
    }

    [TestMethod(DisplayName = "注文明細を追加できる")]
    public void AddOrderDetail_WithValidValue_ShouldAddOrdersDetail()
    {
        // インスタンスを生成する
        var orders = CreateOrders();

        // 追加する注文明細を用意する
        var ordersDetail = CreateOrdersDetail(1, "商品A", 3);

        // 注文明細を追加する
        orders.AddOrderDetail(ordersDetail);

        // 注文明細が追加されていることを検証する
        Assert.HasCount(1, orders.OrdersDetails);
        Assert.AreEqual(ordersDetail, orders.OrdersDetails[0]);
    }

    [TestMethod(DisplayName = "注文明細追加時にnullの場合、DomainExceptionがスローされる")]
    public void AddOrderDetail_WithNull_ShouldThrowDomainException()
    {
        // インスタンスを生成する
        var orders = CreateOrders();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            orders.AddOrderDetail(null!);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文明細は必須です。", ex.Message);
    }

    /// <summary>
    /// ヘルパー：有効な商品を作成する
    /// </summary>
    private Product CreateProduct(string productName = "テスト商品")
    {
        var product = new Product(
            Guid.NewGuid(),
            productName,
            1000
        );

        product.ChangeCategory(
            new ProductCategory(Guid.NewGuid(), "食品")
        );

        product.ChangeStock(
            new ProductStock(Guid.NewGuid(), 10)
        );

        return product;
    }

    /// <summary>
    /// ヘルパー：有効な注文明細を作成する
    /// </summary>
    private OrdersDetail CreateOrdersDetail(
        int detailId = 1,
        string productName = "テスト商品",
        int count = 1)
    {
        return new OrdersDetail(
            detailId,
            CreateProduct(productName),
            count
        );
    }
}