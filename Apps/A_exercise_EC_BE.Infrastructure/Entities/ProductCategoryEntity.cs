using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_exercise_EC_BE.Infrastructure.Entities;

[Table("product_category")]
public class ProductCategoryEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("category_uuid")]
    public Guid CategoryUuid { get; set; }

    [Column("name")]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    public List<ProductEntity> Products { get; set; } = [];
}
