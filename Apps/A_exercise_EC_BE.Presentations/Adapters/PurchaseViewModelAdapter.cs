using System.Globalization;
using A_exercise_EC_BE.Applications.Usecases.Purchases;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Presentations.ViewModels.Purchases;

namespace A_exercise_EC_BE.Presentations.Adapters;

/// <summary>
/// UC005のViewModelとApplication層の
/// 入出力を相互変換する。
/// </summary>
public sealed class PurchaseViewModelAdapter
{
    private const string CompleteMessage =
        "購入が完了しました";

    /// <summary>
    /// 購入確定ViewModelをUseCaseの入力へ変換する。
    /// </summary>
    /// <param name="customerUuid">
    /// 認証済み顧客のUUID
    /// </param>
    /// <param name="viewModel">
    /// 購入確定ViewModel
    /// </param>
    /// <returns>
    /// 購入確定UseCaseの入力
    /// </returns>
    public ConfirmPurchaseRequest ConvertToRequest(
        Guid customerUuid,
        ConfirmPurchaseViewModel viewModel)
    {
        _ = viewModel
            ?? throw new InternalException(
                "引数viewModelがnullです。");

        var items = viewModel.Items?
            .Select(
                item =>
                    new ConfirmPurchaseItemRequest(
                        item.ProductUuid,
                        item.Quantity))
            .ToList()
            ?? [];

        return new ConfirmPurchaseRequest(
            customerUuid,
            viewModel.PaymentMethodId,
            items);
    }

    /// <summary>
    /// 購入確定結果を完了時の応答へ変換する。
    /// </summary>
    /// <param name="result">
    /// 購入確定結果
    /// </param>
    /// <returns>
    /// 購入完了時の応答
    /// </returns>
    public PurchaseCompleteViewModel
        ConvertToCompleteViewModel(
            ConfirmPurchaseResult result)
    {
        _ = result
            ?? throw new InternalException(
                "引数resultがnullです。");

        return new PurchaseCompleteViewModel(
            CompleteMessage,
            result.OrderUuid,
            result.OrderDate.ToString(
                "yyyy/MM/dd HH:mm:ss",
                CultureInfo.InvariantCulture),
            result.AmountTotal);
    }
}
