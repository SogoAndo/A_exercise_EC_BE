using A_exercise_EC_BE.Domains.Adapters;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Entities;
namespace A_exercise_EC_BE.Infrastructures.Adapters;
/// <summary>
/// ドメインオブジェクト:ProductCategoryとProductCategoryEntityの相互変換クラス
/// </summary> 
/// <typeparam name="ProductCategory">ドメインオブジェクト:ProductCategory</typeparam>
/// <typeparam name="ProductCategoryEntity">EFCore:ProductCategoryEntity</typeparam>
public class ProductCategoryEntityAdapter :
IConverter<ProductCategory, ProductCategoryEntity>, IRestorer<ProductCategory, ProductCategoryEntity>
{
    /// <summary>
    /// ドメインオブジェクト:ProductCategoryをProductCategoryEntityに変換する
    /// </summary>
    /// <param name="domain">ドメインオブジェクト:ProductCategory</param>
    /// <returns>EFCore:ProductCategoryEntity</returns>
    public Task<ProductCategoryEntity> ConvertAsync(ProductCategory domain)
    {
        // 引数domainがnullの場合
        _ = domain ?? throw new InternalException("引数domainがnullです。");
        // ドメインオブジェクト:ProductCategoryをProductCategoryEntityに変換する
        var entity = new ProductCategoryEntity();
        entity.CategoryUuid = domain.CategoryUuid;
        entity.Name = domain.Name;
        return Task.FromResult(entity);
    }

    /// <summary>
    /// ProductCategoryEntityからドメインオブジェクト:ProductCategoryを復元する
    /// </summary>
    /// <param name="target">>EFCore:ProductCategoryEntity</param>
    /// <returns>ドメインオブジェクト:ProductCategory</returns>
    public Task<ProductCategory> RestoreAsync(ProductCategoryEntity target)
    {
        // 引数targetがnullの場合
        _ = target ?? throw new InternalException("引数targetがnullです。");
        // ProductCategoryEntityからドメインオブジェクト:ProductCategoryを復元する
        var domain = new ProductCategory(target.CategoryUuid, target.Name);
        return Task.FromResult(domain);
    }
}