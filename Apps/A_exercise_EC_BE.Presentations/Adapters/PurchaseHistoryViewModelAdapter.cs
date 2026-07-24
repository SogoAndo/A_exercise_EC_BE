using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Presentations.ViewModels.Purchases;

namespace A_exercise_EC_BE.Presentations.Adapters;

/// <summary>
/// 注文ドメインオブジェクトを購入履歴ViewModelへ変換する。
/// </summary>
public sealed class PurchaseHistoryViewModelAdapter
{
    private const string EmptyHistoryMessage =
        "購入履歴がありません。";

    /// <summary>
    /// 注文一覧を購入履歴一覧レスポンスへ変換する。
    /// </summary>
    public PurchaseHistoryResultViewModel ConvertToResultViewModel(
        List<Orders> orders)
    {
        _ = orders
            ?? throw new InternalException(
                "引数ordersがnullです。");

        var orderList = orders
            .Select(ConvertToListItemViewModel)
            .ToList();

        return new PurchaseHistoryResultViewModel(
            orderList,
            orderList.Count == 0
                ? EmptyHistoryMessage
                : null);
    }

    /// <summary>
    /// 注文を購入履歴詳細レスポンスへ変換する。
    /// </summary>
    public PurchaseHistoryDetailViewModel ConvertToDetailViewModel(
        Orders order)
    {
        _ = order
            ?? throw new InternalException(
                "引数orderがnullです。");

        return new PurchaseHistoryDetailViewModel(
            order.OrderUuid,
            FormatOrderDate(order.OrderDate),
            order.OrderStatus.Name,
            order.OrdersDetails
                .Select(ConvertToOrderItemViewModel)
                .ToList(),
            order.AmountTotal);
    }

    private static PurchaseHistoryListItemViewModel
        ConvertToListItemViewModel(
            Orders order)
    {
        _ = order
            ?? throw new InternalException(
                "注文情報がnullです。");

        return new PurchaseHistoryListItemViewModel(
            order.OrderUuid,
            FormatOrderDate(order.OrderDate),
            order.OrderStatus.Name,
            order.AmountTotal,
            $"/purchase/history/{order.OrderUuid:D}");
    }

    private static CartItemViewModel
        ConvertToOrderItemViewModel(
            OrdersDetail orderDetail)
    {
        _ = orderDetail
            ?? throw new InternalException(
                "注文明細がnullです。");

        int subtotal;
        try
        {
            subtotal = checked(
                orderDetail.Product.Price
                * orderDetail.Count);
        }
        catch (OverflowException exception)
        {
            throw new InternalException(
                "注文明細の小計を計算できません。",
                exception);
        }

        return new CartItemViewModel(
            orderDetail.Product.ProductUuid,
            orderDetail.Product.Name,
            orderDetail.Product.Price,
            orderDetail.Count,
            subtotal);
    }

    private static string FormatOrderDate(
        DateTime orderDate)
        => orderDate.ToString(
            "yyyy/MM/dd HH:mm:ss");
}
