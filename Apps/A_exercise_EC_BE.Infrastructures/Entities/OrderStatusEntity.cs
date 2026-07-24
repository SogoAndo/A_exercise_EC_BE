using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace A_exercise_EC_BE.Infrastructures.Entities;

/// <summary>
/// 注文ステータステーブルのEntity
/// </summary>
[Table("order_status")]
public class OrderStatusEntity
{
    /// <summary>
    /// 注文ステータスID（主キー）
    /// </summary>
    [Column("id")]
    [Key] // 主キーをマッピング
    public int Id { get; set; }

    /// <summary>
    /// 注文ステータス名
    /// </summary>
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    // Ordersだと単数かリストかわからないのでListをつけた
    public List<OrdersEntity> ListOrders { get; set; } = new();

}