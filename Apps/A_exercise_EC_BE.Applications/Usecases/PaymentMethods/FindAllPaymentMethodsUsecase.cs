using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;

namespace A_exercise_EC_BE.Applications.Usecases.PaymentMethods;

/// <summary>
/// 支払い方法一覧取得ユースケース。
/// </summary>
public class FindAllPaymentMethodsUsecase(
    IPaymentMethodRepository paymentMethodRepository)
    : IFindAllPaymentMethodsUsecase
{
    /// <inheritdoc />
    public async Task<List<PaymentMethod>> ExecuteAsync()
    {
        return await paymentMethodRepository
            .FindAllAsync();
    }
}