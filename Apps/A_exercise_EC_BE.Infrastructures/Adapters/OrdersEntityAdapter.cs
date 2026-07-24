using A_exercise_EC_BE.Domain.Adapters;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Adapters;

/// <summary>
/// ドメインオブジェクト:OrdersとOrdersEntityの相互変換クラス
/// </summary>
public class OrdersEntityAdapter :
    IConverter<Orders, OrdersEntity>,
    IRestorer<Orders, OrdersEntity>
{
    /// <summary>
    /// ドメインオブジェクト:OrdersをOrdersEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:Orders</param>
    /// <returns>EFCore:OrdersEntity</returns>
    public Task<OrdersEntity> ConvertAsync(Orders domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");

        // ドメインオブジェクト:OrdersをOrdersEntityに変換する
        var entity = new OrdersEntity();
        entity.OrderUuid = domain.OrderUuid;
        entity.OrderDate = domain.OrderDate;
        entity.AmountTotal = domain.AmountTotal;
        entity.OrderStatusId = domain.OrderStatus.Id;
        entity.PaymentMethodId = domain.PaymentMethod.Id;

        return Task.FromResult(entity);
    }

    /// <summary>
    /// OrdersEntityからドメインオブジェクト:Ordersを復元する
    /// </summary>
    /// <param name="target">EFCore:OrdersEntity</param>
    /// <returns>ドメインオブジェクト:Orders</returns>
    public async Task<Orders> RestoreAsync(OrdersEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");

        var customer = await new CustomerEntityAdapter().RestoreAsync(target.Customer);
        var orderStatus = await new OrderStatusEntityAdapter().RestoreAsync(target.OrderStatus);
        var paymentMethod = await new PaymentMethodEntityAdapter().RestoreAsync(target.PaymentMethod);
        var details = new List<OrdersDetail>();

        foreach (var detailEntity in target.OrderDetails)
        {
            details.Add(await new OrdersDetailEntityAdapter().RestoreAsync(detailEntity));
        }

        // OrdersEntityからドメインオブジェクト:Ordersを復元する
        var domain = new Orders(
            target.OrderUuid,
            target.OrderDate,
            target.AmountTotal,
            customer,
            orderStatus,
            paymentMethod,
            details
        );

        return domain;
    }
}