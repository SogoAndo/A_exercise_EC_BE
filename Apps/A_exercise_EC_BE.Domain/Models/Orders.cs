using A_exercise_EC_BE.Domain.Exceptions;
namespace A_exercise_EC_BE.Domain.Models;
/// <summary>
/// 注文を表すドメインオブジェクト
/// </summary>
public class Orders
{
    /// <summary>
    /// 注文識別ID(UUID)
    /// </summary>
    public Guid OrderUuid { get; private set; }
    /// <summary>
    /// 注文日
    /// </summary>
    public DateTime OrderDate { get; private set; }
    /// <summary>
    /// 合計金額
    /// </summary>
    public int AmountTotal { get; private set; }
    /// <summary>
    /// 顧客
    /// </summary>
    public Customer Customer { get; private set; }
    /// <summary>
    /// 注文ステータス
    /// </summary>
    public OrderStatus OrderStatus { get; private set; }
    /// <summary>
    /// 支払い方法
    /// </summary>
    public PaymentMethod PaymentMethod { get; private set; }
    /// <summary>
    /// 注文明細（リスト）
    /// </summary>
    public List<OrdersDetail> OrdersDetails { get; private set; } = new();
    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Orders(Guid orderUuid, DateTime orderDate, int amountTotal, Customer? customer, OrderStatus? orderStatus, PaymentMethod? paymentMethod, List<OrdersDetail> ordarsDetails)
    {
        ValidateOrderUuid(orderUuid);
        OrderUuid = orderUuid;

        ValidateOrderDate(orderDate);
        OrderDate = orderDate;

        ValidateAmountTotal(amountTotal);
        AmountTotal = amountTotal;

        Customer = customer ?? throw new DomainException("顧客は必須です。");
        OrderStatus = orderStatus ?? throw new DomainException("注文ステータスは必須です。");
        PaymentMethod = paymentMethod ?? throw new DomainException("支払い方法は必須です。");
        OrdersDetails = ordarsDetails ?? throw new DomainException("注文明細は必須です。");
    }

    /// <summary>
    /// ID未定の注文を作成する場合のコンストラクタ
    /// </summary>
    public Orders(DateTime orderDate, int amountTotal, Customer customer, OrderStatus orderStatus, PaymentMethod paymentMethod, List<OrdersDetail> ordarsDetails)
        : this(Guid.NewGuid(), orderDate, amountTotal, customer, orderStatus, paymentMethod, ordarsDetails) { }

    /// <summary>
    /// 注文識別IDの検証
    /// </summary>
    private void ValidateOrderUuid(Guid orderUuid)
    {
        if (orderUuid == Guid.Empty)
            throw new DomainException("注文識別IDが不正です");
    }

    /// <summary>
    /// 注文日の検証
    /// </summary>
    private void ValidateOrderDate(DateTime orderDate)
    {
        if (orderDate == default)
            throw new DomainException("注文日が不正です");

        if (orderDate > DateTime.Now)
            throw new DomainException("注文日に未来日は指定できません");
    }

    /// <summary>
    /// 合計金額の検証
    /// </summary>
    private int ValidateAmountTotal(int amountTotal)
    {
        if (amountTotal < 0)
            throw new DomainException("合計金額は0以上で入力してください");

        return amountTotal;
    }

    /// <summary>
    /// 注文ステータスの変更
    /// </summary>
    /// <param name="orderStatus">注文ステータス</param>
    public void ChangeOrderStatus(OrderStatus orderStatus)
    {
        OrderStatus = orderStatus ?? throw new DomainException("注文ステータスは必須です。");
    }

    /// <summary>
    /// 注文明細の変更
    /// </summary>
    /// <param name="orderDetail">注文明細</param>
    public void AddOrderDetail(OrdersDetail orderDetail)
    {
        OrdersDetails.Add(orderDetail ?? throw new DomainException("注文明細は必須です。"));
    }

    /// <summary>
    /// 等価性（IDによる比較）
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not Orders other) return false;
        return OrderUuid == other.OrderUuid;
    }

    public override int GetHashCode() => OrderUuid.GetHashCode();

    public override string ToString()
        => $"{OrderUuid}: {OrderDate}, {AmountTotal} / {Customer.Name} / {OrderStatus.Name} / {PaymentMethod.Name}";
}