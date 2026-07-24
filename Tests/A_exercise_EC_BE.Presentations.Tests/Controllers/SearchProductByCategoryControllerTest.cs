using A_exercise_EC_BE.Applications.Usecases.Products;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Presentations.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace A_exercise_EC_BE.Presentations.Tests.Controllers;

[TestClass]
public class SearchProductByCategoryControllerTests
{
    [TestMethod(DisplayName = "カテゴリ検索で商品リストを返す")]
    public async Task Search_WhenProductsExist_ShouldReturnProductList()
    {
        // Arrange
        var usecaseMock =
            new Mock<ISearchProductByCategoryUsecase>();

        var category = new ProductCategory("食品");

        var categoryUuid = Guid.Parse(
            "e50d978b-b73d-4afb-8e85-ace9cf1e12a7");

        var products = new List<Product>
        {
            new Product(
                Guid.NewGuid(),
                "りんご",
                100,
                "",
                category,
                new ProductStock(10),
                0),

            new Product(
                Guid.NewGuid(),
                "みかん",
                200,
                "",
                category,
                new ProductStock(5),
                0)
        };

        usecaseMock
            .Setup(u => u.ExecuteAsync(
                categoryUuid))
            .ReturnsAsync(products);

        var controller =
            new SearchProductByCategoryController(
                usecaseMock.Object);

        // Act
        var result = await controller.Search(
            categoryUuid);

        // Assert
        var okResult =
            result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var actualProducts =
            okResult.Value as List<Product>;

        Assert.IsNotNull(actualProducts);
        Assert.HasCount(2, actualProducts);
        Assert.AreSame(products, actualProducts);

        usecaseMock.Verify(
            u => u.ExecuteAsync(
                categoryUuid),
            Times.Once);
    }

    [TestMethod(DisplayName = "カテゴリ検索で空の商品リストを返す")]
    public async Task Search_WhenProductsDoNotExist_ShouldReturnEmptyProductList()
    {
        // Arrange
        var usecaseMock =
            new Mock<ISearchProductByCategoryUsecase>();

        var categoryUuid = Guid.Parse(
            "e50d978b-b73d-4afb-8e85-ace9cf1e12a7");

        var products = new List<Product>();

        usecaseMock
            .Setup(u => u.ExecuteAsync(
                categoryUuid))
            .ReturnsAsync(products);

        var controller =
            new SearchProductByCategoryController(
                usecaseMock.Object);

        // Act
        var result = await controller.Search(
            categoryUuid);

        // Assert
        var okResult =
            result as OkObjectResult;

        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);

        var actualProducts =
            okResult.Value as List<Product>;

        Assert.IsNotNull(actualProducts);
        Assert.IsEmpty(actualProducts);

        usecaseMock.Verify(
            u => u.ExecuteAsync(
                categoryUuid),
            Times.Once);
    }
}