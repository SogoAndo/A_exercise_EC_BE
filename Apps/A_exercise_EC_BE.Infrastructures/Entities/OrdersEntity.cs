using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace A_exercise_EC_BE.Infrastructures.Entities;

/// <summary>
/// 注文テーブルのEntity
/// </summary>
[Table("orders")]
public class OrdersEntity
{
    /// <summary>
    /// 注文ID（主キー）
    /// </summary>
    [Column("id")]
    [Key] // 主キーをマッピング
    public int Id { get; set; }

    /// <summary>
    /// 注文識別ID（UUID）
    /// </summary>
    [Column("order_uuid")]
    public Guid OrderUuid { get; set; }

    /// <summary>
    /// 注文日
    /// </summary>
    [Column("order_date", TypeName = "timestamp without time zone")]
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// 合計金額
    /// </summary>
    [Column("amount_total")]
    public int AmountTotal { get; set; }

    /// <summary>
    /// 顧客ID（外部キー）
    /// </summary>
    [Column("customer_id")]
    public int CustomerId { get; set; }

    /// <summary>
    /// 注文ステータスID（外部キー）
    /// </summary>
    [Column("order_status_id")]
    public int OrderStatusId { get; set; }

    /// <summary>
    /// 支払い方法ID（外部キー）
    /// </summary>
    [Column("payment_method_id")]
    public int PaymentMethodId { get; set; }

    /// <summary>
    /// 顧客
    /// </summary>
    [ForeignKey("CustomerId")]
    public CustomerEntity Customer { get; set; } = null!;

    /// <summary>
    /// 注文ステータス
    /// </summary>
    [ForeignKey("OrderStatusId")]
    public OrderStatusEntity OrderStatus { get; set; } = null!;

    /// <summary>
    /// 支払い方法
    /// </summary>
    [ForeignKey("PaymentMethodId")]
    public PaymentMethodEntity PaymentMethod { get; set; } = null!;

    /// <summary>
    /// 注文明細一覧
    /// </summary>
    public List<OrdersDetailEntity> OrderDetails { get; set; } = new();

}