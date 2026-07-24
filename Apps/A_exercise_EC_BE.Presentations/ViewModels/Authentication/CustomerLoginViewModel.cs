using System.ComponentModel.DataAnnotations;

namespace A_exercise_EC_BE.Presentations.ViewModels.Authentication;

/// <summary>
/// UC002 顧客ログインの入力情報。
/// </summary>
public sealed class CustomerLoginViewModel
{
    /// <summary>
    /// メールアドレス。
    /// </summary>
    [Required(
        ErrorMessage =
            "メールアドレスを入力してください。")]
    [EmailAddress(
        ErrorMessage =
            "正しいメールアドレス形式で入力してください。")]
    public string MailAddress { get; init; } =
        string.Empty;

    /// <summary>
    /// パスワード。
    /// </summary>
    [Required(
        ErrorMessage =
            "パスワードを入力してください。")]
    [StringLength(
        20,
        MinimumLength = 5,
        ErrorMessage =
            "パスワードは5～20文字で入力してください。")]
    public string Password { get; init; } =
        string.Empty;
}
