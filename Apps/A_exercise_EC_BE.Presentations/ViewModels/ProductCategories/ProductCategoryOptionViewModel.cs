namespace A_exercise_EC_BE.Presentations
    .ViewModels.ProductCategories;

/// <summary>
/// 商品カテゴリのプルダウン項目。
/// </summary>
public class ProductCategoryOptionViewModel
{
    /// <summary>
    /// 商品カテゴリUUID。
    /// </summary>
    public Guid Value { get; set; }

    /// <summary>
    /// 商品カテゴリ名。
    /// </summary>
    public string Label { get; set; } =
        string.Empty;
}