using A_exercise_EC_BE.Domain.Models;

namespace A_exercise_EC_BE.Domain.Repositories;

/// <summary>
/// 顧客アカウントの参照操作。
/// </summary>
public interface ICustomerRepository
{
    Task<Customer?> FindByMailAddressAsync(string mailAddress);
}
