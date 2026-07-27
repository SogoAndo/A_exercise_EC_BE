using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace A_exercise_EC_BE.Presentations.ViewModels.Accounts;

/// <summary>
/// 顧客アカウント登録画面の入力情報
/// </summary>
public class RegisterCustomerAccountViewModel
{
    /// <summary>
    /// 顧客名
    /// </summary>
    [Required(
        ErrorMessage =
            "氏名を入力してください")]
    [StringLength(
        20,
        MinimumLength = 2,
        ErrorMessage =
            "氏名は2文字以上20文字以内で入力してください")]
    public string Name { get; set; } =
        string.Empty;

    /// <summary>
    /// 顧客名カナ
    /// </summary>
    [Required(
        ErrorMessage =
            "氏名カナを入力してください")]
    [StringLength(
        20,
        MinimumLength = 2,
        ErrorMessage =
            "氏名カナは2文字以上20文字以内で入力してください")]
    [RegularExpression(
    "^[ァ-ヶー・ 　]+$",
    ErrorMessage =
        "顧客名カナは全角カナで入力してください")]
    public string Kana { get; set; } =
        string.Empty;

    /// <summary>
    /// 住所1
    /// </summary>
    [Required(
        ErrorMessage =
            "住所1を入力してください")]
    [StringLength(
        100,
        ErrorMessage =
            "住所1は100文字以内で入力してください")]
    public string Address1 { get; set; } =
        string.Empty;

    /// <summary>
    /// 住所2
    /// </summary>
    [StringLength(
        100,
        ErrorMessage =
            "住所2は100文字以内で入力してください")]
    public string? Address2 { get; set; }

    /// <summary>
    /// 電話番号
    /// </summary>
    [Required(
        ErrorMessage =
            "電話番号を入力してください")]
    [StringLength(
        20,
        ErrorMessage =
            "電話番号は20文字以内で入力してください")]
    public string PhoneNumber { get; set; } =
        string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    [FromQuery]
    [Required(
        ErrorMessage =
            "メールアドレスを入力してください")]
    [StringLength(
        200,
        MinimumLength = 4,
        ErrorMessage =
            "メールアドレスは4文字以上200文字以内で入力してください")]
    [EmailAddress(
        ErrorMessage =
            "メールアドレスの形式が正しくありません")]
    public string MailAddress { get; set; } =
        string.Empty;

    /// <summary>
    /// アカウント名
    /// </summary>
    [FromQuery]
    [Required(
        ErrorMessage =
            "アカウント名を入力してください")]
    [StringLength(
        30,
        MinimumLength = 5,
        ErrorMessage =
            "アカウント名は5文字以上30文字以内で入力してください")]
    [RegularExpression(
        "^(?!([a-zA-Z0-9])\\1+$)[a-zA-Z0-9]+$",
        ErrorMessage = "アカウント名は半角英数字で入力し、同じ文字のみの登録はできません")]
    public string Username { get; set; } =
        string.Empty;

    /// <summary>
    /// パスワード
    /// </summary>
    [Required(
        ErrorMessage =
            "パスワードを入力してください")]
    [StringLength(
        20,
        MinimumLength = 5,
        ErrorMessage =
            "パスワードは5文字以上20文字以内で入力してください")]
    [RegularExpression(
        "^(?!([a-zA-Z0-9])\\1+$)[a-zA-Z0-9]+$",
        ErrorMessage = "パスワードは半角英数字で入力し、同じ文字のみの登録はできません")]
    [DataType(
        DataType.Password)]
    public string Password { get; set; } =
        string.Empty;
}