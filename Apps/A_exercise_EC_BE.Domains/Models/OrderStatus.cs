using A_exercise_EC_BE.Domains.Exceptions;
namespace A_exercise_EC_BE.Domains.Models;
/// <summary>
/// 注文ステータスを表すドメインオブジェクト
/// </summary>
public class OrderStatus
{
    /// <summary>
    /// 注文ステータスID
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 注文ステータス名
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 注文ステータス名の最大長
    /// </summary>
    private const int MaxLengthStatusName = 100;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public OrderStatus(int statusId, string statusName)
    {
        ValidateStatusId(statusId);
        Id = statusId;
        ValidateStatusName(statusName);
        Name = statusName;
    }

    /// <summary>
    /// ID未定の注文ステータスを作成するコンストラクタ
    /// </summary>
    public OrderStatus(string statusName)
    {
        ValidateStatusName(statusName);
        Name = statusName;
    }

    /// <summary>
    /// 注文ステータスIDの検証
    /// </summary>
    private void ValidateStatusId(int statusId)
    {
        if (statusId <= 0)
            throw new DomainException("注文ステータスIDが不正です");
    }

    /// <summary>
    /// 注文ステータス名の検証
    /// </summary>
    private void ValidateStatusName(string statusName)
    {
        if (string.IsNullOrWhiteSpace(statusName))
            throw new DomainException("注文ステータス名は必須です");
        if (statusName.Length > MaxLengthStatusName)
            throw new DomainException($"注文ステータス名は{MaxLengthStatusName}文字以内で入力してください");
    }

    /// <summary>
    /// 等価性（IDによる比較）
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not OrderStatus other) return false;

        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString()
        => $"{Id}: {Name}";
}