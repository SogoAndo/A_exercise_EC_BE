using A_exercise_EC_BE.Presentations
    .ViewModels.ProductCategories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_exercise_EC_BE.Presentations.Tests
    .ViewModels.ProductCategories;

/// <summary>
/// ProductCategoryOptionViewModelの単体テスト。
/// </summary>
[TestClass]
public class ProductCategoryOptionViewModelTest
{
    /// <summary>
    /// デフォルト値でインスタンスを生成できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ProductCategoryOptionViewModel_デフォルト値でインスタンスを生成できる")]
    public void
        ProductCategoryOptionViewModel_DefaultValue_CanCreateInstance()
    {
        // Act
        var model =
            new ProductCategoryOptionViewModel();

        // Assert
        Assert.AreEqual(
            Guid.Empty,
            model.Value);

        Assert.AreEqual(
            string.Empty,
            model.Label);
    }

    /// <summary>
    /// 各プロパティに値を設定できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "ProductCategoryOptionViewModel_各プロパティに値を設定できる")]
    public void
        ProductCategoryOptionViewModel_SetProperties_CanGetSameValues()
    {
        // Arrange
        var categoryUuid =
            Guid.NewGuid();

        // Act
        var model =
            new ProductCategoryOptionViewModel
            {
                Value =
                    categoryUuid,

                Label =
                    "食品"
            };

        // Assert
        Assert.AreEqual(
            categoryUuid,
            model.Value);

        Assert.AreEqual(
            "食品",
            model.Label);
    }
}