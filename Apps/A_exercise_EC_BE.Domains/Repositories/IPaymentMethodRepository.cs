using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Repositories;

/// <summary>
/// 支払い方法Repositoryのインターフェース。
/// </summary>
public interface IPaymentMethodRepository
{

    /// <summary>
    /// 支払い方法一覧を取得する。
    /// </summary>
    /// <returns>支払い方法一覧</returns>
    Task<List<PaymentMethod>> FindAllAsync();
    /// <summary>
    /// 支払い方法IDに一致する支払い方法を取得する。
    /// </summary>
    /// <param name="paymentMethodId">
    /// 支払い方法ID
    /// </param>
    /// <returns>
    /// 一致する支払い方法。
    /// 存在しない場合はnull。
    /// </returns>
    Task<PaymentMethod?> FindByIdAsync(
        int paymentMethodId);
}
