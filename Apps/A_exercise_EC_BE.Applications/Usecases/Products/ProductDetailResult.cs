namespace A_exercise_EC_BE.Applications.Usecases.Products;

/// <summary>
/// UC004 商品詳細取得の処理結果。
/// </summary>
public sealed record ProductDetailResult(
    Guid ProductUuid,
    string ProductName,
    int Price,
    string? ProductImage,
    int StockQuantity);
