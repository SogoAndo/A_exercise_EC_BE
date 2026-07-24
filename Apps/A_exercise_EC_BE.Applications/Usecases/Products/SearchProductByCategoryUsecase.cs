using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;

namespace A_exercise_EC_BE.Applications.Usecases.Products;

public class SearchProductByCategoryUsecase:ISearchProductByCategoryUsecase
{
private readonly IProductRepository _repository;

public SearchProductByCategoryUsecase(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Product>> ExecuteAsync(Guid? productCategoryId)
    {
        if (!productCategoryId.HasValue ||
           productCategoryId.Value == Guid.Empty)
        {
            return await
                _repository.FindAllAsync();
        }

        /*
         * カテゴリ指定ありの場合は、
         * 指定カテゴリの商品だけ取得する。
         */
        return await
            _repository.SelectByProductCategoryIdAsync(productCategoryId.Value);
    }
}