using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Domain.Repositories;
using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructure.Repositories;

/// <summary>
/// 顧客ログインで使用する顧客アカウントRepository。
/// </summary>
public class CustomerRepository(
    AppDbContext context,
    CustomerEntityAdapter adapter) : ICustomerRepository
{
    public async Task<Customer?> FindByMailAddressAsync(string mailAddress)
    {
        try
        {
            var entity = await context.Customers
                .AsNoTracking()
                .SingleOrDefaultAsync(customer => customer.MailAddress == mailAddress);

            return entity is null
                ? null
                : await adapter.RestoreAsync(entity);
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new InternalException(
                "顧客アカウントの取得中に予期しないエラーが発生しました。",
                exception);
        }
    }
}
