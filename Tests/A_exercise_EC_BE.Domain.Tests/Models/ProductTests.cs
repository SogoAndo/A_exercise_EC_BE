using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;

namespace A_exercise_EC_BE.Domain.Tests.Models;

[TestClass]
[TestCategory("Domain/Models")]
public class ProductTests
{
    [TestMethod]
    public void Constructor_WithValidValues_CreatesProduct()
    {
        var id = Guid.NewGuid();
        var category = new ProductCategory(Guid.NewGuid(), "文房具");
        var stock = new ProductStock(Guid.NewGuid(), 10);

        var product = new Product(
            id,
            "水性ボールペン",
            120,
            null,
            category,
            stock,
            0);

        Assert.AreEqual(id, product.ProductUuid);
        Assert.AreEqual("水性ボールペン", product.Name);
        Assert.AreEqual(120, product.Price);
        Assert.IsNull(product.ImageUrl);
        Assert.AreSame(category, product.ProductCategory);
        Assert.AreSame(stock, product.ProductStock);
        Assert.AreEqual(0, product.DeleteFlg);
    }

    [TestMethod]
    public void Constructor_WithEmptyUuid_ThrowsDomainException()
    {
        Assert.ThrowsExactly<DomainException>(() => CreateProduct(productUuid: Guid.Empty));
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(1_000_001)]
    public void Constructor_WithPriceOutsideRange_ThrowsDomainException(int price)
    {
        Assert.ThrowsExactly<DomainException>(() => CreateProduct(price: price));
    }

    [TestMethod]
    public void Constructor_WithRelativeImageUrl_ThrowsDomainException()
    {
        Assert.ThrowsExactly<DomainException>(
            () => CreateProduct(imageUrl: "/images/product.png"));
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(2)]
    public void Constructor_WithInvalidDeleteFlag_ThrowsDomainException(int deleteFlg)
    {
        Assert.ThrowsExactly<DomainException>(
            () => CreateProduct(deleteFlg: deleteFlg));
    }

    [TestMethod]
    public void Constructor_WithoutCategory_ThrowsDomainException()
    {
        var stock = new ProductStock(Guid.NewGuid(), 10);

        Assert.ThrowsExactly<DomainException>(
            () => new Product(
                Guid.NewGuid(),
                "水性ボールペン",
                120,
                null,
                null!,
                stock,
                0));
    }

    [TestMethod]
    public void Constructor_WithoutStock_ThrowsDomainException()
    {
        var category = new ProductCategory(Guid.NewGuid(), "文房具");

        Assert.ThrowsExactly<DomainException>(
            () => new Product(
                Guid.NewGuid(),
                "水性ボールペン",
                120,
                null,
                category,
                null!,
                0));
    }

    private static Product CreateProduct(
        Guid? productUuid = null,
        int price = 120,
        string? imageUrl = null,
        ProductCategory? productCategory = null,
        ProductStock? productStock = null,
        int deleteFlg = 0)
    {
        var category = productCategory ?? new ProductCategory(Guid.NewGuid(), "文房具");
        var stock = productStock ?? new ProductStock(Guid.NewGuid(), 10);

        return new Product(
            productUuid ?? Guid.NewGuid(),
            "水性ボールペン",
            price,
            imageUrl,
            category,
            stock,
            deleteFlg);
    }
}
