using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace A_exercise_EC_BE.Infrastructure.Entities;

/// <summary>
/// 管理側と共有する顧客テーブルのEntity。
/// </summary>
[Table("customer")]
public class CustomerEntity
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("customer_uuid")]
    public Guid CustomerUuid { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("kana")]
    public string? Kana { get; set; }

    [Column("address1")]
    public string Address1 { get; set; } = string.Empty;

    [Column("address2")]
    public string? Address2 { get; set; }

    [Column("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Column("mail_address")]
    public string MailAddress { get; set; } = string.Empty;

    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Column("password")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }
}
