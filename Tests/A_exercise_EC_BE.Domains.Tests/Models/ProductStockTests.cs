using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Tests.Models;

[TestClass]
[TestCategory("Domain/Models")]
public class ProductStockTests
{
    [TestMethod]
    public void Constructor_WithValidValues_CreatesStock()
    {
        var id = Guid.NewGuid();

        var stock = new ProductStock(id, 0);

        Assert.AreEqual(id, stock.StockUuid);
        Assert.AreEqual(0, stock.Quantity);
    }

    [TestMethod]
    public void Constructor_WithEmptyUuid_ThrowsDomainException()
    {
        Assert.ThrowsExactly<DomainException>(
            () => new ProductStock(Guid.Empty, 1));
    }

    [TestMethod]
    public void Constructor_WithNegativeQuantity_ThrowsDomainException()
    {
        Assert.ThrowsExactly<DomainException>(
            () => new ProductStock(Guid.NewGuid(), -1));
    }
}
