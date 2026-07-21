using A_exercise_EC_BE.Domain.Exceptions;

namespace A_exercise_EC_BE.Domain.Models;

/// <summary>
/// 商品カテゴリを表すドメインオブジェクト。
/// </summary>
public class ProductCategory
{
    private const int MaxNameLength = 30;

    public Guid CategoryUuid { get; }
    public string Name { get; }

    public ProductCategory(Guid categoryUuid, string name)
    {
        if (categoryUuid == Guid.Empty)
        {
            throw new DomainException("商品カテゴリ識別IDが不正です");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("商品カテゴリ名は必須です");
        }

        if (name.Length > MaxNameLength)
        {
            throw new DomainException($"商品カテゴリ名は{MaxNameLength}文字以内で入力してください");
        }

        CategoryUuid = categoryUuid;
        Name = name;
    }

    public override bool Equals(object? obj) =>
        obj is ProductCategory other && CategoryUuid == other.CategoryUuid;

    public override int GetHashCode() => CategoryUuid.GetHashCode();
}
