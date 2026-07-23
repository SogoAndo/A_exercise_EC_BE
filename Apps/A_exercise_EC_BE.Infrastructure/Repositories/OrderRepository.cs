using A_exercise_EC_BE.Domain.Adapters;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Domain.Repositories;
using A_exercise_EC_BE.Infrastructure.Contexts;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Repositories;

/// <summary>
/// 注文Repository。
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    private readonly IConverter<
        Orders,
        OrdersEntity
    > _adapter;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="context">
    /// DBコンテキスト
    /// </param>
    /// <param name="adapter">
    /// 注文変換Adapter
    /// </param>
    public OrderRepository(
        AppDbContext context,
        IConverter<Orders, OrdersEntity> adapter
    )
    {
        _context = context;
        _adapter = adapter;
    }

    /// <inheritdoc />
    public async Task CreateAsync(
        Orders order
    )
    {
        try
        {
            _ = order
                ?? throw new InternalException(
                    "永続化する注文がnullです。"
                );

            var entity =
                await _adapter.ConvertAsync(
                    order
                );

            await _context.Orders.AddAsync(
                entity
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
                "注文の永続化中に予期しないエラーが発生しました。",
                ex
            );
        }
    }
}