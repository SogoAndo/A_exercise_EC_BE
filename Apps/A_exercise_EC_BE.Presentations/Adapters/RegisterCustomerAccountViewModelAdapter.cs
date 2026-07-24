using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Presentations.ViewModels.Accounts;

namespace A_exercise_EC_BE.Presentations.Adapters;

/// <summary>
/// 顧客アカウント登録ViewModelを
/// Customerへ変換するAdapter
/// </summary>
public class RegisterCustomerAccountViewModelAdapter
{
    /// <summary>
    /// 顧客アカウント登録ViewModelを
    /// Customerへ変換する
    /// </summary>
    /// <param name="viewModel">
    /// 顧客アカウント登録画面の入力情報
    /// </param>
    /// <returns>
    /// 変換した顧客ドメインオブジェクト
    /// </returns>
    public Customer Convert(
        RegisterCustomerAccountViewModel viewModel)
    {
        _ = viewModel
            ?? throw new InternalException(
                "引数viewModelがnullです。");

        return new Customer(
            viewModel.Name.Trim(),
            viewModel.Kana.Trim(),
            viewModel.Address1.Trim(),
            NormalizeOptionalValue(
                viewModel.Address2),
            viewModel.PhoneNumber.Trim(),
            viewModel.MailAddress.Trim(),
            viewModel.Username.Trim(),
            viewModel.Password,
            DateTime.Now
        );
    }

    /// <summary>
    /// 任意入力項目を正規化する
    /// </summary>
    /// <param name="value">
    /// 入力値
    /// </param>
    /// <returns>
    /// 空白の場合null、それ以外は前後空白を除去した値
    /// </returns>
    private static string? NormalizeOptionalValue(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();
    }
}