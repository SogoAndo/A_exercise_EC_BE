using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructures.Repositories;

/// <summary>
/// 注文ステータスRepository。
/// </summary>
public class OrderStatusRepository(
    AppDbContext context,
    OrderStatusEntityAdapter adapter)
    : IOrderStatusRepository
{
    /// <inheritdoc />
    public async Task<OrderStatus?> FindByNameAsync(
        string name)
    {
        try
        {
            var entity = await context.OrderStatuses
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    orderStatus =>
                        orderStatus.Name
                        == name);

            return entity is null
                ? null
                : await adapter.RestoreAsync(entity);
        }
        catch (Exception exception)
        {
            throw new InternalException(
                $"注文ステータス名:{name}"
                + "の注文ステータス取得中に"
                + "予期しないエラーが発生しました。",
                exception);
        }
    }
}
