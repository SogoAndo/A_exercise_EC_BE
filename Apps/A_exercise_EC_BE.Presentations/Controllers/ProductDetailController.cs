using A_exercise_EC_BE.Applications.Usecases.Products;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.ViewModels.Products;
using Microsoft.AspNetCore.Mvc;

namespace A_exercise_EC_BE.Presentations.Controllers;

/// <summary>
/// UC004 商品詳細取得API。
/// </summary>
[ApiController]
[Route("products/detail")]
[Tags("UC004: 商品購入")]
public sealed class ProductDetailController(
    IGetProductDetailUsecase getProductDetailUsecase,
    ProductDetailViewModelAdapter adapter)
    : ControllerBase
{
    /// <summary>
    /// 指定商品の詳細情報と現在の在庫数を取得する。
    /// </summary>
    /// <param name="productId">商品UUID。</param>
    /// <returns>商品詳細。</returns>
    [HttpGet("{productId:guid}")]
    [ProducesResponseType(
        typeof(ProductDetailViewModel),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductDetailViewModel>>
        GetAsync(
            Guid productId)
    {
        var result =
            await getProductDetailUsecase.GetAsync(
                productId);

        return Ok(
            adapter.ConvertToViewModel(
                result));
    }
}
