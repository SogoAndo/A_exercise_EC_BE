using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace A_exercise_EC_BE.Infrastructure.Entities;

/// <summary>
/// 支払い方法テーブルのEntity
/// </summary>
[Table("payment_method")]
public class PaymentMethodEntity
{
    /// <summary>
    /// 支払い方法ID（主キー）
    /// </summary>
    [Column("id")]
    [Key] // 主キーをマッピング
    public int Id { get; set; }

    /// <summary>
    /// 支払い方法名
    /// </summary>
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    public List<OrdersEntity> ListOrders { get; set; } = new();

}