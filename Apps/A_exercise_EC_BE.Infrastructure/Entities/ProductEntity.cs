using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_exercise_EC_BE.Infrastructure.Entities;

[Table("product")]
public class ProductEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_uuid")]
    public Guid ProductUuid { get; set; }

    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("price")]
    public int Price { get; set; }

    [Column("image_url")]
    [MaxLength(200)]
    public string? ImageUrl { get; set; }

    [Column("product_category_id")]
    public int ProductCategoryId { get; set; }

    [Column("delete_flg")]
    public int DeleteFlg { get; set; }

    public ProductCategoryEntity ProductCategory { get; set; } = null!;

    public ProductStockEntity? ProductStock { get; set; }
}
