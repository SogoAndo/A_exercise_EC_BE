using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;

namespace A_exercise_EC_BE.Applications.Usecases.Purchases;

/// <summary>
/// UC007: 購入履歴閲覧UseCase。
/// </summary>
public sealed class SearchPurchaseHistoryUsecase(
    IOrderRepository orderRepository)
    : ISearchPurchaseHistoryUsecase
{
    /// <inheritdoc />
    public Task<List<Orders>> SearchAsync(
        Guid customerUuid)
    {
        ValidateUuid(
            customerUuid,
            "顧客識別IDが不正です。");

        return orderRepository.FindByCustomerUuidAsync(
            customerUuid);
    }

    /// <inheritdoc />
    public async Task<Orders?> FindDetailAsync(
        Guid customerUuid,
        Guid orderUuid)
    {
        ValidateUuid(
            customerUuid,
            "顧客識別IDが不正です。");
        ValidateUuid(
            orderUuid,
            "注文識別IDが不正です。");

        var order = await orderRepository.FindByOrderUuidAsync(
            orderUuid);

        if (order is null
            || order.Customer.CustomerUuid != customerUuid)
        {
            return null;
        }

        return order;
    }

    private static void ValidateUuid(
        Guid uuid,
        string message)
    {
        if (uuid == Guid.Empty)
        {
            throw new DomainException(message);
        }
    }
}
