using Microsoft.EntityFrameworkCore;
using A_exercise_EC_BE.Domains.Adapters;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Entities;

namespace A_exercise_EC_BE.Infrastructures.Repositories;

/// <summary>
/// 注文Repository。
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    private readonly OrdersFactory _factory;
    private readonly OrdersEntityAdapter _adapter;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="context">
    /// DBコンテキスト
    /// </param>
    /// <param name="factory">
    /// 注文変換Adapter
    /// </param>
    public OrderRepository(
        AppDbContext context,
        OrdersFactory factory,
        OrdersEntityAdapter adapter
    )
    {
        _context = context;
        _factory = factory;
        _adapter = adapter;
    }

    public async Task CreateAsync(
    Orders order)
    {
        try
        {
            _ = order
                ?? throw new InternalException(
                    "永続化する注文がnullです。");

            /*
             * 顧客UUIDから、DB内部の顧客IDを取得する。
             */
            var customerEntity =
                await _context.Customers
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        customer =>
                            customer.CustomerUuid
                            == order.Customer.CustomerUuid);

            if (customerEntity is null)
            {
                throw new InternalException(
                    $"顧客UUID:"
                    + $"{order.Customer.CustomerUuid}"
                    + "の顧客が存在しません。");
            }

            /*
             * Orders本体だけをEntityへ変換する。
             */
            var entity =
                await _adapter.ConvertAsync(
                    order);

            /*
             * Adapterでは設定できない顧客の内部IDを補う。
             */
            entity.CustomerId =
                customerEntity.Id;

            /*
             * OrdersEntityだけを追加する。
             *
             * Customer、OrderStatus、PaymentMethodを
             * Navigationへ設定しないため、
             * 既存データが再INSERTされない。
             */
            await _context.Orders.AddAsync(
                entity);

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
                ex);
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
        .AsNoTracking()
        .Include(order =>
            order.Customer)
        .Include(order =>
            order.OrderStatus)
        .Include(order =>
            order.PaymentMethod)
        .Include(order =>
            order.OrderDetails)
        .ThenInclude(detail =>
            detail.Product)
        .Where(order =>
            order.Customer.CustomerUuid
            == customerUuid)
        .OrderByDescending(order =>
            order.OrderDate)
        .ToListAsync();

            return await _factory.RestoreAsync(
    entities);
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
            order.Customer)
        .Include(order =>
            order.OrderStatus)
        .Include(order =>
            order.PaymentMethod)
        .Include(order =>
            order.OrderDetails)
        .ThenInclude(detail =>
            detail.Product)
        .SingleOrDefaultAsync(order =>
            order.OrderUuid
            == orderUuid);

            if (entity is null)
            {
                return null;
            }

            return await _factory.RestoreAsync(
                entity
            );
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