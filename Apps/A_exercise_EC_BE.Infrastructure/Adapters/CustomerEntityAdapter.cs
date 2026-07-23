using A_exercise_EC_BE.Domain.Adapters;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Adapters;

/// <summary>
/// CustomerEntityからCustomerを復元するAdapter。
/// </summary>
public class CustomerEntityAdapter : IRestorer<Customer, CustomerEntity>
{

    /// <summary>
    /// CustomerをCustomerEntityへ変換する。
    /// </summary>
    /// <param name="target">変換元のCustomer</param>
    /// <returns>CustomerEntity</returns>
    public Task<CustomerEntity> ConvertAsync(
        Customer target
    )
    {
        _ = target
            ?? throw new InternalException(
                "顧客がnullです。");

        return Task.FromResult(
            new CustomerEntity
            {
                CustomerUuid = target.CustomerUuid,
                Name = target.Name,
                Kana = target.Kana,
                Address1 = target.Address1,
                Address2 = target.Address2,
                PhoneNumber = target.PhoneNumber,
                MailAddress = target.MailAddress,
                Username = target.Username,
                PasswordHash = target.PasswordHash,
                CreatedAt = target.CreatedAt
            });
    }
    public Task<Customer> RestoreAsync(CustomerEntity target)
    {
        _ = target ?? throw new InternalException("顧客Entityがnullです。");

        return Task.FromResult(new Customer(
            target.CustomerUuid,
            target.Name,
            target.Kana,
            target.Address1,
            target.Address2,
            target.PhoneNumber,
            target.MailAddress,
            target.Username,
            target.PasswordHash,
            target.CreatedAt));
    }
}
