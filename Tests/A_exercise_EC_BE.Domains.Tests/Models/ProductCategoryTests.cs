using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Tests.Models;

[TestClass]
[TestCategory("Domain/Models")]
public class ProductCategoryTests
{
    [TestMethod]
    public void Constructor_WithValidValues_CreatesCategory()
    {
        var id = Guid.NewGuid();

        var category = new ProductCategory(id, "文房具");

        Assert.AreEqual(id, category.CategoryUuid);
        Assert.AreEqual("文房具", category.Name);
    }

    [TestMethod]
    public void Constructor_WithEmptyUuid_ThrowsDomainException()
    {
        Assert.ThrowsExactly<DomainException>(
            () => new ProductCategory(Guid.Empty, "文房具"));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void Constructor_WithBlankName_ThrowsDomainException(string? name)
    {
        Assert.ThrowsExactly<DomainException>(
            () => new ProductCategory(Guid.NewGuid(), name!));
    }

    [TestMethod]
    public void Constructor_WithNameOver30Characters_ThrowsDomainException()
    {
        Assert.ThrowsExactly<DomainException>(
            () => new ProductCategory(Guid.NewGuid(), new string('あ', 31)));
    }
}
