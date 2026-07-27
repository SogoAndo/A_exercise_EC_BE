using A_exercise_EC_BE.Applications.Usecases.Products;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using Moq;

namespace A_exercise_EC_BE.Applications.Tests.Usecases.Products;

/// <summary>
/// GetProductDetailUsecaseの単体テスト。
/// </summary>
[TestClass]
[TestCategory("Applications/Usecases/Products")]
public class GetProductDetailUsecaseTests
{
    [TestMethod(
        DisplayName =
            "商品が存在する場合は在庫数を含む商品詳細を返す")]
    public async Task
        GetAsync_WhenProductExists_ReturnsProductDetail()
    {
        var productUuid =
            Guid.Parse(
                "11111111-1111-1111-1111-111111111111");
        var product =
            new Product(
                productUuid,
                "ボールペン",
                120,
                "https://example.com/pen.png",
                new ProductCategory("筆記具"),
                new ProductStock(8),
                0);
        var repository =
            new Mock<IProductRepository>();
        repository
            .Setup(target =>
                target.FindByIdAsync(
                    productUuid))
            .ReturnsAsync(product);
        var usecase =
            new GetProductDetailUsecase(
                repository.Object);

        var actual =
            await usecase.GetAsync(
                productUuid);

        Assert.AreEqual(
            productUuid,
            actual.ProductUuid);
        Assert.AreEqual(
            "ボールペン",
            actual.ProductName);
        Assert.AreEqual(
            120,
            actual.Price);
        Assert.AreEqual(
            "https://example.com/pen.png",
            actual.ProductImage);
        Assert.AreEqual(
            8,
            actual.StockQuantity);
        repository.Verify(
            target =>
                target.FindByIdAsync(
                    productUuid),
            Times.Once);
    }

    [TestMethod(
        DisplayName =
            "商品UUIDが空の場合は商品を検索せず404対象の例外にする")]
    public async Task
        GetAsync_WhenProductIdIsEmpty_ThrowsNotFound()
    {
        var repository =
            new Mock<IProductRepository>();
        var usecase =
            new GetProductDetailUsecase(
                repository.Object);

        var exception =
            await Assert.ThrowsExactlyAsync<
                NotFoundException>(
                () =>
                    usecase.GetAsync(
                        Guid.Empty));

        Assert.AreEqual(
            "指定された商品は存在しません",
            exception.Message);
        repository.Verify(
            target =>
                target.FindByIdAsync(
                    It.IsAny<Guid>()),
            Times.Never);
    }

    [TestMethod(
        DisplayName =
            "商品が存在しない場合は404対象の例外にする")]
    public async Task
        GetAsync_WhenProductDoesNotExist_ThrowsNotFound()
    {
        var productUuid =
            Guid.Parse(
                "22222222-2222-2222-2222-222222222222");
        var repository =
            new Mock<IProductRepository>();
        repository
            .Setup(target =>
                target.FindByIdAsync(
                    productUuid))
            .ReturnsAsync((Product?)null);
        var usecase =
            new GetProductDetailUsecase(
                repository.Object);

        var exception =
            await Assert.ThrowsExactlyAsync<
                NotFoundException>(
                () =>
                    usecase.GetAsync(
                        productUuid));

        Assert.AreEqual(
            "指定された商品は存在しません",
            exception.Message);
    }

    [TestMethod(
        DisplayName =
            "商品に在庫情報がない場合は内部エラーにする")]
    public async Task
        GetAsync_WhenStockDoesNotExist_ThrowsInternal()
    {
        var productUuid =
            Guid.Parse(
                "33333333-3333-3333-3333-333333333333");
        var product =
            new Product(
                productUuid,
                "消しゴム",
                80);
        var repository =
            new Mock<IProductRepository>();
        repository
            .Setup(target =>
                target.FindByIdAsync(
                    productUuid))
            .ReturnsAsync(product);
        var usecase =
            new GetProductDetailUsecase(
                repository.Object);

        var exception =
            await Assert.ThrowsExactlyAsync<
                InternalException>(
                () =>
                    usecase.GetAsync(
                        productUuid));

        Assert.AreEqual(
            "商品在庫情報が登録されていません。",
            exception.Message);
    }
}
