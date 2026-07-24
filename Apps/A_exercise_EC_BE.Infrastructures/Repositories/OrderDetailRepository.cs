using A_exercise_EC_BE.Domains.Adapters;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Entities;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructures.Repositories;

/// <summary>
/// 注文明細Repository。
/// </summary>
public class OrderDetailRepository
    : IOrderDetailRepository
{
    private readonly AppDbContext _context;

    private readonly IConverter<
        OrdersDetail,
        OrdersDetailEntity
    > _adapter;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="context">
    /// DBコンテキスト
    /// </param>
    /// <param name="adapter">
    /// 注文明細変換Adapter
    /// </param>
    public OrderDetailRepository(
        AppDbContext context,
        IConverter<
            OrdersDetail,
            OrdersDetailEntity
        > adapter
    )
    {
        _context = context;
        _adapter = adapter;
    }

    public async Task CreateRangeAsync(
    int orderId,
    List<OrdersDetail> orderDetails)
    {
        try
        {
            _ = orderDetails
                ?? throw new InternalException(
                    "永続化する注文明細がnullです。");

            if (orderDetails.Count == 0)
            {
                throw new InternalException(
                    "永続化する注文明細が存在しません。");
            }

            var entities =
                new List<OrdersDetailEntity>();

            foreach (var orderDetail
                in orderDetails)
            {
                var productEntity =
                    await _context.Products
                        .SingleOrDefaultAsync(
                            product =>
                                product.ProductUuid
                                == orderDetail.Product
                                    .ProductUuid);

                if (productEntity is null)
                {
                    throw new InternalException(
                        $"商品UUID:"
                        + $"{orderDetail.Product.ProductUuid}"
                        + "の商品が存在しません。");
                }

                var entity =
                    await _adapter.ConvertAsync(
                        orderDetail);

                entity.OrderId =
                    orderId;

                entity.ProductId =
                    productEntity.Id;

                await _context.OrdersDetails.AddAsync(entity);
            }

            await _context.SaveChangesAsync();
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException(
                "注文明細の永続化中に予期しないエラーが発生しました。",
                ex);
        }
    }
}