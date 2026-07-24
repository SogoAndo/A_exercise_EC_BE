using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Domain.Repositories;
using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Contexts;
using A_exercise_EC_BE.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructure.Repositories;

/// <summary>
/// ECサイトで販売可能な商品を参照するRepository。
/// </summary>
public class ProductRepository(AppDbContext context, ProductFactory factory)
    : IProductRepository
{
    public async Task<List<Product>> FindAllAsync()
    {
        try
        {
            var entities = await BaseQuery()
                .Where(product => product.DeleteFlg == 0)
                .OrderBy(product => product.Id)
                .ToListAsync();

            return await factory.RestoreAsync(entities);
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new InternalException(
                "商品一覧の取得中に予期しないエラーが発生しました。",
                exception);
        }
    }

    public async Task<List<Product>> SelectByProductCategoryIdAsync(
        Guid productCategoryUuid)
    {
        try
        {
            var entities = await BaseQuery()
                .Where(product =>
                    product.DeleteFlg == 0
                    && product.ProductCategory.CategoryUuid == productCategoryUuid)
                .OrderBy(product => product.Id)
                .ToListAsync();

            return await factory.RestoreAsync(entities);
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new InternalException(
                $"商品カテゴリID:{productCategoryUuid}の商品取得中に予期しないエラーが発生しました。",
                exception);
        }
    }

    public async Task<Product?> FindByIdAsync(Guid productUuid)
    {
        try
        {
            var entity = await BaseQuery()
                .SingleOrDefaultAsync(product =>
                    product.ProductUuid == productUuid
                    && product.DeleteFlg == 0);

            return entity is null
                ? null
                : await factory.RestoreAsync(entity);
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw new InternalException(
                $"商品ID:{productUuid}の商品取得中に予期しないエラーが発生しました。",
                exception);
        }
    }

    private IQueryable<ProductEntity> BaseQuery() =>
        context.Products
            .AsNoTracking()
            .Include(product => product.ProductCategory)
            .Include(product => product.ProductStock);
}
