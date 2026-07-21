using A_exercise_EC_BE.Domain.Exceptions;

namespace A_exercise_EC_BE.Domain.Models;

/// <summary>
/// ECサイトに表示する商品を表すドメインオブジェクト。
/// </summary>
public class Product
{
    private const int MaxNameLength = 100;
    private const int MaxImageUrlLength = 200;
    private const int MaxPrice = 1_000_000;

    public Guid ProductUuid { get; }
    public string Name { get; }
    public int Price { get; }
    public string? ImageUrl { get; }
    public ProductCategory ProductCategory { get; }
    public ProductStock ProductStock { get; }
    public int DeleteFlg { get; }

    public Product(
        Guid productUuid,
        string name,
        int price,
        string? imageUrl,
        ProductCategory productCategory,
        ProductStock productStock,
        int deleteFlg)
    {
        if (productUuid == Guid.Empty)
        {
            throw new DomainException("商品識別IDが不正です");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("商品名は必須です");
        }

        if (name.Length > MaxNameLength)
        {
            throw new DomainException($"商品名は{MaxNameLength}文字以内で入力してください");
        }

        if (price < 0 || price > MaxPrice)
        {
            throw new DomainException($"価格は0円以上{MaxPrice:N0}円以下で入力してください");
        }

        ValidateImageUrl(imageUrl);

        if (deleteFlg is not 0 and not 1)
        {
            throw new DomainException("削除フラグが不正です");
        }

        ProductUuid = productUuid;
        Name = name;
        Price = price;
        ImageUrl = imageUrl;
        ProductCategory = productCategory
            ?? throw new DomainException("商品カテゴリは必須です");
        ProductStock = productStock
            ?? throw new DomainException("商品在庫は必須です");
        DeleteFlg = deleteFlg;
    }

    private static void ValidateImageUrl(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return;
        }

        if (imageUrl.Length > MaxImageUrlLength)
        {
            throw new DomainException($"画像URLは{MaxImageUrlLength}文字以内で入力してください");
        }

        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new DomainException("画像URLはhttpまたはhttpsの絶対URLで入力してください");
        }
    }
}
