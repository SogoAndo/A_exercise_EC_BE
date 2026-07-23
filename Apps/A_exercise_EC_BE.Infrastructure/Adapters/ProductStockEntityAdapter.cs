using A_exercise_EC_BE.Domain.Adapters;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Infrastructure.Entities;
namespace A_exercise_EC_BE.Infrastructure.Adapters;
/// <summary>
/// ドメインオブジェクト:ProductStockとProductStockEntityの相互変換クラス
/// </summary> 
/// <typeparam name="ProductStock">ドメインオブジェクト:ProductStock</typeparam>
/// <typeparam name="ProductStockEntity">EFCore:ProductStockEntity</typeparam>
public class ProductStockEntityAdapter :
IConverter<ProductStock, ProductStockEntity>, IRestorer<ProductStock, ProductStockEntity>
{
    /// <summary>
    /// ドメインオブジェクト:ProductStockをProductStockEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:ProductStock</param>
    /// <returns>EFCore:ProductStockEntity</returns>
    public Task<ProductStockEntity> ConvertAsync(ProductStock domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");
        // ドメインオブジェクト:ProductStockをProductStockEntityに変換する
        var entity = new ProductStockEntity();
        entity.StockUuid = domain.StockUuid;
        entity.Quantity = domain.Quantity;
        return Task.FromResult(entity);
    }

    /// <summary>
    /// ProductStockEntityからドメインオブジェクト:ProductStockを復元する
    /// </summary>
    /// <param name="target">>EFCore:ProductStockEntity</param>
    /// <returns>ドメインオブジェクト:ProductStock</returns>
    public Task<ProductStock> RestoreAsync(ProductStockEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");
        // ProductStockEntityからドメインオブジェクト:ProductStockを復元する
        var domain = new ProductStock(target.StockUuid, target.Quantity);
        return Task.FromResult(domain);
    }
}