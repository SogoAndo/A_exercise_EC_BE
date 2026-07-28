using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Tests.Models;

/// <summary>
/// OrderStatusクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class OrderStatusTests
{
    /// <summary>
    /// ヘルパー：有効な注文ステータスを作成する
    /// </summary>
    private OrderStatus CreateOrderStatus(
        int statusId = 1,
        string statusName = "注文受付")
    {
        return new OrderStatus(statusId, statusName);
    }

    [TestMethod(DisplayName = "コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var statusId = 1;
        var statusName = "注文受付";

        // インスタンスを生成する
        var orderStatus = new OrderStatus(statusId, statusName);

        // 注文ステータスIDを検証する
        Assert.AreEqual(statusId, orderStatus.Id);

        // 注文ステータス名を検証する
        Assert.AreEqual(statusName, orderStatus.Name);
    }

    [TestMethod(DisplayName = "ID未定の注文ステータスを作成するとインスタンス生成される")]
    public void Constructor_WithoutId_ShouldCreateInstance()
    {
        // データを用意する
        var statusName = "注文受付";

        // インスタンスを生成する
        var orderStatus = new OrderStatus(statusName);

        // 注文ステータスIDが初期値であることを検証する
        Assert.AreEqual(0, orderStatus.Id);

        // 注文ステータス名を検証する
        Assert.AreEqual(statusName, orderStatus.Name);
    }

    [TestMethod(DisplayName = "注文ステータスIDが1の場合、インスタンス生成される")]
    public void StatusId_WithOne_ShouldCreateInstance()
    {
        // データを用意する
        var statusId = 1;

        // インスタンスを生成する
        var orderStatus = CreateOrderStatus(statusId: statusId);

        // 注文ステータスIDを検証する
        Assert.AreEqual(statusId, orderStatus.Id);
    }

    [TestMethod(DisplayName = "注文ステータスIDが0の場合、DomainExceptionがスローされる")]
    public void StatusId_WithZero_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrderStatus(statusId: 0);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文ステータスIDが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "注文ステータスIDが負数の場合、DomainExceptionがスローされる")]
    public void StatusId_WithNegativeValue_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrderStatus(statusId: -1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文ステータスIDが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "注文ステータス名がnullの場合、DomainExceptionがスローされる")]
    public void NullStatusName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrderStatus(statusName: null!);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文ステータス名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "注文ステータス名が空白の場合、DomainExceptionがスローされる")]
    public void EmptyStatusName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrderStatus(statusName: "");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文ステータス名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "注文ステータス名が空白文字のみの場合、DomainExceptionがスローされる")]
    public void WhiteSpaceStatusName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrderStatus(statusName: "   ");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文ステータス名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "注文ステータス名が100文字の場合、インスタンス生成される")]
    public void StatusName_With100Chars_ShouldCreateInstance()
    {
        // データを用意する
        var statusName = new string('あ', 100);

        // インスタンスを生成する
        var orderStatus = CreateOrderStatus(statusName: statusName);

        // 注文ステータス名を検証する
        Assert.AreEqual(statusName, orderStatus.Name);
    }

    [TestMethod(DisplayName = "注文ステータス名が101文字以上の場合、DomainExceptionがスローされる")]
    public void StatusName_LongerThan100Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var statusName = new string('あ', 101);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateOrderStatus(statusName: statusName);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文ステータス名は100文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "ID未定の注文ステータスで注文ステータス名が空白の場合、DomainExceptionがスローされる")]
    public void Constructor_WithoutId_EmptyStatusName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new OrderStatus("");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文ステータス名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "ID未定の注文ステータスで注文ステータス名が101文字以上の場合、DomainExceptionがスローされる")]
    public void Constructor_WithoutId_StatusNameLongerThan100Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var statusName = new string('あ', 101);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new OrderStatus(statusName);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("注文ステータス名は100文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "同一インスタンスの場合、等価と判定される")]
    public void Equals_WithSameInstance_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var orderStatus = CreateOrderStatus();

        // 等価性を検証する
        var result = orderStatus.Equals(orderStatus);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "同じIDで等価と判定される")]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var orderStatus1 = CreateOrderStatus(statusId: 1, statusName: "注文受付");
        var orderStatus2 = CreateOrderStatus(statusId: 1, statusName: "発送済み");

        // 等価性を検証する
        var result = orderStatus1.Equals(orderStatus2);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "異なるIDで非等価と判定される")]
    public void Equals_WithDifferentId_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var orderStatus1 = CreateOrderStatus(statusId: 1, statusName: "注文受付");
        var orderStatus2 = CreateOrderStatus(statusId: 2, statusName: "発送済み");

        // 等価性を検証する
        var result = orderStatus1.Equals(orderStatus2);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "nullと比較した場合、非等価と判定される")]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var orderStatus = CreateOrderStatus();

        // 等価性を検証する
        var result = orderStatus.Equals(null);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "異なる型と比較した場合、非等価と判定される")]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var orderStatus = CreateOrderStatus();

        // 等価性を検証する
        var result = orderStatus.Equals("orderStatus");

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "ID未定の注文ステータス同士は等価と判定される")]
    public void Equals_WithBothUnsetId_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var orderStatus1 = new OrderStatus("注文受付");
        var orderStatus2 = new OrderStatus("発送済み");

        // 等価性を検証する
        var result = orderStatus1.Equals(orderStatus2);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "同じIDの場合、同じハッシュコードが返される")]
    public void GetHashCode_WithSameId_ShouldReturnSameHashCode()
    {
        // インスタンスを生成する
        var orderStatus1 = CreateOrderStatus(statusId: 1, statusName: "注文受付");
        var orderStatus2 = CreateOrderStatus(statusId: 1, statusName: "発送済み");

        // ハッシュコードを取得する
        var hashCode1 = orderStatus1.GetHashCode();
        var hashCode2 = orderStatus2.GetHashCode();

        // ハッシュコードを検証する
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [TestMethod(DisplayName = "ToStringで注文ステータス情報が文字列化される")]
    public void ToString_ShouldContainOrderStatusProperties()
    {
        // データを用意する
        var statusId = 1;
        var statusName = "注文受付";

        // インスタンスを生成する
        var orderStatus = CreateOrderStatus(
            statusId: statusId,
            statusName: statusName);

        // 文字列化する
        var result = orderStatus.ToString();

        // 文字列に注文ステータス情報が含まれることを検証する
        StringAssert.Contains(result, statusId.ToString());
        StringAssert.Contains(result, statusName);
    }
}