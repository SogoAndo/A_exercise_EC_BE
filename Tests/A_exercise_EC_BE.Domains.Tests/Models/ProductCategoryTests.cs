using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Tests.Models;

/// <summary>
/// ProductCategoryクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class ProductCategoryTests
{
    /// <summary>
    /// ヘルパー：有効な商品カテゴリを作成する
    /// </summary>
    private ProductCategory CreateProductCategory(
        Guid? categoryUuid = null,
        string categoryName = "食品")
    {
        return new ProductCategory(
            categoryUuid ?? Guid.NewGuid(),
            categoryName);
    }

    [TestMethod(DisplayName = "コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var categoryUuid = Guid.NewGuid();
        var categoryName = "食品";

        // インスタンスを生成する
        var productCategory = new ProductCategory(
            categoryUuid,
            categoryName);

        // 商品カテゴリ識別IDを検証する
        Assert.AreEqual(categoryUuid, productCategory.CategoryUuid);

        // 商品カテゴリ名を検証する
        Assert.AreEqual(categoryName, productCategory.Name);
    }

    [TestMethod(DisplayName = "新規作成の場合UUIDが自動生成される")]
    public void NewInstance_ShouldGenerateUuidAutomatically()
    {
        // データを用意する
        var categoryName = "食品";

        // インスタンスを生成する
        var productCategory = new ProductCategory(categoryName);

        // 商品カテゴリ識別IDが自動生成されていることを検証する
        Assert.AreNotEqual(Guid.Empty, productCategory.CategoryUuid);

        // 商品カテゴリ名を検証する
        Assert.AreEqual(categoryName, productCategory.Name);
    }

    [TestMethod(DisplayName = "商品カテゴリ識別IDが空の場合、DomainExceptionがスローされる")]
    public void EmptyCategoryUuid_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProductCategory(categoryUuid: Guid.Empty);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品カテゴリ識別IDが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "商品カテゴリ名がnullの場合、DomainExceptionがスローされる")]
    public void NullCategoryName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProductCategory(categoryName: null!);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品カテゴリ名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "商品カテゴリ名が空白の場合、DomainExceptionがスローされる")]
    public void EmptyCategoryName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProductCategory(categoryName: "");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品カテゴリ名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "商品カテゴリ名が空白文字のみの場合、DomainExceptionがスローされる")]
    public void WhiteSpaceCategoryName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProductCategory(categoryName: "   ");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品カテゴリ名は必須です", ex.Message);
    }

    [TestMethod(
        DisplayName =
            "商品カテゴリ名が30文字の場合、インスタンス生成される")]
    public void CategoryName_With30Chars_ShouldCreateInstance()
    {
        // Arrange
        var categoryName = new string('あ', 30);

        // Act
        var productCategory =
            CreateProductCategory(
                categoryName: categoryName);

        // Assert
        Assert.AreEqual(
            categoryName,
            productCategory.Name);
    }

    [TestMethod(
     DisplayName =
         "商品カテゴリ名が31文字の場合、DomainExceptionがスローされる")]
    public void
     CategoryName_With31Chars_ShouldThrowDomainException()
    {
        // Arrange
        var categoryName = new string('あ', 31);

        // Act
        var exception =
            Assert.ThrowsExactly<DomainException>(
                () =>
                {
                    _ = CreateProductCategory(
                        categoryName: categoryName);
                });

        // Assert
        Assert.AreEqual(
            "商品カテゴリ名は30文字以内で入力してください",
            exception.Message);
    }

    [TestMethod(DisplayName = "新規作成で商品カテゴリ名が空白の場合、DomainExceptionがスローされる")]
    public void NewInstance_EmptyCategoryName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new ProductCategory("");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品カテゴリ名は必須です", ex.Message);
    }

    [TestMethod(
     DisplayName =
         "新規作成で商品カテゴリ名が31文字の場合、DomainExceptionがスローされる")]
    public void
     NewInstance_CategoryNameWith31Chars_ShouldThrowDomainException()
    {
        // Arrange
        var categoryName = new string('あ', 31);

        // Act
        var exception =
            Assert.ThrowsExactly<DomainException>(
                () =>
                {
                    _ = new ProductCategory(
                        categoryName);
                });

        // Assert
        Assert.AreEqual(
            "商品カテゴリ名は30文字以内で入力してください",
            exception.Message);
    }

    [TestMethod(DisplayName = "同一インスタンスの場合、等価と判定される")]
    public void Equals_WithSameInstance_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var productCategory = CreateProductCategory();

        // 等価性を検証する
        var result = productCategory.Equals(productCategory);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "UUIDで等価と判定される")]
    public void Equals_WithSameUuid_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid();
        var productCategory1 = CreateProductCategory(categoryUuid: uuid, categoryName: "食品");
        var productCategory2 = CreateProductCategory(categoryUuid: uuid, categoryName: "飲料");

        // 等価性を検証する
        var result = productCategory1.Equals(productCategory2);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "異なるUUIDで非等価と判定される")]
    public void Equals_WithDifferentUuid_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var productCategory1 = CreateProductCategory(categoryName: "食品");
        var productCategory2 = CreateProductCategory(categoryName: "飲料");

        // 等価性を検証する
        var result = productCategory1.Equals(productCategory2);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "nullと比較した場合、非等価と判定される")]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var productCategory = CreateProductCategory();

        // 等価性を検証する
        var result = productCategory.Equals(null);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "異なる型と比較した場合、非等価と判定される")]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var productCategory = CreateProductCategory();

        // 等価性を検証する
        var result = productCategory.Equals("productCategory");

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "同じUUIDの場合、同じハッシュコードが返される")]
    public void GetHashCode_WithSameUuid_ShouldReturnSameHashCode()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid();
        var productCategory1 = CreateProductCategory(categoryUuid: uuid, categoryName: "食品");
        var productCategory2 = CreateProductCategory(categoryUuid: uuid, categoryName: "飲料");

        // ハッシュコードを取得する
        var hashCode1 = productCategory1.GetHashCode();
        var hashCode2 = productCategory2.GetHashCode();

        // ハッシュコードを検証する
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [TestMethod(DisplayName = "ToStringで商品カテゴリ情報が文字列化される")]
    public void ToString_ShouldContainProductCategoryProperties()
    {
        // データを用意する
        var categoryUuid = Guid.NewGuid();
        var categoryName = "食品";

        // インスタンスを生成する
        var productCategory = CreateProductCategory(
            categoryUuid: categoryUuid,
            categoryName: categoryName);

        // 文字列化する
        var result = productCategory.ToString();

        // 文字列に商品カテゴリ情報が含まれることを検証する
        StringAssert.Contains(result, categoryUuid.ToString());
        StringAssert.Contains(result, categoryName);
    }
}