using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructures.Repositories;

/// <summary>
/// 顧客ログインで使用する顧客アカウントRepository。
/// </summary>
public class CustomerRepository(
    AppDbContext _context,
    CustomerEntityAdapter _adapter) : ICustomerRepository
{
    public async Task<Customer?> FindByMailAddressAsync(string mailAddress)
    {
        try
        {
            var entity = await _context.Customers
                .AsNoTracking()
                .SingleOrDefaultAsync(customer => customer.MailAddress == mailAddress);

            return entity is null
                ? null
                : await _adapter.RestoreAsync(entity);
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

    /// <summary>
    /// アカウント名が既に存在するかを確認する
    /// </summary>
    /// <param name="accountName">アカウント名</param>
    /// <returns>存在する場合true</returns>
    public async Task<bool> ExistsByUsernameAsync(string accountName)
    {
        try
        {
            return await _context.Customers
                .AsNoTracking()
                .AnyAsync(a => a.Username == accountName);
        }
        catch (Exception ex)
        {
            throw new InternalException(
                $"アカウント名:{accountName}の存在確認中に予期しないエラーが発生しました。",
                ex);
        }
    }

    /// <summary>
    /// メールアドレスが既に存在するかを確認する
    /// </summary>
    /// <param name="accountName">アカウント名</param>
    /// <returns>存在する場合true</returns>
    public async Task<bool> ExistsByMailAddressAsync(string mailAddress)
    {
        try
        {
            return await _context.Customers
                .AsNoTracking()
                .AnyAsync(a => a.MailAddress == mailAddress);
        }
        catch (Exception ex)
        {
            throw new InternalException(
                $"メールアドレス:{mailAddress}の存在確認中に予期しないエラーが発生しました。",
                ex);
        }
    }

    /// <summary>
    /// 顧客アカウントを永続化する
    /// </summary>
    /// <param name="customer">永続化する顧客アカウント</param>
    /// <returns>なし</returns>
    public async Task CreateAsync(
        Customer customer
    )
    {
        try
        {
            // CustomerをCustomerEntityに変換する
            var entity =
                await _adapter.ConvertAsync(
                    customer
                );

            // 顧客アカウントを登録する
            await _context.Customers.AddAsync(
                entity
            );

            // データベースに反映する
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new InternalException(
                "顧客アカウントの永続化中に予期しないエラーが発生しました。",
                ex
            );
        }
    }
}
