using A_exercise_EC_BE.Domains.Adapters;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Entities;

namespace A_exercise_EC_BE.Infrastructures.Adapters;

/// <summary>
/// ドメインオブジェクト:CustomerとCustomerEntityの相互変換クラス
/// </summary>
public class CustomerEntityAdapter :
    IConverter<Customer, CustomerEntity>,
    IRestorer<Customer, CustomerEntity>
{
    /// <summary>
    /// ドメインオブジェクト:CustomerをCustomerEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:Customer</param>
    /// <returns>EFCore:CustomerEntity</returns>
    public Task<CustomerEntity> ConvertAsync(Customer domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");

        var entity = new CustomerEntity();
        entity.CustomerUuid = domain.CustomerUuid;
        entity.Name = domain.Name;
        entity.Kana = domain.Kana;
        entity.Address1 = domain.Address1;
        entity.Address2 = domain.Address2;
        entity.PhoneNumber = domain.PhoneNumber;
        entity.MailAddress = domain.MailAddress;
        entity.Username = domain.Username;
        entity.Password = domain.Password;
        entity.CreatedAt = domain.CreatedAt;

        return Task.FromResult(entity);
    }

    /// <summary>
    /// CustomerEntityからドメインオブジェクト:Customerを復元する
    /// </summary>
    /// <param name="target">EFCore:CustomerEntity</param>
    /// <returns>ドメインオブジェクト:Customer</returns>
    public Task<Customer> RestoreAsync(CustomerEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");

        var domain = new Customer(
            target.CustomerUuid,
            target.Name,
            target.Kana,
            target.Address1,
            target.Address2,
            target.PhoneNumber,
            target.MailAddress,
            target.Username,
            target.Password,
            target.CreatedAt
        );

        return Task.FromResult(domain);
    }
}