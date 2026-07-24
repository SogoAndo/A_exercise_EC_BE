using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace A_exercise_EC_BE.Infrastructure.Entities;

/// <summary>
/// 注文明細テーブルのEntity
/// </summary>
[Table("orders_detail")]
public class OrdersDetailEntity
{
    /// <summary>
    /// 注文明細ID（主キー）
    /// </summary>
    [Column("id")]
    [Key] // 主キーをマッピング
    public int Id { get; set; }

    /// <summary>
    /// 注文ID（外部キー）
    /// </summary>
    [Column("order_id")]
    public int OrderId { get; set; }

    /// <summary>
    /// 商品ID（外部キー）
    /// </summary>
    [Column("product_id")]
    public int ProductId { get; set; }

    /// <summary>
    /// 注文数
    /// </summary>
    [Column("count")]
    public int Count { get; set; }

    /// <summary>
    /// 注文
    /// </summary>
    [ForeignKey("OrderId")]
    public OrdersEntity Order { get; set; } = null!;

    /// <summary>
    /// 商品
    /// </summary>
    [ForeignKey("ProductId")]
    public ProductEntity Product { get; set; } = null!;

}