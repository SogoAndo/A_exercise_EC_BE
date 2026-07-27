namespace A_exercise_EC_BE.Presentations.ViewModels.Products;

/// <summary>
/// UC004 商品詳細の応答。
/// </summary>
public sealed record ProductDetailViewModel(
    Guid ProductUuid,
    string ProductName,
    int Price,
    string? ProductImage,
    int StockQuantity);
