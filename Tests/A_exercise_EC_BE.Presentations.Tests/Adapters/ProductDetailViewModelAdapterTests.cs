using A_exercise_EC_BE.Applications.Usecases.Products;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Presentations.Adapters;

namespace A_exercise_EC_BE.Presentations.Tests.Adapters;

/// <summary>
/// ProductDetailViewModelAdapterの単体テスト。
/// </summary>
[TestClass]
[TestCategory("Presentations/Adapters")]
public class ProductDetailViewModelAdapterTests
{
    [TestMethod(
        DisplayName =
            "商品詳細取得結果をViewModelへ変換する")]
    public void
        ConvertToViewModel_WhenResultIsValid_ReturnsViewModel()
    {
        var productUuid =
            Guid.Parse(
                "11111111-1111-1111-1111-111111111111");
        var result =
            new ProductDetailResult(
                productUuid,
                "ボールペン",
                120,
                "https://example.com/pen.png",
                8);
        var adapter =
            new ProductDetailViewModelAdapter();

        var actual =
            adapter.ConvertToViewModel(
                result);

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
    }

    [TestMethod(
        DisplayName =
            "商品詳細取得結果がnullの場合は内部エラーにする")]
    public void
        ConvertToViewModel_WhenResultIsNull_ThrowsInternal()
    {
        var adapter =
            new ProductDetailViewModelAdapter();

        var exception =
            Assert.ThrowsExactly<
                InternalException>(
                () =>
                    adapter.ConvertToViewModel(
                        null!));

        Assert.AreEqual(
            "引数resultがnullです。",
            exception.Message);
    }
}
