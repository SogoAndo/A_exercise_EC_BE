using A_exercise_EC_BE.Domain.Exceptions;
namespace A_exercise_EC_BE.Domain.Models;
/// <summary>
/// 支払い方法を表すドメインオブジェクト
/// </summary>
public class PaymentMethod
{
    /// <summary>
    /// 支払い方法ID
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 支払い方法名
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    /// <summary>
    ///  支払い方法の最大長
    /// </summary>
    private const int MaxLengthPaymentName = 100;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PaymentMethod(int paymentId, string paymentName)
    {
        ValidatePaymentId(paymentId);
        Id = paymentId;
        ValidatePaymentName(paymentName);
        Name = paymentName;
    }

    /// <summary>
    /// ID未定の支払い方法を作成する場合のコンストラクタ
    /// </summary>
    public PaymentMethod(string paymentName)
    {
        ValidatePaymentName(paymentName);
        Name = paymentName;
    }

    /// <summary>
    /// 支払い方法IDの検証
    /// </summary>
    private void ValidatePaymentId(int paymentId)
    {
        if (paymentId <= 0)
            throw new DomainException("支払い方法IDが不正です");
    }

    /// <summary>
    /// 支払い方法名の検証
    /// </summary>
    private void ValidatePaymentName(string paymentName)
    {
        if (string.IsNullOrWhiteSpace(paymentName))
            throw new DomainException("支払い方法名は必須です");
        if (paymentName.Length > MaxLengthPaymentName)
            throw new DomainException($"支払い方法名は{MaxLengthPaymentName}文字以内で入力してください");
    }

    /// <summary>
    /// 等価性（IDによる比較）
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not PaymentMethod other) return false;

        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString()
        => $"{Id}: {Name}";
}