using A_exercise_EC_BE.Domains.Adapters;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Entities;

namespace A_exercise_EC_BE.Infrastructures.Adapters;

/// <summary>
/// ドメインオブジェクト:OrderStatusとOrderStatusEntityの相互変換クラス
/// </summary>
public class OrderStatusEntityAdapter :
    IConverter<OrderStatus, OrderStatusEntity>,
    IRestorer<OrderStatus, OrderStatusEntity>
{
    /// <summary>
    /// ドメインオブジェクト:OrderStatusをOrderStatusEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:OrderStatus</param>
    /// <returns>EFCore:OrderStatusEntity</returns>
    public Task<OrderStatusEntity> ConvertAsync(OrderStatus domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");

        // ドメインオブジェクト:OrderStatusをOrderStatusEntityに変換する
        var entity = new OrderStatusEntity();
        entity.Id = domain.Id;
        entity.Name = domain.Name;

        return Task.FromResult(entity);
    }

    /// <summary>
    /// OrderStatusEntityからドメインオブジェクト:OrderStatusを復元する
    /// </summary>
    /// <param name="target">EFCore:OrderStatusEntity</param>
    /// <returns>ドメインオブジェクト:OrderStatus</returns>
    public Task<OrderStatus> RestoreAsync(OrderStatusEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");

        // OrderStatusEntityからドメインオブジェクト:OrderStatusを復元する
        var domain = new OrderStatus(
            target.Id,
            target.Name
        );

        return Task.FromResult(domain);
    }
}