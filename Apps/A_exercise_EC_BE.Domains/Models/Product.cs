using A_exercise_EC_BE.Domains.Exceptions;
namespace A_exercise_EC_BE.Domains.Models;
/// <summary>
/// 商品を表すドメインオブジェクト
/// </summary>
public class Product
{
    /// <summary>
    /// 商品識別ID(UUID)
    /// </summary>
    public Guid ProductUuid { get; private set; }
    /// <summary>
    /// 商品名
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    /// 価格
    /// </summary>
    public int Price { get; private set; }
    /// <summary>
    /// 画像URL
    /// </summary>
    public string? ImageUrl { get; private set; } = string.Empty;
    /// <summary>
    /// 商品カテゴリ
    /// </summary>
    public ProductCategory? ProductCategory { get; private set; }
    /// <summary>
    /// 商品在庫
    /// </summary>
    public ProductStock? ProductStock { get; private set; }

    /// <summary>
    /// 削除フラグ
    /// </summary>
    public int DeleteFlg { get; private set; }

    /// <summary>
    /// 商品名の最大長
    /// </summary>
    private const int MaxLengthName = 100;
    /// <summary>
    /// 画像URLの最大長
    /// </summary>
    private const int MaxLengthImageUrl = 200;
    /// <summary>
    /// 価格の最大値
    /// </summary>
    private const int MaxPrice = 1000000;

    /// <summary>
    /// コンストラクタ
    /// productPriceは7/8以前引数string戻り値intにしていたが、
    /// Adapterとの互換性のため引数int戻り値intに変更
    /// </summary>
    public Product(Guid productUuid, string productName, int productPrice, string productImageUrl, ProductCategory? productCategory, ProductStock? productStock, int deleteFlg)
    {
        ValidateProductUuid(productUuid);
        ProductUuid = productUuid;
        ValidateProductName(productName);
        Name = productName;
        Price = productPrice;
        ValidatePrice(productPrice);
        ValidateImageUrl(productImageUrl);
        ImageUrl = productImageUrl;
        ProductCategory = productCategory ?? throw new DomainException("商品カテゴリは必須です");
        ProductStock = productStock ?? throw new DomainException("商品在庫は必須です");
        ValidateDeleteFlg(deleteFlg);
        DeleteFlg = deleteFlg;
    }

    /// <summary>
    /// ID未定の商品を作成する場合のコンストラクタ
    /// </summary>
    public Product(string productName, int productPrice, string productImageUrl, ProductCategory? productCategory, ProductStock? productStock, int deleteFlg)
        : this(Guid.NewGuid(), productName, productPrice, productImageUrl, productCategory, productStock, deleteFlg) { }

    /// <summary>
    /// 引数3つのコンストラクタ
    /// </summary>
    public Product(Guid productUuid, string productName, int productPrice)
    {
        ValidateProductUuid(productUuid);
        ProductUuid = productUuid;

        ValidateProductName(productName);
        Name = productName;

        ValidatePrice(productPrice);
        Price = productPrice;


    }
    public Product(Guid productUuid, string productName, int productPrice, string imageUrl, int deleteFlg)
    {
        ValidateProductUuid(productUuid);
        ProductUuid = productUuid;

        ValidateProductName(productName);
        Name = productName;

        ValidatePrice(productPrice);
        Price = productPrice;

        ValidateImageUrl(imageUrl);
        ImageUrl = imageUrl;

        ValidateDeleteFlg(deleteFlg);
        DeleteFlg = deleteFlg;
    }
    /// <summary>
    /// 商品識別IDの検証
    /// </summary>
    private void ValidateProductUuid(Guid productUuid)
    {
        if (productUuid == Guid.Empty)
            throw new DomainException("商品識別IDが不正です");
    }

    /// <summary>
    /// 商品名の検証
    /// </summary>
    private void ValidateProductName(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("商品名は必須です");
        if (productName.Length > MaxLengthName)
            throw new DomainException($"商品名は{MaxLengthName}文字で入力してください");
    }
    /// <summary>
    /// 価格の検証
    /// </summary>
    private int ValidatePrice(int productPrice)
    {
        if (productPrice > MaxPrice)
            throw new DomainException("価格は100万円以下で入力してください");

        return productPrice;
    }
    /// <summary>
    /// 画像URLの検証
    /// </summary>
    private void ValidateImageUrl(string? productImageUrl)
    {
        if (string.IsNullOrWhiteSpace(productImageUrl))
            return;

        if (productImageUrl.Length > MaxLengthImageUrl)
            throw new DomainException($"画像URLは{MaxLengthImageUrl}文字以内で入力してください");

        if (!Uri.TryCreate(productImageUrl, UriKind.Absolute, out var uri))
            throw new DomainException("画像URLの形式が不正です");

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            throw new DomainException("画像URLはhttpまたはhttpsで入力してください");
    }

    /// <summary>
    /// 削除フラグの検証
    /// </summary>
    private void ValidateDeleteFlg(int deleteFlg)
    {
        if (deleteFlg != 0 && deleteFlg != 1)
            throw new DomainException("削除フラグが不正です");
    }
    /// <summary>
    /// 商品名の変更
    /// </summary>
    public void ChangeName(string productName)
    {
        ValidateProductName(productName);
        Name = productName;
    }

    /// <summary>
    /// 価格の変更
    /// </summary>
    public void ChangePrice(int productPrice)
    {
        ValidatePrice(productPrice);
        Price = productPrice;
    }

    /// <summary>
    /// 画像URLの変更
    /// </summary>
    public void ChangeImageUrl(string productImageUrl)
    {
        ValidateImageUrl(productImageUrl);
        ImageUrl = productImageUrl;
    }

    /// <summary>
    /// 商品カテゴリの変更
    /// </summary>
    public void ChangeCategory(ProductCategory productCategory)
    {
        ProductCategory = productCategory ?? throw new DomainException("商品カテゴリは必須です");
    }


    /// <summary>
    /// 商品在庫の変更
    /// </summary>
    public void ChangeStock(ProductStock productStock)
    {
        ProductStock = productStock ?? throw new DomainException("商品在庫は必須です");
    }

    /// <summary>
    /// 削除フラグの変更
    /// </summary>
    public void ChangeDeleteFlg(int deleteFlg)
    {
        ValidateDeleteFlg(deleteFlg);
        DeleteFlg = deleteFlg;
    }

    /// <summary>
    /// 等価性（IDによる比較）
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not Product other) return false;
        return ProductUuid == other.ProductUuid;
    }

    public override int GetHashCode() => ProductUuid.GetHashCode();

    public override string ToString()
        => $"{ProductUuid}: {Name},{Price}円,{ImageUrl} / {ProductCategory?.Name} / {DeleteFlg}";
}