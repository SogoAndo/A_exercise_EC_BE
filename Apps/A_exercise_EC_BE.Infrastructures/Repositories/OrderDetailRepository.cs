using A_exercise_EC_BE.Domain.Adapters;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Domain.Repositories;
using A_exercise_EC_BE.Infrastructure.Contexts;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Repositories;

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

    /// <inheritdoc />
    public async Task CreateRangeAsync(
        IReadOnlyCollection<OrdersDetail>
            orderDetails
    )
    {
        try
        {
            _ = orderDetails
                ?? throw new InternalException(
                    "永続化する注文明細がnullです。"
                );

            if (orderDetails.Count == 0)
            {
                throw new InternalException(
                    "永続化する注文明細が存在しません。"
                );
            }

            var entities =
                new List<OrdersDetailEntity>();

            foreach (var orderDetail
                in orderDetails)
            {
                var entity =
                    await _adapter.ConvertAsync(
                        orderDetail
                    );

                entities.Add(entity);
            }

            await _context.OrdersDetails
                .AddRangeAsync(
                    entities
                );

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
                ex
            );
        }
    }
}