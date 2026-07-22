using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_exercise_EC_BE.Infrastructure.Entities;

[Table("product_stock")]
public class ProductStockEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("stock_uuid")]
    public Guid StockUuid { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    public ProductEntity Product { get; set; } = null!;
}
