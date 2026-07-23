using A_exercise_EC_BE.Domain.Adapters;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Adapters;

/// <summary>
/// ドメインオブジェクト:PaymentMethodとPaymentMethodEntityの相互変換クラス
/// </summary>
public class PaymentMethodEntityAdapter :
    IConverter<PaymentMethod, PaymentMethodEntity>,
    IRestorer<PaymentMethod, PaymentMethodEntity>
{
    /// <summary>
    /// ドメインオブジェクト:PaymentMethodをPaymentMethodEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:PaymentMethod</param>
    /// <returns>EFCore:PaymentMethodEntity</returns>
    public Task<PaymentMethodEntity> ConvertAsync(PaymentMethod domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");

        var entity = new PaymentMethodEntity();
        entity.Id = domain.Id;
        entity.Name = domain.Name;

        return Task.FromResult(entity);
    }

    /// <summary>
    /// PaymentMethodEntityからドメインオブジェクト:PaymentMethodを復元する
    /// </summary>
    /// <param name="target">EFCore:PaymentMethodEntity</param>
    /// <returns>ドメインオブジェクト:PaymentMethod</returns>
    public Task<PaymentMethod> RestoreAsync(PaymentMethodEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");

        var domain = new PaymentMethod(
            target.Id,
            target.Name
        );

        return Task.FromResult(domain);
    }
}