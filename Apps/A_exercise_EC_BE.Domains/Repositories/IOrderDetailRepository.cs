using A_exercise_EC_BE.Domain.Models;

namespace A_exercise_EC_BE.Domain.Repositories;

/// <summary>
/// 注文明細Repositoryのインターフェース。
/// </summary>
public interface IOrderDetailRepository
{
    /// <summary>
    /// 複数の注文明細をまとめて永続化する。
    /// </summary>
    /// <param name="orderDetails">
    /// 永続化する注文明細
    /// </param>
    /// <returns>なし</returns>
    Task CreateRangeAsync(
        IReadOnlyCollection<OrdersDetail>
            orderDetails
    );
}