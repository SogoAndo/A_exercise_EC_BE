using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Repositories;

/// <summary>
/// 顧客アカウントの参照操作。
/// </summary>
public interface ICustomerRepository
{
    Task<Customer?> FindByMailAddressAsync(string mailAddress);

    /// <summary>
    /// 顧客UUIDに一致する顧客を取得する。
    /// </summary>
    /// <param name="customerUuid">顧客UUID</param>
    /// <returns>
    /// 一致する顧客。
    /// 存在しない場合はnull。
    /// </returns>
    Task<Customer?> FindByCustomerUuidAsync(
        Guid customerUuid);

    /// <summary>
    /// ユーザー名がすでに登録されているか確認する。
    /// </summary>
    Task<bool> ExistsByUsernameAsync(
        string username
    );

    /// <summary>
    /// メールアドレスがすでに登録されているか確認する。
    /// </summary>
    Task<bool> ExistsByMailAddressAsync(
        string mailAddress
    );

    Task CreateAsync(Customer customer);

}
