using A_exercise_EC_BE.Domains.Adapters;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Entities;
namespace A_exercise_EC_BE.Infrastructures.Adapters;
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