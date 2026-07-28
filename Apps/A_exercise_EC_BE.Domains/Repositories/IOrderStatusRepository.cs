using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Repositories;

/// <summary>
/// 注文ステータスRepositoryのインターフェース。
/// </summary>
public interface IOrderStatusRepository
{
    /// <summary>
    /// 注文ステータス名に一致する
    /// 注文ステータスを取得する。
    /// </summary>
    /// <param name="name">
    /// 注文ステータス名
    /// </param>
    /// <returns>
    /// 一致する注文ステータス。
    /// 存在しない場合はnull。
    /// </returns>
    Task<OrderStatus?> FindByNameAsync(
        string name);
}
