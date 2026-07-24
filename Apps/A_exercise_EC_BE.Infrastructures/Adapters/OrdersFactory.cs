using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Infrastructures.Entities;

namespace A_exercise_EC_BE.Infrastructures.Adapters;

/// <summary>
/// 注文、顧客、注文ステータス、支払い方法、注文明細の集約関係を構築するFactory
/// </summary>
public class OrdersFactory
{
    private readonly OrdersEntityAdapter _ordersEntityAdapter;
    private readonly CustomerEntityAdapter _customerEntityAdapter;
    private readonly OrderStatusEntityAdapter _orderStatusEntityAdapter;
    private readonly PaymentMethodEntityAdapter _paymentMethodEntityAdapter;
    private readonly OrdersDetailEntityAdapter _ordersDetailEntityAdapter;

    public OrdersFactory(
        OrdersEntityAdapter ordersEntityAdapter,
        CustomerEntityAdapter customerEntityAdapter,
        OrderStatusEntityAdapter orderStatusEntityAdapter,
        PaymentMethodEntityAdapter paymentMethodEntityAdapter,
        OrdersDetailEntityAdapter ordersDetailEntityAdapter)
    {
        _ordersEntityAdapter = ordersEntityAdapter;
        _customerEntityAdapter = customerEntityAdapter;
        _orderStatusEntityAdapter = orderStatusEntityAdapter;
        _paymentMethodEntityAdapter = paymentMethodEntityAdapter;
        _ordersDetailEntityAdapter = ordersDetailEntityAdapter;
    }

    /// <summary>
    /// OrdersドメインからOrdersEntityを生成する
    /// </summary>
    public async Task<OrdersEntity> ConvertAsync(Orders domain)
    {
        var entity = await _ordersEntityAdapter.ConvertAsync(domain);

        entity.Customer =
            await _customerEntityAdapter.ConvertAsync(domain.Customer);

        entity.OrderStatus =
            await _orderStatusEntityAdapter.ConvertAsync(domain.OrderStatus);

        entity.PaymentMethod =
            await _paymentMethodEntityAdapter.ConvertAsync(domain.PaymentMethod);

        if (domain.OrdersDetails is not null)
        {
            entity.OrderDetails = new List<OrdersDetailEntity>();

            foreach (var detail in domain.OrdersDetails)
            {
                entity.OrderDetails.Add(
                    await _ordersDetailEntityAdapter.ConvertAsync(detail));
            }
        }

        return entity;
    }

    /// <summary>
    /// OrdersEntityからOrdersドメインを復元する
    /// </summary>
    public async Task<Orders> RestoreAsync(OrdersEntity target)
    {
        var customer =
            await _customerEntityAdapter.RestoreAsync(target.Customer);

        var orderStatus =
            await _orderStatusEntityAdapter.RestoreAsync(target.OrderStatus);

        var paymentMethod =
            await _paymentMethodEntityAdapter.RestoreAsync(target.PaymentMethod);

        var details = new List<OrdersDetail>();

        foreach (var detailEntity in target.OrderDetails)
        {
            details.Add(await _ordersDetailEntityAdapter.RestoreAsync(detailEntity));
        }

        var order = new Orders(
            target.OrderUuid,
            target.OrderDate,
            target.AmountTotal,
            customer,
            orderStatus,
            paymentMethod,
            details
        );

        return order;
    }

    /// <summary>
    /// OrdersEntityリストからOrdersリストを復元する
    /// </summary>
    public async Task<List<Orders>> RestoreAsync(List<OrdersEntity> targets)
    {
        var orders = new List<Orders>();

        foreach (var target in targets)
        {
            orders.Add(await RestoreAsync(target));
        }

        return orders;
    }
}