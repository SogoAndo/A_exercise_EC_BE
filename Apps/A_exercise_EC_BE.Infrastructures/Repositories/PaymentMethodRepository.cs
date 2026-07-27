using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructures.Repositories;

/// <summary>
/// 支払い方法Repository。
/// </summary>
public class PaymentMethodRepository(
    AppDbContext context,
    PaymentMethodEntityAdapter adapter)
    : IPaymentMethodRepository
{

    /// <inheritdoc />
    public async Task<List<PaymentMethod>> FindAllAsync()
    {
        try
        {
            var entities = await context.PaymentMethods
                .AsNoTracking()
                .OrderBy(
                    paymentMethod => paymentMethod.Id)
                .ToListAsync();

            var paymentMethods =
                new List<PaymentMethod>();

            foreach (var entity in entities)
            {
                paymentMethods.Add(
                    await adapter.RestoreAsync(entity));
            }

            return paymentMethods;
        }
        catch (Exception exception)
        {
            throw new InternalException(
                "支払い方法一覧取得中に予期しないエラーが発生しました。",
                exception);
        }
    }
    /// <inheritdoc />
    public async Task<PaymentMethod?> FindByIdAsync(
        int paymentMethodId)
    {
        try
        {
            var entity = await context.PaymentMethods
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    paymentMethod =>
                        paymentMethod.Id
                        == paymentMethodId);

            return entity is null
                ? null
                : await adapter.RestoreAsync(entity);
        }
        catch (Exception exception)
        {
            throw new InternalException(
                $"支払い方法ID:{paymentMethodId}"
                + "の支払い方法取得中に"
                + "予期しないエラーが発生しました。",
                exception);
        }
    }
}
