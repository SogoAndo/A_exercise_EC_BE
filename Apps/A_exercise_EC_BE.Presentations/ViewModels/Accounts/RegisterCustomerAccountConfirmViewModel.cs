namespace A_exercise_EC_BE.Presentations.ViewModels.Accounts;

/// <summary>
/// 顧客アカウント登録確認画面の表示情報
/// </summary>
public class RegisterCustomerAccountConfirmViewModel
{
    /// <summary>
    /// 画面タイトル
    /// </summary>
    public string Title { get; set; } =
        string.Empty;

    /// <summary>
    /// 顧客名
    /// </summary>
    public string Name { get; set; } =
        string.Empty;

    /// <summary>
    /// 顧客名カナ
    /// </summary>
    public string Kana { get; set; } =
        string.Empty;

    /// <summary>
    /// 住所1
    /// </summary>
    public string Address1 { get; set; } =
        string.Empty;

    /// <summary>
    /// 住所2
    /// </summary>
    public string? Address2 { get; set; }

    /// <summary>
    /// 電話番号
    /// </summary>
    public string PhoneNumber { get; set; } =
        string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    public string MailAddress { get; set; } =
        string.Empty;

    /// <summary>
    /// アカウント名
    /// </summary>
    public string Username { get; set; } =
        string.Empty;

    /// <summary>
    /// マスクされたパスワード
    /// </summary>
    public string PasswordMask { get; set; } =
        string.Empty;
}