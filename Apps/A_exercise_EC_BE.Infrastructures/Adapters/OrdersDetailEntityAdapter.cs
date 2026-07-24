using A_exercise_EC_BE.Domains.Adapters;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Entities;

namespace A_exercise_EC_BE.Infrastructures.Adapters;

/// <summary>
/// ドメインオブジェクト:OrderDetailとOrderDetailEntityの相互変換クラス
/// </summary>
public class OrdersDetailEntityAdapter :
    IConverter<OrdersDetail, OrdersDetailEntity>,
    IRestorer<OrdersDetail, OrdersDetailEntity>
{
    /// <summary>
    /// ドメインオブジェクト:OrderDetailをOrderDetailEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:OrderDetail</param>
    /// <returns>EFCore:OrderDetailEntity</returns>
    public Task<OrdersDetailEntity> ConvertAsync(OrdersDetail domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");

        // ドメインオブジェクト:OrderDetailをOrderDetailEntityに変換する
        var entity = new OrdersDetailEntity();
        entity.Count = domain.Count;

        return Task.FromResult(entity);
    }

    /// <summary>
    /// OrderDetailEntityからドメインオブジェクト:OrderDetailを復元する
    /// </summary>
    /// <param name="target">EFCore:OrderDetailEntity</param>
    /// <returns>ドメインオブジェクト:OrderDetail</returns>
    public async Task<OrdersDetail> RestoreAsync(OrdersDetailEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");

        if (target.Product is null)
        {
            throw new InternalException("注文明細の商品が取得できていません。");
        }

        var product = await new ProductEntityAdapter().RestoreAsync(target.Product);

        // OrderDetailEntityからドメインオブジェクト:OrderDetailを復元する
        var domain = new OrdersDetail(
            target.Id,
            product,
            target.Count
        );

        return domain;
    }
}