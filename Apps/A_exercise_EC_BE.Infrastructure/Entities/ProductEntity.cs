using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace A_exercise_EC_BE.Infrastructure.Entities;

/// <summary>
/// 商品テーブルのEntity
/// </summary>
[Table("product")]
public class ProductEntity
{
    [Column("id")]
    [Key] // 主キーをマッピング
    public int Id { get; set; }

    [Column("product_uuid")]
    public Guid ProductUuid { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("price")]
    public int Price { get; set; }

    [Column("image_url")]
    public string? ImageUrl { get; set; }

    [Column("product_category_id")]
    public int ProductCategoryId { get; set; }

    [Column("delete_flg")]
    public int DeleteFlg { get; set; }

    [ForeignKey("ProductCategoryId")]
    public ProductCategoryEntity ProductCategory { get; set; } = null!;

    // 在庫情報（1:1 関係を想定）
    public ProductStockEntity? ProductStock { get; set; }

    // 注文明細（1つの商品が多数の明細に入る）
    public List<OrdersDetailEntity> OrderDetails { get; set; } = new();

}