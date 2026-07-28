namespace A_exercise_EC_BE.Presentations.ViewModels.Accounts;

/// <summary>
/// 顧客アカウント登録完了画面の表示情報
/// </summary>
public class RegisterCustomerAccountCompleteViewModel
{
    /// <summary>
    /// 画面タイトル
    /// </summary>
    public string Title { get; set; } =
        string.Empty;

    /// <summary>
    /// 完了メッセージ
    /// </summary>
    public string Message { get; set; } =
        string.Empty;

    /// <summary>
    /// 顧客識別ID
    /// </summary>
    public Guid CustomerUuid { get; set; }

    /// <summary>
    /// 顧客名
    /// </summary>
    public string Name { get; set; } =
        string.Empty;

    /// <summary>
    /// アカウント名
    /// </summary>
    public string Username { get; set; } =
        string.Empty;

    /// <summary>
    /// 登録日時
    /// </summary>
    public DateTime CreatedAt { get; set; }
}