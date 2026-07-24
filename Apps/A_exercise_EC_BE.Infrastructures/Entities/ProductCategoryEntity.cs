using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace A_exercise_EC_BE.Infrastructures.Entities;
/// <summary>
/// product_categoryテーブルに対応するEntity Framework Coreのエンティティ
/// </summary>
[Table("product_category")]
public class ProductCategoryEntity
{
    [Column("id")]
    [Key] // 主キーをマッピング
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public int Id { get; set; }

    [Required] // NOT NUll
    [StringLength(36)] // データ長は36文字
    [Column("category_uuid")]// マッピングする列名
    public Guid CategoryUuid { get; set; }

    [Column("name")]
    [Required] // NOT NULL
    [StringLength(30)]// データ長は30文字
    // 列名と同じ名称のプロパティなので[Column]は使わない
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// カテゴリに属する商品(1:N)
    /// </summary>
    public List<ProductEntity> Products { get; set; } = new();

}