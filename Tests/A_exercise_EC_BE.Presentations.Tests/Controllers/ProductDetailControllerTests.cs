using System.Reflection;
using A_exercise_EC_BE.Applications.Usecases.Products;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.Controllers;
using A_exercise_EC_BE.Presentations.ViewModels.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace A_exercise_EC_BE.Presentations.Tests.Controllers;

/// <summary>
/// ProductDetailControllerの単体テスト。
/// </summary>
[TestClass]
[TestCategory("Presentations/Controllers")]
public class ProductDetailControllerTests
{
    [TestMethod(
        DisplayName =
            "商品詳細と在庫数を200で返す")]
    public async Task
        GetAsync_WhenProductExists_ReturnsOk()
    {
        var productUuid =
            Guid.Parse(
                "11111111-1111-1111-1111-111111111111");
        var usecase =
            new Mock<IGetProductDetailUsecase>();
        usecase
            .Setup(target =>
                target.GetAsync(
                    productUuid))
            .ReturnsAsync(
                new ProductDetailResult(
                    productUuid,
                    "ボールペン",
                    120,
                    "https://example.com/pen.png",
                    8));
        var controller =
            new ProductDetailController(
                usecase.Object,
                new ProductDetailViewModelAdapter());

        var actionResult =
            await controller.GetAsync(
                productUuid);

        var okResult =
            actionResult.Result
                as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response =
            okResult.Value
                as ProductDetailViewModel;
        Assert.IsNotNull(response);
        Assert.AreEqual(
            productUuid,
            response.ProductUuid);
        Assert.AreEqual(
            "ボールペン",
            response.ProductName);
        Assert.AreEqual(
            120,
            response.Price);
        Assert.AreEqual(
            "https://example.com/pen.png",
            response.ProductImage);
        Assert.AreEqual(
            8,
            response.StockQuantity);
        usecase.Verify(
            target =>
                target.GetAsync(
                    productUuid),
            Times.Once);
    }

    [TestMethod(
        DisplayName =
            "商品詳細Actionは仕様書のURLで認証を要求しない")]
    public void
        GetAsync_UsesExpectedRouteWithoutAuthentication()
    {
        var controllerType =
            typeof(ProductDetailController);
        var route =
            controllerType
                .GetCustomAttribute<
                    RouteAttribute>();
        var method =
            controllerType.GetMethod(
                nameof(
                    ProductDetailController.GetAsync))
            ?? throw new InvalidOperationException(
                "商品詳細Actionが見つかりません。");
        var httpGet =
            method.GetCustomAttribute<
                HttpGetAttribute>();
        var authorize =
            method.GetCustomAttribute<
                AuthorizeAttribute>();

        Assert.AreEqual(
            "products/detail",
            route?.Template);
        Assert.AreEqual(
            "{productId:guid}",
            httpGet?.Template);
        Assert.IsNull(authorize);
    }
}
