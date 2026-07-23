using A_exercise_EC_BE.Domain.Models;

namespace A_exercise_EC_BE.Application.Usecases.Accounts;

/// <summary>
/// 顧客アカウント登録ユースケースのインターフェース
/// </summary>
public interface IRegisterCustomerAccountUsecase
{
    /// <summary>
    /// アカウント名が既に存在するかを検証する
    /// </summary>
    /// <param name="username">アカウント名</param>
    /// <returns>なし</returns>
    Task ExistsByUsernameAsync(string username);

    /// <summary>
    /// メールアドレスが既に存在するかを検証する
    /// </summary>
    /// <param name="mailAddress">メールアドレス</param>
    /// <returns>なし</returns>
    Task ExistsByMailAddressAsync(string mailAddress);

    /// <summary>
    /// 顧客アカウントを登録する
    /// </summary>
    /// <param name="customer">登録対象の顧客</param>
    /// <returns>なし</returns>
    Task RegisterCustomerAccountAsync(Customer customer);
}