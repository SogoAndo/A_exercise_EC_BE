using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Tests.Models;

/// <summary>
/// ProductStockクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class ProductStockTests
{
    /// <summary>
    /// ヘルパー：有効な商品在庫を作成する
    /// </summary>
    private ProductStock CreateProductStock(
        Guid? stockUuid = null,
        int quantity = 10)
    {
        return new ProductStock(
            stockUuid ?? Guid.NewGuid(),
            quantity);
    }

    [TestMethod(DisplayName = "コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var stockUuid = Guid.NewGuid();
        var quantity = 10;

        // インスタンスを生成する
        var productStock = new ProductStock(
            stockUuid,
            quantity);

        // 商品在庫識別IDを検証する
        Assert.AreEqual(stockUuid, productStock.StockUuid);

        // 商品在庫数を検証する
        Assert.AreEqual(quantity, productStock.Quantity);
    }

    [TestMethod(DisplayName = "新規作成の場合UUIDが自動生成される")]
    public void NewInstance_ShouldGenerateUuidAutomatically()
    {
        // データを用意する
        var quantity = 10;

        // インスタンスを生成する
        var productStock = new ProductStock(quantity);

        // 商品在庫識別IDが自動生成されていることを検証する
        Assert.AreNotEqual(Guid.Empty, productStock.StockUuid);

        // 商品在庫数を検証する
        Assert.AreEqual(quantity, productStock.Quantity);
    }

    [TestMethod(DisplayName = "商品在庫識別IDが空の場合、DomainExceptionがスローされる")]
    public void EmptyStockUuid_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProductStock(stockUuid: Guid.Empty);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品在庫識別IDが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "在庫数が0の場合、インスタンス生成される")]
    public void Quantity_WithZero_ShouldCreateInstance()
    {
        // データを用意する
        var quantity = 0;

        // インスタンスを生成する
        var productStock = CreateProductStock(quantity: quantity);

        // 商品在庫数を検証する
        Assert.AreEqual(quantity, productStock.Quantity);
    }

    [TestMethod(DisplayName = "在庫数が正の値の場合、インスタンス生成される")]
    public void Quantity_WithPositiveValue_ShouldCreateInstance()
    {
        // データを用意する
        var quantity = 10;

        // インスタンスを生成する
        var productStock = CreateProductStock(quantity: quantity);

        // 商品在庫数を検証する
        Assert.AreEqual(quantity, productStock.Quantity);
    }

    [TestMethod(DisplayName = "在庫数が負数の場合、DomainExceptionがスローされる")]
    public void Quantity_WithNegativeValue_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProductStock(quantity: -1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("在庫数は0以上で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "新規作成で在庫数が0の場合、インスタンス生成される")]
    public void NewInstance_QuantityWithZero_ShouldCreateInstance()
    {
        // データを用意する
        var quantity = 0;

        // インスタンスを生成する
        var productStock = new ProductStock(quantity);

        // 商品在庫数を検証する
        Assert.AreEqual(quantity, productStock.Quantity);
    }

    [TestMethod(DisplayName = "新規作成で在庫数が負数の場合、DomainExceptionがスローされる")]
    public void NewInstance_QuantityWithNegativeValue_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new ProductStock(-1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("在庫数は0以上で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "同一インスタンスの場合、等価と判定される")]
    public void Equals_WithSameInstance_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var productStock = CreateProductStock();

        // 等価性を検証する
        var result = productStock.Equals(productStock);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "UUIDで等価と判定される")]
    public void Equals_WithSameUuid_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid();
        var productStock1 = CreateProductStock(stockUuid: uuid, quantity: 10);
        var productStock2 = CreateProductStock(stockUuid: uuid, quantity: 20);

        // 等価性を検証する
        var result = productStock1.Equals(productStock2);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "異なるUUIDで非等価と判定される")]
    public void Equals_WithDifferentUuid_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var productStock1 = CreateProductStock(quantity: 10);
        var productStock2 = CreateProductStock(quantity: 20);

        // 等価性を検証する
        var result = productStock1.Equals(productStock2);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "nullと比較した場合、非等価と判定される")]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var productStock = CreateProductStock();

        // 等価性を検証する
        var result = productStock.Equals(null);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "異なる型と比較した場合、非等価と判定される")]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var productStock = CreateProductStock();

        // 等価性を検証する
        var result = productStock.Equals("productStock");

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "同じUUIDの場合、同じハッシュコードが返される")]
    public void GetHashCode_WithSameUuid_ShouldReturnSameHashCode()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid();
        var productStock1 = CreateProductStock(stockUuid: uuid, quantity: 10);
        var productStock2 = CreateProductStock(stockUuid: uuid, quantity: 20);

        // ハッシュコードを取得する
        var hashCode1 = productStock1.GetHashCode();
        var hashCode2 = productStock2.GetHashCode();

        // ハッシュコードを検証する
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [TestMethod(DisplayName = "ToStringで商品在庫情報が文字列化される")]
    public void ToString_ShouldContainProductStockProperties()
    {
        // データを用意する
        var stockUuid = Guid.NewGuid();
        var quantity = 10;

        // インスタンスを生成する
        var productStock = CreateProductStock(
            stockUuid: stockUuid,
            quantity: quantity);

        // 文字列化する
        var result = productStock.ToString();

        // 文字列に商品在庫情報が含まれることを検証する
        StringAssert.Contains(result, stockUuid.ToString());
        StringAssert.Contains(result, quantity.ToString());
    }
}