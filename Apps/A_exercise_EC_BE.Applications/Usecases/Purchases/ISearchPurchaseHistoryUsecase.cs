using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Applications.Usecases.Purchases;

/// <summary>
/// UC007: 購入履歴閲覧UseCaseのインターフェース。
/// </summary>
public interface ISearchPurchaseHistoryUsecase
{
    /// <summary>
    /// 顧客に紐づく購入履歴を取得する。
    /// </summary>
    /// <param name="customerUuid">認証済み顧客のUUID。</param>
    /// <returns>購入履歴。履歴がない場合は空のリスト。</returns>
    Task<List<Orders>> SearchAsync(
        Guid customerUuid);

    /// <summary>
    /// 顧客自身の購入履歴詳細を取得する。
    /// </summary>
    /// <param name="customerUuid">認証済み顧客のUUID。</param>
    /// <param name="orderUuid">注文UUID。</param>
    /// <returns>
    /// 購入履歴詳細。注文が存在しない、または別顧客の注文である場合はnull。
    /// </returns>
    Task<Orders?> FindDetailAsync(
        Guid customerUuid,
        Guid orderUuid);
}
