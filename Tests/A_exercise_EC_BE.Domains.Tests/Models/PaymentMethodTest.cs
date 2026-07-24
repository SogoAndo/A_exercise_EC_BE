using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Tests.Models;

/// <summary>
/// PaymentMethodクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class PaymentMethodTests
{
    /// <summary>
    /// ヘルパー：有効な支払い方法を作成する
    /// </summary>
    private PaymentMethod CreatePaymentMethod(
        int paymentId = 1,
        string paymentName = "クレジットカード")
    {
        return new PaymentMethod(paymentId, paymentName);
    }

    [TestMethod(DisplayName = "コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var paymentId = 1;
        var paymentName = "クレジットカード";

        // インスタンスを生成する
        var paymentMethod = new PaymentMethod(paymentId, paymentName);

        // 支払い方法IDを検証する
        Assert.AreEqual(paymentId, paymentMethod.Id);

        // 支払い方法名を検証する
        Assert.AreEqual(paymentName, paymentMethod.Name);
    }

    [TestMethod(DisplayName = "ID未定の支払い方法を作成するとインスタンス生成される")]
    public void Constructor_WithoutId_ShouldCreateInstance()
    {
        // データを用意する
        var paymentName = "クレジットカード";

        // インスタンスを生成する
        var paymentMethod = new PaymentMethod(paymentName);

        // 支払い方法IDが初期値であることを検証する
        Assert.AreEqual(0, paymentMethod.Id);

        // 支払い方法名を検証する
        Assert.AreEqual(paymentName, paymentMethod.Name);
    }

    [TestMethod(DisplayName = "支払い方法IDが1の場合、インスタンス生成される")]
    public void PaymentId_WithOne_ShouldCreateInstance()
    {
        // データを用意する
        var paymentId = 1;

        // インスタンスを生成する
        var paymentMethod = CreatePaymentMethod(paymentId: paymentId);

        // 支払い方法IDを検証する
        Assert.AreEqual(paymentId, paymentMethod.Id);
    }

    [TestMethod(DisplayName = "支払い方法IDが0の場合、DomainExceptionがスローされる")]
    public void PaymentId_WithZero_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreatePaymentMethod(paymentId: 0);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("支払い方法IDが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "支払い方法IDが負数の場合、DomainExceptionがスローされる")]
    public void PaymentId_WithNegativeValue_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreatePaymentMethod(paymentId: -1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("支払い方法IDが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "支払い方法名がnullの場合、DomainExceptionがスローされる")]
    public void NullPaymentName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreatePaymentMethod(paymentName: null!);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("支払い方法名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "支払い方法名が空白の場合、DomainExceptionがスローされる")]
    public void EmptyPaymentName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreatePaymentMethod(paymentName: "");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("支払い方法名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "支払い方法名が空白文字のみの場合、DomainExceptionがスローされる")]
    public void WhiteSpacePaymentName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreatePaymentMethod(paymentName: "   ");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("支払い方法名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "支払い方法名が100文字の場合、インスタンス生成される")]
    public void PaymentName_With100Chars_ShouldCreateInstance()
    {
        // データを用意する
        var paymentName = new string('あ', 100);

        // インスタンスを生成する
        var paymentMethod = CreatePaymentMethod(paymentName: paymentName);

        // 支払い方法名を検証する
        Assert.AreEqual(paymentName, paymentMethod.Name);
    }

    [TestMethod(DisplayName = "支払い方法名が101文字以上の場合、DomainExceptionがスローされる")]
    public void PaymentName_LongerThan100Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var paymentName = new string('あ', 101);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreatePaymentMethod(paymentName: paymentName);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("支払い方法名は100文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "ID未定の支払い方法で支払い方法名が空白の場合、DomainExceptionがスローされる")]
    public void Constructor_WithoutId_EmptyPaymentName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new PaymentMethod("");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("支払い方法名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "ID未定の支払い方法で支払い方法名が101文字以上の場合、DomainExceptionがスローされる")]
    public void Constructor_WithoutId_PaymentNameLongerThan100Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var paymentName = new string('あ', 101);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new PaymentMethod(paymentName);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("支払い方法名は100文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "同一インスタンスの場合、等価と判定される")]
    public void Equals_WithSameInstance_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var paymentMethod = CreatePaymentMethod();

        // 等価性を検証する
        var result = paymentMethod.Equals(paymentMethod);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "同じIDで等価と判定される")]
    public void Equals_WithSameId_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var paymentMethod1 = CreatePaymentMethod(paymentId: 1, paymentName: "クレジットカード");
        var paymentMethod2 = CreatePaymentMethod(paymentId: 1, paymentName: "銀行振込");

        // 等価性を検証する
        var result = paymentMethod1.Equals(paymentMethod2);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "異なるIDで非等価と判定される")]
    public void Equals_WithDifferentId_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var paymentMethod1 = CreatePaymentMethod(paymentId: 1, paymentName: "クレジットカード");
        var paymentMethod2 = CreatePaymentMethod(paymentId: 2, paymentName: "銀行振込");

        // 等価性を検証する
        var result = paymentMethod1.Equals(paymentMethod2);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "nullと比較した場合、非等価と判定される")]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var paymentMethod = CreatePaymentMethod();

        // 等価性を検証する
        var result = paymentMethod.Equals(null);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "異なる型と比較した場合、非等価と判定される")]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var paymentMethod = CreatePaymentMethod();

        // 等価性を検証する
        var result = paymentMethod.Equals("paymentMethod");

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "ID未定の支払い方法同士は等価と判定される")]
    public void Equals_WithBothUnsetId_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var paymentMethod1 = new PaymentMethod("クレジットカード");
        var paymentMethod2 = new PaymentMethod("銀行振込");

        // 等価性を検証する
        var result = paymentMethod1.Equals(paymentMethod2);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "同じIDの場合、同じハッシュコードが返される")]
    public void GetHashCode_WithSameId_ShouldReturnSameHashCode()
    {
        // インスタンスを生成する
        var paymentMethod1 = CreatePaymentMethod(paymentId: 1, paymentName: "クレジットカード");
        var paymentMethod2 = CreatePaymentMethod(paymentId: 1, paymentName: "銀行振込");

        // ハッシュコードを取得する
        var hashCode1 = paymentMethod1.GetHashCode();
        var hashCode2 = paymentMethod2.GetHashCode();

        // ハッシュコードを検証する
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [TestMethod(DisplayName = "ToStringで支払い方法情報が文字列化される")]
    public void ToString_ShouldContainPaymentMethodProperties()
    {
        // データを用意する
        var paymentId = 1;
        var paymentName = "クレジットカード";

        // インスタンスを生成する
        var paymentMethod = CreatePaymentMethod(
            paymentId: paymentId,
            paymentName: paymentName);

        // 文字列化する
        var result = paymentMethod.ToString();

        // 文字列に支払い方法情報が含まれることを検証する
        StringAssert.Contains(result, paymentId.ToString());
        StringAssert.Contains(result, paymentName);
    }
}