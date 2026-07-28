using A_exercise_EC_BE.Applications.Usecases.Products;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using A_exercise_EC_BE.Domains.Models;
using Microsoft.AspNetCore.Mvc;

namespace A_exercise_EC_BE.Presentations.Controllers;
/// <summary>
/// ユースケース:[商品をカテゴリー検索をする]を実現するコントローラ
/// </summary>
[ApiController]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[Route("product/search")]
[Tags("UC003: 商品カテゴリー検索")]
public class SearchProductByCategoryController : ControllerBase
{
    private readonly ISearchProductByCategoryUsecase _usecase;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[商品をカテゴリー検索する]を実現するインターフェイス</param>
    public SearchProductByCategoryController(ISearchProductByCategoryUsecase usecase)
    {
        _usecase = usecase;
    }
    /// <summary>
    /// カテゴリーで商品を検索する
    /// </summary>
    /// <param name="productCategoryUuid">検索Id</param>
    /// <returns>検索結果の商品一覧</returns>

    [HttpGet]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search(Guid productCategoryUuid)
    {
        // 商品キーワード検索する
        var result = await _usecase.ExecuteAsync(productCategoryUuid);
        return Ok(result);
    }
}
