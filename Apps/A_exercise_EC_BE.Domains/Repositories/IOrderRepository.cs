using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Repositories;

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
    /// <returns>
    /// 登録した注文のDB内部ID。
    /// 注文明細との関連付けにのみ使用する。
    /// </returns>
    Task<int> CreateAsync(
        Orders order
    );

    Task<List<Orders>>
       FindByCustomerUuidAsync(
           Guid customerUuid
       );

    /// <summary>
    /// 注文UUIDを指定して注文詳細を取得する。
    /// </summary>
    /// <param name="orderUuid">注文UUID</param>
    /// <returns>
    /// 注文詳細。
    /// 対象の注文が存在しない場合はnull。
    /// </returns>
    Task<Orders?> FindByOrderUuidAsync(
        Guid orderUuid
    );
}
