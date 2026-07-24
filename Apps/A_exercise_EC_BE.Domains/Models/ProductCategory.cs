using A_exercise_EC_BE.Domains.Exceptions;
namespace A_exercise_EC_BE.Domains.Models;
/// <summary>
/// 商品カテゴリを表すドメインオブジェクト
/// </summary>
public class ProductCategory
{
    /// <summary>
    /// 商品カテゴリ識別ID(UUID)s
    /// </summary>
    public Guid CategoryUuid { get; private set; }
    /// <summary>
    /// 商品カテゴリ名
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 商品カテゴリ名の最大長
    /// </summary>
    private const int MaxLengthCategoryName = 30;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public ProductCategory(Guid categoryUuid, string categoryName)
    {
        ValidateCategoryUuid(categoryUuid);
        CategoryUuid = categoryUuid;
        ValidateCategoryName(categoryName);
        Name = categoryName;
    }

    /// <summary>
    /// ID未定の商品カテゴリを作成する場合のコンストラクタ
    /// </summary>
    public ProductCategory(string categoryName)
        : this(Guid.NewGuid(), categoryName) { }

    /// <summary>
    /// 商品カテゴリ識別IDの検証
    /// </summary>
    private void ValidateCategoryUuid(Guid categoryUuid)
    {
        if (categoryUuid == Guid.Empty)
            throw new DomainException("商品カテゴリ識別IDが不正です");
    }

    /// <summary>
    /// 商品カテゴリ名の検証
    /// </summary>
    private void ValidateCategoryName(string categoryName)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new DomainException("商品カテゴリ名は必須です");
        if (categoryName.Length > MaxLengthCategoryName)
            throw new DomainException($"商品カテゴリ名は{MaxLengthCategoryName}文字以内で入力してください");
    }

    /// <summary>
    /// 等価性（IDによる比較）
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not ProductCategory other) return false;
        return CategoryUuid == other.CategoryUuid;
    }

    public override int GetHashCode() => CategoryUuid.GetHashCode();

    public override string ToString()
        => $"{CategoryUuid}: {Name}";
}