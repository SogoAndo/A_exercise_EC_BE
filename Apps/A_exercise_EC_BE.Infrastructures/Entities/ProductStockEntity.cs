using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace A_exercise_EC_BE.Infrastructure.Entities;

/// <summary>
/// 商品在庫テーブルのEntity
/// </summary>
[Table("product_stock")]
public class ProductStockEntity
{
    /// <summary>
    /// 商品在庫ID（主キー）
    /// </summary>
    [Column("id")]
    [Key] // 主キーをマッピング
    public int Id { get; set; }

    /// <summary>
    /// 商品在庫識別ID（UUID）
    /// </summary>
    [Column("stock_uuid")]
    public Guid StockUuid { get; set; }

    /// <summary>
    /// 在庫数
    /// </summary>
    [Column("quantity")]
    public int Quantity { get; set; }

    /// <summary>
    /// 商品ID（外部キー）
    /// </summary>
    [Column("product_id")]
    public int ProductId { get; set; }

    /// <summary>
    /// 商品
    /// </summary>
    [ForeignKey("ProductId")]
    public ProductEntity Product { get; set; } = null!;

}