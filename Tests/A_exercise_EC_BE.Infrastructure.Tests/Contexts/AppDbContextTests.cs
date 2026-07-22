using A_exercise_EC_BE.Infrastructure.Contexts;
using A_exercise_EC_BE.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace A_exercise_EC_BE.Infrastructure.Tests.Contexts;

[TestClass]
[TestCategory("Infrastructure/Contexts")]
public class AppDbContextTests
{
    [TestMethod]
    public void Model_MatchesSharedProductTables()
    {
        using var context = CreateContext();
        var product = context.Model.FindEntityType(typeof(ProductEntity));
        var category = context.Model.FindEntityType(typeof(ProductCategoryEntity));
        var stock = context.Model.FindEntityType(typeof(ProductStockEntity));

        Assert.IsNotNull(product);
        Assert.IsNotNull(category);
        Assert.IsNotNull(stock);
        Assert.AreEqual("product", product.GetTableName());
        Assert.AreEqual("product_category", category.GetTableName());
        Assert.AreEqual("product_stock", stock.GetTableName());
        Assert.AreEqual(100, product.FindProperty(nameof(ProductEntity.Name))?.GetMaxLength());
        Assert.AreEqual(200, product.FindProperty(nameof(ProductEntity.ImageUrl))?.GetMaxLength());
        Assert.AreEqual(30, category.FindProperty(nameof(ProductCategoryEntity.Name))?.GetMaxLength());
        Assert.IsTrue(product.GetIndexes().Single(
            index => index.Properties.Single().Name == nameof(ProductEntity.ProductUuid)).IsUnique);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(
                "Host=localhost;Database=model_validation;Username=test;Password=test")
            .Options;

        return new AppDbContext(options);
    }
}
