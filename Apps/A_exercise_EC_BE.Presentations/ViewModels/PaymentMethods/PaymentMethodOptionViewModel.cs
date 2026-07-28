namespace A_exercise_EC_BE.Presentations.ViewModels.PaymentMethods;

/// <summary>
/// 支払い方法プルダウン項目。
/// </summary>
public class PaymentMethodOptionViewModel
{
    /// <summary>
    /// プルダウンの値。
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// プルダウンの表示名。
    /// </summary>
    public string Label { get; set; } =
        string.Empty;
}