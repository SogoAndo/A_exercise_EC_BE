using A_exercise_EC_BE.Applications
    .Usecases.ProductCategories;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace A_exercise_EC_BE.Applications.Tests
    .Usecases.ProductCategories;

/// <summary>
/// FindAllProductCategoriesUsecaseの単体テスト
/// </summary>
[TestClass]
public class FindAllProductCategoriesUsecaseTest
{
    private Mock<IProductCategoryRepository>
        _productCategoryRepositoryMock = null!;

    private FindAllProductCategoriesUsecase
        _usecase = null!;

    /// <summary>
    /// テストの前処理
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _productCategoryRepositoryMock =
            new Mock<IProductCategoryRepository>();

        _usecase =
            new FindAllProductCategoriesUsecase(
                _productCategoryRepositoryMock.Object);
    }

    /// <summary>
    /// 商品カテゴリ一覧を取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExecuteAsync_商品カテゴリ一覧を取得できる")]
    public async Task
        ExecuteAsync_ReturnsProductCategories()
    {
        // Arrange
        var expected =
            new List<ProductCategory>();

        _productCategoryRepositoryMock
            .Setup(
                x => x.FindAllAsync())
            .ReturnsAsync(expected);

        // Act
        var actual =
            await _usecase.ExecuteAsync();

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _productCategoryRepositoryMock.Verify(
            x => x.FindAllAsync(),
            Times.Once);
    }

    /// <summary>
    /// 商品カテゴリが存在しない場合、
    /// 空の一覧を返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExecuteAsync_商品カテゴリが存在しない場合は空の一覧を返す")]
    public async Task
        ExecuteAsync_WhenNoCategories_ReturnsEmptyList()
    {
        // Arrange
        var expected =
            new List<ProductCategory>();

        _productCategoryRepositoryMock
            .Setup(
                x => x.FindAllAsync())
            .ReturnsAsync(expected);

        // Act
        var actual =
            await _usecase.ExecuteAsync();

        // Assert
        Assert.IsNotNull(actual);

        Assert.IsEmpty(
            actual);

        _productCategoryRepositoryMock.Verify(
            x => x.FindAllAsync(),
            Times.Once);
    }

    /// <summary>
    /// リポジトリで例外が発生した場合、
    /// 同じ例外を呼び出し元へ伝えること
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExecuteAsync_リポジトリで例外が発生した場合は同じ例外をスローする")]
    public async Task
        ExecuteAsync_WhenRepositoryThrows_ThrowsSameException()
    {
        // Arrange
        var expected =
            new InvalidOperationException(
                "商品カテゴリの取得に失敗しました。");

        _productCategoryRepositoryMock
            .Setup(
                x => x.FindAllAsync())
            .ThrowsAsync(expected);

        // Act
        var actual =
            await Assert.ThrowsExactlyAsync<
                InvalidOperationException>(
                async () =>
                {
                    await _usecase.ExecuteAsync();
                });

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _productCategoryRepositoryMock.Verify(
            x => x.FindAllAsync(),
            Times.Once);
    }
}