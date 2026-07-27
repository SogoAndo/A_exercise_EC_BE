using System.Reflection;
using System.Runtime.CompilerServices;
using A_exercise_EC_BE.Applications
    .Usecases.ProductCategories;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Presentations.Controllers;
using A_exercise_EC_BE.Presentations
    .ViewModels.ProductCategories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace A_exercise_EC_BE.Presentations.Tests.Controllers;

/// <summary>
/// ProductCategoryControllerの単体テスト。
/// </summary>
[TestClass]
[TestCategory("Controllers")]
public class ProductCategoryControllerTests
{
    private Mock<IFindAllProductCategoriesUsecase>
        _findAllProductCategoriesUsecaseMock = null!;

    private ProductCategoryController
        _controller = null!;

    /// <summary>
    /// テストの前処理。
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _findAllProductCategoriesUsecaseMock =
            new Mock<
                IFindAllProductCategoriesUsecase>();

        _controller =
            new ProductCategoryController(
                _findAllProductCategoriesUsecaseMock
                    .Object);
    }

    /// <summary>
    /// 商品カテゴリが存在する場合、
    /// プルダウン項目一覧を返すこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindAllOptionsAsync_商品カテゴリが存在する場合はプルダウン項目一覧を返す")]
    public async Task
        FindAllOptionsAsync_WhenProductCategoriesExist_ReturnsOptions()
    {
        // Arrange
        var firstCategoryUuid =
            Guid.NewGuid();

        var secondCategoryUuid =
            Guid.NewGuid();

        var productCategories =
            new List<ProductCategory>
            {
                CreateProductCategory(
                    firstCategoryUuid,
                    "食品"),

                CreateProductCategory(
                    secondCategoryUuid,
                    "文房具")
            };

        _findAllProductCategoriesUsecaseMock
            .Setup(
                x => x.ExecuteAsync())
            .ReturnsAsync(productCategories);

        // Act
        var actionResult =
            await _controller
                .FindAllOptionsAsync();

        // Assert
        var okResult =
            actionResult.Result
                as OkObjectResult;

        Assert.IsNotNull(
            okResult);

        Assert.AreEqual(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        var actual =
            okResult.Value
                as List<
                    ProductCategoryOptionViewModel>;

        Assert.IsNotNull(
            actual);

        Assert.HasCount(
            2,
            actual);

        Assert.AreEqual(
            firstCategoryUuid,
            actual[0].Value);

        Assert.AreEqual(
            "食品",
            actual[0].Label);

        Assert.AreEqual(
            secondCategoryUuid,
            actual[1].Value);

        Assert.AreEqual(
            "文房具",
            actual[1].Label);

        _findAllProductCategoriesUsecaseMock
            .Verify(
                x => x.ExecuteAsync(),
                Times.Once);
    }

    /// <summary>
    /// 商品カテゴリが存在しない場合、
    /// 空のプルダウン項目一覧を返すこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindAllOptionsAsync_商品カテゴリが存在しない場合は空の一覧を返す")]
    public async Task
        FindAllOptionsAsync_WhenProductCategoriesDoNotExist_ReturnsEmptyOptions()
    {
        // Arrange
        _findAllProductCategoriesUsecaseMock
            .Setup(
                x => x.ExecuteAsync())
            .ReturnsAsync(
                new List<ProductCategory>());

        // Act
        var actionResult =
            await _controller
                .FindAllOptionsAsync();

        // Assert
        var okResult =
            actionResult.Result
                as OkObjectResult;

        Assert.IsNotNull(
            okResult);

        Assert.AreEqual(
            StatusCodes.Status200OK,
            okResult.StatusCode);

        var actual =
            okResult.Value
                as List<
                    ProductCategoryOptionViewModel>;

        Assert.IsNotNull(
            actual);

        Assert.IsEmpty(
            actual);

        _findAllProductCategoriesUsecaseMock
            .Verify(
                x => x.ExecuteAsync(),
                Times.Once);
    }

    /// <summary>
    /// ユースケースで例外が発生した場合、
    /// 同じ例外を呼び出し元へ伝播すること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "FindAllOptionsAsync_ユースケースで例外が発生した場合は同じ例外を再スローする")]
    public async Task
        FindAllOptionsAsync_WhenUsecaseThrowsException_PropagatesException()
    {
        // Arrange
        var expected =
            new InvalidOperationException(
                "商品カテゴリの取得に失敗しました。");

        _findAllProductCategoriesUsecaseMock
            .Setup(
                x => x.ExecuteAsync())
            .ThrowsAsync(expected);

        // Act
        var actual =
            await Assert.ThrowsExactlyAsync<
                InvalidOperationException>(
                async () =>
                {
                    await _controller
                        .FindAllOptionsAsync();
                });

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _findAllProductCategoriesUsecaseMock
            .Verify(
                x => x.ExecuteAsync(),
                Times.Once);
    }

    /// <summary>
    /// テスト用の商品カテゴリを生成する。
    /// </summary>
    /// <param name="categoryUuid">
    /// 商品カテゴリ識別ID。
    /// </param>
    /// <param name="name">
    /// 商品カテゴリ名。
    /// </param>
    /// <returns>
    /// テスト用の商品カテゴリ。
    /// </returns>
    private static ProductCategory
        CreateProductCategory(
            Guid categoryUuid,
            string name)
    {
        var productCategory =
            (ProductCategory)RuntimeHelpers
                .GetUninitializedObject(
                    typeof(ProductCategory));

        SetPrivateProperty(
            productCategory,
            "CategoryUuid",
            categoryUuid);

        SetPrivateProperty(
            productCategory,
            "Name",
            name);

        return productCategory;
    }

    /// <summary>
    /// private setのプロパティへ
    /// テスト用の値を設定する。
    /// </summary>
    /// <typeparam name="T">
    /// 設定対象の型。
    /// </typeparam>
    /// <param name="target">
    /// 設定対象。
    /// </param>
    /// <param name="propertyName">
    /// プロパティ名。
    /// </param>
    /// <param name="value">
    /// 設定値。
    /// </param>
    private static void SetPrivateProperty<T>(
        T target,
        string propertyName,
        object? value)
    {
        var field =
            typeof(T).GetField(
                $"<{propertyName}>k__BackingField",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        if (field is null)
        {
            throw new InvalidOperationException(
                $"{propertyName}のバッキングフィールドが見つかりません。");
        }

        field.SetValue(
            target,
            value);
    }
}