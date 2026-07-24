using Microsoft.VisualStudio.TestTools.UnitTesting;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Tests.Models;

/// <summary>
/// Productクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class ProductTests
{
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
    private Product CreateProduct(
        Guid? productUuid = null,
        string productName = "商品A",
        int productPrice = 1000,
        string productImageUrl = "https://example.com/image.png",
        ProductCategory? productCategory = null,
        ProductStock? productStock = null,
        int deleteFlg = 0)
    {
        return new Product(
            productUuid ?? Guid.NewGuid(),
            productName,
            productPrice,
            productImageUrl,
            productCategory ?? CreateProductCategory(),
            productStock ?? CreateProductStock(),
            deleteFlg);
    }

    [TestMethod(DisplayName = "コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var productUuid = Guid.NewGuid();
        var productName = "商品A";
        var productPrice = 1000;
        var productImageUrl = "https://example.com/image.png";
        var productCategory = CreateProductCategory();
        var productStock = CreateProductStock();
        var deleteFlg = 0;

        // インスタンスを生成する
        var product = new Product(
            productUuid,
            productName,
            productPrice,
            productImageUrl,
            productCategory,
            productStock,
            deleteFlg);

        // 商品識別IDを検証する
        Assert.AreEqual(productUuid, product.ProductUuid);

        // 商品名を検証する
        Assert.AreEqual(productName, product.Name);

        // 価格を検証する
        Assert.AreEqual(productPrice, product.Price);

        // 画像URLを検証する
        Assert.AreEqual(productImageUrl, product.ImageUrl);

        // 商品カテゴリを検証する
        Assert.AreEqual(productCategory, product.ProductCategory);

        // 商品在庫を検証する
        Assert.AreEqual(productStock, product.ProductStock);

        // 削除フラグを検証する
        Assert.AreEqual(deleteFlg, product.DeleteFlg);
    }

    [TestMethod(DisplayName = "新規作成の場合UUIDが自動生成される")]
    public void NewInstance_ShouldGenerateUuidAutomatically()
    {
        // データを用意する
        var productName = "商品A";
        var productPrice = 1000;
        var productImageUrl = "https://example.com/image.png";
        var productCategory = CreateProductCategory();
        var productStock = CreateProductStock();
        var deleteFlg = 0;

        // インスタンスを生成する
        var product = new Product(
            productName,
            productPrice,
            productImageUrl,
            productCategory,
            productStock,
            deleteFlg);

        // 商品識別IDが自動生成されていることを検証する
        Assert.AreNotEqual(Guid.Empty, product.ProductUuid);

        // 商品名を検証する
        Assert.AreEqual(productName, product.Name);

        // 価格を検証する
        Assert.AreEqual(productPrice, product.Price);

        // 画像URLを検証する
        Assert.AreEqual(productImageUrl, product.ImageUrl);

        // 商品カテゴリを検証する
        Assert.AreEqual(productCategory, product.ProductCategory);

        // 商品在庫を検証する
        Assert.AreEqual(productStock, product.ProductStock);

        // 削除フラグを検証する
        Assert.AreEqual(deleteFlg, product.DeleteFlg);
    }

    [TestMethod(DisplayName = "引数3つのコンストラクタに正常値を指定すると商品識別ID、商品名、価格が設定される")]
    public void Constructor_WithThreeArguments_ShouldSetProductUuidNameAndPrice()
    {
        // データを用意する
        var productUuid = Guid.NewGuid();
        var productName = "商品A";
        var productPrice = 1000;

        // インスタンスを生成する
        var product = new Product(
            productUuid,
            productName,
            productPrice);

        // 商品識別IDを検証する
        Assert.AreEqual(productUuid, product.ProductUuid);

        // 商品名を検証する
        Assert.AreEqual(productName, product.Name);

        // 価格を検証する
        Assert.AreEqual(productPrice, product.Price);
    }

    /// <summary>
    /// 5引数コンストラクタに正常値を指定した場合のテスト
    /// </summary>
    [TestMethod(
        DisplayName =
            "5引数コンストラクタに正常値を指定すると各プロパティが設定される")]
    public void Constructor_WithFiveArguments_ShouldSetProperties()
    {
        // Arrange
        var productUuid = Guid.NewGuid();
        var productName = "商品A";
        var productPrice = 1000;
        var imageUrl = "https://example.com/image.png";
        var deleteFlg = 0;

        // Act
        var product = new Product(
            productUuid,
            productName,
            productPrice,
            imageUrl,
            deleteFlg);

        // Assert
        Assert.AreEqual(
            productUuid,
            product.ProductUuid);

        Assert.AreEqual(
            productName,
            product.Name);

        Assert.AreEqual(
            productPrice,
            product.Price);

        Assert.AreEqual(
            imageUrl,
            product.ImageUrl);

        Assert.AreEqual(
            deleteFlg,
            product.DeleteFlg);

        /*
         * このコンストラクタでは
         * カテゴリと在庫を設定しない。
         */
        Assert.IsNull(product.ProductCategory);
        Assert.IsNull(product.ProductStock);
    }

    [TestMethod(DisplayName = "商品識別IDが空の場合、DomainExceptionがスローされる")]
    public void EmptyProductUuid_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProduct(productUuid: Guid.Empty);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品識別IDが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "商品名がnullの場合、DomainExceptionがスローされる")]
    public void NullProductName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProduct(productName: null!);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "商品名が空白の場合、DomainExceptionがスローされる")]
    public void EmptyProductName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProduct(productName: "");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "商品名が空白文字のみの場合、DomainExceptionがスローされる")]
    public void WhiteSpaceProductName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProduct(productName: "   ");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "商品名が100文字の場合、インスタンス生成される")]
    public void ProductName_With100Chars_ShouldCreateInstance()
    {
        // データを用意する
        var productName = new string('あ', 100);

        // インスタンスを生成する
        var product = CreateProduct(productName: productName);

        // 商品名を検証する
        Assert.AreEqual(productName, product.Name);
    }

    [TestMethod(DisplayName = "商品名が101文字以上の場合、DomainExceptionがスローされる")]
    public void ProductName_LongerThan100Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var productName = new string('あ', 101);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProduct(productName: productName);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品名は100文字で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "価格が0円の場合、インスタンス生成される")]
    public void Price_WithZero_ShouldCreateInstance()
    {
        // データを用意する
        var productPrice = 0;

        // インスタンスを生成する
        var product = CreateProduct(productPrice: productPrice);

        // 価格を検証する
        Assert.AreEqual(productPrice, product.Price);
    }

    [TestMethod(DisplayName = "価格が100万円の場合、インスタンス生成される")]
    public void Price_WithMaxValue_ShouldCreateInstance()
    {
        // データを用意する
        var productPrice = 1000000;

        // インスタンスを生成する
        var product = CreateProduct(productPrice: productPrice);

        // 価格を検証する
        Assert.AreEqual(productPrice, product.Price);
    }

    [TestMethod(DisplayName = "価格が100万円を超える場合、DomainExceptionがスローされる")]
    public void Price_OverMaxValue_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProduct(productPrice: 1000001);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("価格は100万円以下で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "価格が負数の場合、現在の実装ではインスタンス生成される")]
    public void Price_WithNegativeValue_ShouldCreateInstance_CurrentImplementation()
    {
        // データを用意する
        var productPrice = -1;

        // インスタンスを生成する
        var product = CreateProduct(productPrice: productPrice);

        // 価格を検証する
        Assert.AreEqual(productPrice, product.Price);
    }

    [TestMethod(DisplayName = "画像URLが空白の場合、インスタンス生成される")]
    public void EmptyImageUrl_ShouldCreateInstance()
    {
        // データを用意する
        var productImageUrl = "";

        // インスタンスを生成する
        var product = CreateProduct(productImageUrl: productImageUrl);

        // 画像URLを検証する
        Assert.AreEqual(productImageUrl, product.ImageUrl);
    }

    [TestMethod(DisplayName = "画像URLが空白文字のみの場合、インスタンス生成される")]
    public void WhiteSpaceImageUrl_ShouldCreateInstance()
    {
        // データを用意する
        var productImageUrl = "   ";

        // インスタンスを生成する
        var product = CreateProduct(productImageUrl: productImageUrl);

        // 画像URLを検証する
        Assert.AreEqual(productImageUrl, product.ImageUrl);
    }

    [TestMethod(DisplayName = "画像URLがhttp形式の場合、インスタンス生成される")]
    public void ImageUrl_WithHttp_ShouldCreateInstance()
    {
        // データを用意する
        var productImageUrl = "http://example.com/image.png";

        // インスタンスを生成する
        var product = CreateProduct(productImageUrl: productImageUrl);

        // 画像URLを検証する
        Assert.AreEqual(productImageUrl, product.ImageUrl);
    }

    [TestMethod(DisplayName = "画像URLがhttps形式の場合、インスタンス生成される")]
    public void ImageUrl_WithHttps_ShouldCreateInstance()
    {
        // データを用意する
        var productImageUrl = "https://example.com/image.png";

        // インスタンスを生成する
        var product = CreateProduct(productImageUrl: productImageUrl);

        // 画像URLを検証する
        Assert.AreEqual(productImageUrl, product.ImageUrl);
    }

    [TestMethod(DisplayName = "画像URLが201文字以上の場合、DomainExceptionがスローされる")]
    public void ImageUrl_LongerThan200Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var productImageUrl = "https://example.com/" + new string('a', 181);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProduct(productImageUrl: productImageUrl);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("画像URLは200文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "画像URLの形式が不正な場合、DomainExceptionがスローされる")]
    public void InvalidImageUrl_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProduct(productImageUrl: "not-url");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("画像URLの形式が不正です", ex.Message);
    }

    [TestMethod(DisplayName = "画像URLがhttpまたはhttps以外の場合、DomainExceptionがスローされる")]
    public void ImageUrl_WithInvalidScheme_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProduct(productImageUrl: "ftp://example.com/image.png");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("画像URLはhttpまたはhttpsで入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "商品カテゴリがnullの場合、DomainExceptionがスローされる")]
    public void NullProductCategory_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new Product(
                Guid.NewGuid(),
                "商品A",
                1000,
                "https://example.com/image.png",
                null,
                CreateProductStock(),
                0);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品カテゴリは必須です", ex.Message);
    }

    [TestMethod(DisplayName = "商品在庫がnullの場合、DomainExceptionがスローされる")]
    public void NullProductStock_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = new Product(
                Guid.NewGuid(),
                "商品A",
                1000,
                "https://example.com/image.png",
                CreateProductCategory(),
                null,
                0);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品在庫は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "削除フラグが0の場合、インスタンス生成される")]
    public void DeleteFlg_WithZero_ShouldCreateInstance()
    {
        // データを用意する
        var deleteFlg = 0;

        // インスタンスを生成する
        var product = CreateProduct(deleteFlg: deleteFlg);

        // 削除フラグを検証する
        Assert.AreEqual(deleteFlg, product.DeleteFlg);
    }

    [TestMethod(DisplayName = "削除フラグが1の場合、インスタンス生成される")]
    public void DeleteFlg_WithOne_ShouldCreateInstance()
    {
        // データを用意する
        var deleteFlg = 1;

        // インスタンスを生成する
        var product = CreateProduct(deleteFlg: deleteFlg);

        // 削除フラグを検証する
        Assert.AreEqual(deleteFlg, product.DeleteFlg);
    }

    [TestMethod(DisplayName = "削除フラグが負数の場合、DomainExceptionがスローされる")]
    public void DeleteFlg_WithNegativeValue_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProduct(deleteFlg: -1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("削除フラグが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "削除フラグが2の場合、DomainExceptionがスローされる")]
    public void DeleteFlg_WithTwo_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateProduct(deleteFlg: 2);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("削除フラグが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "有効な商品名に変更できる")]
    public void ChangeName_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var product = CreateProduct();

        // 商品名を変更する
        product.ChangeName("新商品");

        // 商品名を検証する
        Assert.AreEqual("新商品", product.Name);
    }

    [TestMethod(DisplayName = "不正な商品名に変更するとDomainExceptionがスローされる")]
    public void ChangeName_WithInvalidValue_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangeName("");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "有効な価格に変更できる")]
    public void ChangePrice_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var product = CreateProduct();

        // 価格を変更する
        product.ChangePrice(2000);

        // 価格を検証する
        Assert.AreEqual(2000, product.Price);
    }

    [TestMethod(DisplayName = "価格を100万円に変更できる")]
    public void ChangePrice_WithMaxValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var product = CreateProduct();

        // 価格を変更する
        product.ChangePrice(1000000);

        // 価格を検証する
        Assert.AreEqual(1000000, product.Price);
    }

    [TestMethod(DisplayName = "価格を100万円超に変更するとDomainExceptionがスローされる")]
    public void ChangePrice_OverMaxValue_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangePrice(1000001);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("価格は100万円以下で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "価格を負数に変更すると現在の実装では変更できる")]
    public void ChangePrice_WithNegativeValue_ShouldSucceed_CurrentImplementation()
    {
        // インスタンスを生成する
        var product = CreateProduct();

        // 価格を変更する
        product.ChangePrice(-1);

        // 価格を検証する
        Assert.AreEqual(-1, product.Price);
    }

    [TestMethod(DisplayName = "有効な画像URLに変更できる")]
    public void ChangeImageUrl_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var product = CreateProduct();

        // 画像URLを変更する
        product.ChangeImageUrl("https://example.com/new.png");

        // 画像URLを検証する
        Assert.AreEqual("https://example.com/new.png", product.ImageUrl);
    }

    [TestMethod(DisplayName = "不正な画像URLに変更するとDomainExceptionがスローされる")]
    public void ChangeImageUrl_WithInvalidValue_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangeImageUrl("not-url");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("画像URLの形式が不正です", ex.Message);
    }

    [TestMethod(DisplayName = "有効な商品カテゴリに変更できる")]
    public void ChangeCategory_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var product = CreateProduct();
        var newCategory = CreateProductCategory("新カテゴリ");

        // 商品カテゴリを変更する
        product.ChangeCategory(newCategory);

        // 商品カテゴリを検証する
        Assert.AreEqual("新カテゴリ", product.ProductCategory!.Name);
    }

    [TestMethod(DisplayName = "商品カテゴリをnullに変更するとDomainExceptionがスローされる")]
    public void ChangeCategory_WithNull_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangeCategory(null!);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品カテゴリは必須です", ex.Message);
    }

    [TestMethod(DisplayName = "有効な商品在庫に変更できる")]
    public void ChangeStock_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var product = CreateProduct();
        var newStock = CreateProductStock(30);

        // 商品在庫を変更する
        product.ChangeStock(newStock);

        // 商品在庫を検証する
        Assert.AreEqual(30, product.ProductStock!.Quantity);
    }

    [TestMethod(DisplayName = "商品在庫をnullに変更するとDomainExceptionがスローされる")]
    public void ChangeStock_WithNull_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangeStock(null!);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("商品在庫は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "削除フラグを0に変更できる")]
    public void ChangeDeleteFlg_WithZero_ShouldSucceed()
    {
        // インスタンスを生成する
        var product = CreateProduct(deleteFlg: 1);

        // 削除フラグを変更する
        product.ChangeDeleteFlg(0);

        // 削除フラグを検証する
        Assert.AreEqual(0, product.DeleteFlg);
    }

    [TestMethod(DisplayName = "削除フラグを1に変更できる")]
    public void ChangeDeleteFlg_WithOne_ShouldSucceed()
    {
        // インスタンスを生成する
        var product = CreateProduct(deleteFlg: 0);

        // 削除フラグを変更する
        product.ChangeDeleteFlg(1);

        // 削除フラグを検証する
        Assert.AreEqual(1, product.DeleteFlg);
    }

    [TestMethod(DisplayName = "削除フラグを不正値に変更するとDomainExceptionがスローされる")]
    public void ChangeDeleteFlg_WithInvalidValue_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangeDeleteFlg(2);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("削除フラグが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "同一インスタンスの場合、等価と判定される")]
    public void Equals_WithSameInstance_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var product = CreateProduct();

        // 等価性を検証する
        var result = product.Equals(product);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "UUIDで等価と判定される")]
    public void Equals_WithSameUuid_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid();
        var product1 = CreateProduct(productUuid: uuid, productName: "商品A");
        var product2 = CreateProduct(productUuid: uuid, productName: "商品B");

        // 等価性を検証する
        var result = product1.Equals(product2);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "異なるUUIDで非等価と判定される")]
    public void Equals_WithDifferentUuid_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var product1 = CreateProduct(productName: "商品A");
        var product2 = CreateProduct(productName: "商品B");

        // 等価性を検証する
        var result = product1.Equals(product2);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "nullと比較した場合、非等価と判定される")]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var product = CreateProduct();

        // 等価性を検証する
        var result = product.Equals(null);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "異なる型と比較した場合、非等価と判定される")]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var product = CreateProduct();

        // 等価性を検証する
        var result = product.Equals("product");

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "同じUUIDの場合、同じハッシュコードが返される")]
    public void GetHashCode_WithSameUuid_ShouldReturnSameHashCode()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid();
        var product1 = CreateProduct(productUuid: uuid, productName: "商品A");
        var product2 = CreateProduct(productUuid: uuid, productName: "商品B");

        // ハッシュコードを取得する
        var hashCode1 = product1.GetHashCode();
        var hashCode2 = product2.GetHashCode();

        // ハッシュコードを検証する
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [TestMethod(DisplayName = "ToStringで商品情報が文字列化される")]
    public void ToString_ShouldContainProductProperties()
    {
        // データを用意する
        var productUuid = Guid.NewGuid();
        var productName = "商品A";
        var productPrice = 1000;
        var productImageUrl = "https://example.com/image.png";
        var productCategory = CreateProductCategory("食品");
        var productStock = CreateProductStock(10);
        var deleteFlg = 0;

        // インスタンスを生成する
        var product = CreateProduct(
            productUuid: productUuid,
            productName: productName,
            productPrice: productPrice,
            productImageUrl: productImageUrl,
            productCategory: productCategory,
            productStock: productStock,
            deleteFlg: deleteFlg);

        // 文字列化する
        var result = product.ToString();

        // 文字列に商品情報が含まれることを検証する
        StringAssert.Contains(result, productUuid.ToString());
        StringAssert.Contains(result, productName);
        StringAssert.Contains(result, productPrice.ToString());
        StringAssert.Contains(result, productImageUrl);
        StringAssert.Contains(result, "食品");
        StringAssert.Contains(result, deleteFlg.ToString());
    }

    [TestMethod(DisplayName = "画像URLがnullの場合、インスタンス生成される")]
    public void NullImageUrl_ShouldCreateInstance()
    {
        var product = CreateProduct(productImageUrl: null!);

        Assert.IsNull(product.ImageUrl);
    }

    [TestMethod(DisplayName = "画像URLが200文字の場合、インスタンス生成される")]
    public void ImageUrl_With200Chars_ShouldCreateInstance()
    {
        var prefix = "https://example.com/";
        var productImageUrl = prefix + new string('a', 200 - prefix.Length);

        var product = CreateProduct(productImageUrl: productImageUrl);

        Assert.AreEqual(productImageUrl, product.ImageUrl);
    }

    [TestMethod(DisplayName = "商品名をnullに変更するとDomainExceptionがスローされる")]
    public void ChangeName_WithNull_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangeName(null!);
        });

        Assert.AreEqual("商品名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "商品名を空白文字のみに変更するとDomainExceptionがスローされる")]
    public void ChangeName_WithWhiteSpace_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangeName("   ");
        });

        Assert.AreEqual("商品名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "商品名を101文字に変更するとDomainExceptionがスローされる")]
    public void ChangeName_With101Chars_ShouldThrowDomainException()
    {
        var product = CreateProduct();
        var productName = new string('あ', 101);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangeName(productName);
        });

        Assert.AreEqual("商品名は100文字で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "画像URLをhttp形式に変更できる")]
    public void ChangeImageUrl_WithHttp_ShouldSucceed()
    {
        var product = CreateProduct();

        var imageUrl = "http://example.com/new.png";
        product.ChangeImageUrl(imageUrl);

        Assert.AreEqual(imageUrl, product.ImageUrl);
    }

    [TestMethod(DisplayName = "画像URLを空文字に変更できる")]
    public void ChangeImageUrl_WithEmpty_ShouldSucceed()
    {
        var product = CreateProduct();

        product.ChangeImageUrl("");

        Assert.AreEqual("", product.ImageUrl);
    }

    [TestMethod(DisplayName = "画像URLを空白文字のみに変更できる")]
    public void ChangeImageUrl_WithWhiteSpace_ShouldSucceed()
    {
        var product = CreateProduct();

        product.ChangeImageUrl("   ");

        Assert.AreEqual("   ", product.ImageUrl);
    }

    [TestMethod(DisplayName = "画像URLを201文字に変更するとDomainExceptionがスローされる")]
    public void ChangeImageUrl_With201Chars_ShouldThrowDomainException()
    {
        var product = CreateProduct();
        var prefix = "https://example.com/";
        var imageUrl = prefix + new string('a', 201 - prefix.Length);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangeImageUrl(imageUrl);
        });

        Assert.AreEqual("画像URLは200文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "画像URLをftp形式に変更するとDomainExceptionがスローされる")]
    public void ChangeImageUrl_WithFtp_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangeImageUrl("ftp://example.com/image.png");
        });

        Assert.AreEqual("画像URLはhttpまたはhttpsで入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "削除フラグを-1に変更するとDomainExceptionがスローされる")]
    public void ChangeDeleteFlg_WithNegativeValue_ShouldThrowDomainException()
    {
        var product = CreateProduct();

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            product.ChangeDeleteFlg(-1);
        });

        Assert.AreEqual("削除フラグが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "ToStringで商品カテゴリがnullの場合も文字列化できる")]
    public void ToString_WhenProductCategoryIsNull_ShouldReturnString()
    {
        // Arrange
        var productUuid = Guid.NewGuid();

        var product = new Product(
            productUuid,
            "商品A",
            1000);

        // Act
        var result = product.ToString();

        // Assert
        Assert.AreEqual(
            $"{productUuid}: 商品A,1000円, /  / 0",
            result);
    }
}