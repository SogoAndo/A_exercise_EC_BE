using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Presentations.ViewModels.Accounts;

namespace A_exercise_EC_BE.Presentations.Adapters;

/// <summary>
/// 顧客アカウント登録ViewModelを
/// 顧客ドメインオブジェクトなどへ変換するAdapter
/// </summary>
public class RegisterCustomerAccountViewModelAdapter
{
    /// <summary>
    /// 入力用ViewModelをCustomerへ変換する
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
    /// 入力用ViewModelを確認画面用ViewModelへ変換する
    /// </summary>
    /// <param name="viewModel">
    /// 顧客アカウント登録画面の入力情報
    /// </param>
    /// <returns>
    /// 確認画面の表示情報
    /// </returns>
    public RegisterCustomerAccountConfirmViewModel
        ToConfirmViewModel(
            RegisterCustomerAccountViewModel viewModel)
    {
        _ = viewModel
            ?? throw new InternalException(
                "引数viewModelがnullです。");

        return new RegisterCustomerAccountConfirmViewModel
        {
            Title =
                "顧客アカウント登録(確認)",

            Name =
                viewModel.Name.Trim(),

            Kana =
                viewModel.Kana.Trim(),

            Address1 =
                viewModel.Address1.Trim(),

            Address2 =
                NormalizeOptionalValue(
                    viewModel.Address2),

            PhoneNumber =
                viewModel.PhoneNumber.Trim(),

            MailAddress =
                viewModel.MailAddress.Trim(),

            Username =
                viewModel.Username.Trim(),

            PasswordMask =
                "********"
        };
    }

    /// <summary>
    /// Customerを完了画面用ViewModelへ変換する
    /// </summary>
    /// <param name="customer">
    /// 登録した顧客
    /// </param>
    /// <returns>
    /// 完了画面の表示情報
    /// </returns>
    public RegisterCustomerAccountCompleteViewModel
        ToCompleteViewModel(
            Customer customer)
    {
        _ = customer
            ?? throw new InternalException(
                "引数customerがnullです。");

        return new RegisterCustomerAccountCompleteViewModel
        {
            Title =
                "顧客アカウント登録(完了)",

            Message =
                "顧客アカウントの登録が完了しました。",

            CustomerUuid =
                customer.CustomerUuid,

            Name =
                customer.Name,

            Username =
                customer.Username,

            CreatedAt =
                customer.CreatedAt
        };
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