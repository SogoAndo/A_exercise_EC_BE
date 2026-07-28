using Microsoft.EntityFrameworkCore;

using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Infrastructures.Contexts;

namespace A_exercise_EC_BE.Infrastructures.Repositories;

/// <summary>
/// 商品在庫Repository。
/// </summary>
public class ProductStockRepository
    : IProductStockRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    /// <param name="context">
    /// DBコンテキスト
    /// </param>
    public ProductStockRepository(
        AppDbContext context
    )
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<bool> TryDecreaseAsync(
        Guid productUuid,
        int quantity
    )
    {
        try
        {
            if (quantity <= 0)
            {
                throw new InternalException(
                    "在庫から減算する数量は1以上である必要があります。"
                );
            }

            var updatedCount =
                await _context.ProductStocks
                    .Where(stock =>
                        stock.Product
                            .ProductUuid
                            == productUuid
                        && stock.Quantity
                            >= quantity
                    )
                    .ExecuteUpdateAsync(
                        setters =>
                            setters.SetProperty(
                                stock =>
                                    stock.Quantity,
                                stock =>
                                    stock.Quantity
                                    - quantity
                            )
                    );

            return updatedCount > 0;
        }
        catch (InternalException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InternalException(
                "商品在庫の更新中に予期しないエラーが発生しました。",
                ex
            );
        }
    }
}