using A_exercise_EC_BE.Applications
    .Usecases.ProductCategories;
using A_exercise_EC_BE.Presentations
    .ViewModels.ProductCategories;
using Microsoft.AspNetCore.Mvc;

namespace A_exercise_EC_BE.Presentations.Controllers;

/// <summary>
/// 商品カテゴリAPI。
/// </summary>
[ApiController]
[Route("product-category")]
[Tags("商品カテゴリ")]
public class ProductCategoryController(
    IFindAllProductCategoriesUsecase
        findAllProductCategoriesUsecase)
    : ControllerBase
{
    /// <summary>
    /// 商品カテゴリのプルダウン項目を取得する。
    /// </summary>
    /// <returns>
    /// 商品カテゴリのプルダウン項目一覧。
    /// </returns>
    [HttpGet("options")]
    [ProducesResponseType(
        typeof(List<ProductCategoryOptionViewModel>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<
        List<ProductCategoryOptionViewModel>>>
        FindAllOptionsAsync()
    {
        var productCategories =
            await findAllProductCategoriesUsecase
                .ExecuteAsync();

        var viewModels = productCategories
            .Select(
                productCategory =>
                    new ProductCategoryOptionViewModel
                    {
                        Value =
                            productCategory.CategoryUuid,
                        Label =
                            productCategory.Name
                    })
            .ToList();

        return Ok(viewModels);
    }
}