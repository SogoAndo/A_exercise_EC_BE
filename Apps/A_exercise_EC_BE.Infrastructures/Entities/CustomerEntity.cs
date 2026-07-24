using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace A_exercise_EC_BE.Infrastructures.Entities;

/// <summary>
/// 顧客テーブルのEntity
/// </summary>
[Table("customer")]
public class CustomerEntity
{
    /// <summary>
    /// 顧客ID（主キー）
    /// </summary>
    [Column("id")]
    [Key] // 主キーをマッピング
    public int Id { get; set; }

    /// <summary>
    /// 顧客識別ID（UUID）
    /// </summary>
    [Column("customer_uuid")]
    public Guid CustomerUuid { get; set; }

    /// <summary>
    /// 顧客名
    /// </summary>
    [Column("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 顧客名カナ
    /// </summary>
    [Column("kana")]
    public string Kana { get; set; } = string.Empty;

    /// <summary>
    /// 住所1
    /// </summary>
    [Column("address1")]
    public string Address1 { get; set; } = string.Empty;

    /// <summary>
    /// 住所2
    /// </summary>
    [Column("address2")]
    public string? Address2 { get; set; }

    /// <summary>
    /// 電話番号
    /// </summary>
    [Column("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    [Column("mail_address")]
    public string MailAddress { get; set; } = string.Empty;

    /// <summary>
    /// ユーザー名
    /// </summary>
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// パスワード
    /// </summary>
    [Column("password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 登録日時
    /// </summary>
    [Column("created_at", TypeName = "timestamp without time zone")]
    public DateTime CreatedAt { get; set; }

    public List<OrdersEntity> ListOrders { get; set; } = new();

}