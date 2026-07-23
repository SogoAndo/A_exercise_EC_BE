using A_exercise_EC_BE.Domain.Models;

namespace A_exercise_EC_BE.Domain.Repositories;

/// <summary>
/// 注文Repositoryのインターフェース。
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// 注文を永続化する。
    /// </summary>
    /// <param name="order">
    /// 永続化する注文
    /// </param>
    /// <returns>なし</returns>
    Task CreateAsync(
        Orders order
    );
}