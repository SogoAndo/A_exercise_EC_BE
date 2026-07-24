using Microsoft.EntityFrameworkCore;
using A_exercise_EC_BE.Domain.Adapters;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Domain.Repositories;
using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Contexts;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Repositories;

/// <summary>
/// 注文Repository。
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    private readonly OrdersEntityAdapter _adapter;

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
        OrdersEntityAdapter adapter
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

    /// <summary>
    /// 顧客UUIDに紐づく購入履歴一覧を取得する。
    /// </summary>
    /// <param name="customerUuid">
    /// 顧客UUID
    /// </param>
    /// <returns>
    /// 購入履歴一覧
    /// </returns>
    public async Task<List<Orders>>
        FindByCustomerUuidAsync(
            Guid customerUuid
        )
    {
        try
        {
            var entities =
                await _context.Orders
                    .Where(order =>
                        order.Customer.CustomerUuid
                        == customerUuid
                    )
                    .OrderByDescending(order =>
                        order.OrderDate)
                    .ToListAsync();

            var orders =
                new List<Orders>();

            foreach (var entity in entities)
            {
                orders.Add(
                    await _adapter.RestoreAsync(
                        entity
                    )
                );
            }

            return orders;
        }
        catch (Exception ex)
        {
            throw new InternalException(
                "購入履歴一覧の取得中に予期しないエラーが発生しました。",
                ex
            );
        }
    }

    /// <summary>
    /// 注文UUIDを指定して注文詳細を取得する。
    /// </summary>
    /// <param name="orderUuid">注文UUID</param>
    /// <returns>
    /// 注文詳細。
    /// 対象の注文が存在しない場合はnull。
    /// </returns>
    public async Task<Orders?> FindByOrderUuidAsync(
        Guid orderUuid
    )
    {
        try
        {
            var entity = await _context.Orders
                .AsNoTracking()
                .Include(order =>
                    order.OrderDetails
                )
                .ThenInclude(detail =>
                    detail.Product
                )
                .SingleOrDefaultAsync(order =>
                    order.OrderUuid == orderUuid
                );

            if (entity is null)
            {
                return null;
            }

            return await _adapter.RestoreAsync(
                entity
            );
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException(
                "購入履歴詳細の取得中に予期しないエラーが発生しました。",
                ex
            );
        }
    }
}