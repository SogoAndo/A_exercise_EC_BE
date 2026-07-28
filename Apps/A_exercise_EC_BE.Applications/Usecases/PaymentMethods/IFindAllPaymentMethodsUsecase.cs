using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Applications.Usecases.PaymentMethods;

/// <summary>
/// 支払い方法一覧取得ユースケース。
/// </summary>
public interface IFindAllPaymentMethodsUsecase
{
    /// <summary>
    /// 支払い方法をすべて取得する。
    /// </summary>
    /// <returns>支払い方法一覧。</returns>
    Task<List<PaymentMethod>> ExecuteAsync();
}