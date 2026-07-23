using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Entities;

namespace A_exercise_EC_BE.Infrastructure.Tests.Adapters;

[TestClass]
public class ProductFactoryTests
{
    private ProductFactory _factory = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _factory = new ProductFactory(
            new ProductEntityAdapter(),
            new ProductCategoryEntityAdapter(),
            new ProductStockEntityAdapter());
    }

    [TestMethod]
    public async Task ConvertAsync_カテゴリ在庫なしのProductをProductEntityに変換できる()
    {
        // Arrange
        var product = new Product(
            Guid.NewGuid(),
            "りんご",
            150);

        // Act
        var result = await _factory.ConvertAsync(product);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(product.ProductUuid, result.ProductUuid);
        Assert.AreEqual(product.Name, result.Name);
        Assert.AreEqual(product.Price, result.Price);
        Assert.IsNull(result.ProductCategory);
        Assert.IsNull(result.ProductStock);
    }

    [TestMethod]
    public async Task ConvertAsync_カテゴリありのProductをProductEntityに変換できる()
    {
        // Arrange
        var category = new ProductCategory(
            Guid.NewGuid(),
            "食品");

        var product = new Product(
            Guid.NewGuid(),
            "りんご",
            150);

        product.ChangeCategory(category);

        // Act
        var result = await _factory.ConvertAsync(product);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(product.ProductUuid, result.ProductUuid);
        Assert.AreEqual(product.Name, result.Name);
        Assert.AreEqual(product.Price, result.Price);

        Assert.IsNotNull(result.ProductCategory);
        Assert.AreEqual(category.CategoryUuid, result.ProductCategory.CategoryUuid);
        Assert.AreEqual(category.Name, result.ProductCategory.Name);

        Assert.IsNull(result.ProductStock);
    }

    [TestMethod]
    public async Task ConvertAsync_在庫ありのProductをProductEntityに変換できる()
    {
        // Arrange
        var stock = new ProductStock(10);

        var product = new Product(
            Guid.NewGuid(),
            "りんご",
            150);

        product.ChangeStock(stock);

        // Act
        var result = await _factory.ConvertAsync(product);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(product.ProductUuid, result.ProductUuid);
        Assert.AreEqual(product.Name, result.Name);
        Assert.AreEqual(product.Price, result.Price);

        Assert.IsNull(result.ProductCategory);

        Assert.IsNotNull(result.ProductStock);
        Assert.AreEqual(stock.Quantity, result.ProductStock.Quantity);
    }

    [TestMethod]
    public async Task ConvertAsync_カテゴリ在庫ありのProductをProductEntityに変換できる()
    {
        // Arrange
        var category = new ProductCategory(
            Guid.NewGuid(),
            "食品");

        var stock = new ProductStock(10);

        var product = new Product(
            Guid.NewGuid(),
            "りんご",
            150);

        product.ChangeCategory(category);
        product.ChangeStock(stock);

        // Act
        var result = await _factory.ConvertAsync(product);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(product.ProductUuid, result.ProductUuid);
        Assert.AreEqual(product.Name, result.Name);
        Assert.AreEqual(product.Price, result.Price);

        Assert.IsNotNull(result.ProductCategory);
        Assert.AreEqual(category.CategoryUuid, result.ProductCategory.CategoryUuid);
        Assert.AreEqual(category.Name, result.ProductCategory.Name);

        Assert.IsNotNull(result.ProductStock);
        Assert.AreEqual(stock.Quantity, result.ProductStock.Quantity);
    }

    [TestMethod]
    public async Task ConvertAsync_ProductリストをProductEntityリストに変換できる()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product(Guid.NewGuid(), "りんご", 150),
            new Product(Guid.NewGuid(), "みかん", 120)
        };

        // Act
        var results = await _factory.ConvertAsync(products);

        // Assert
        Assert.IsNotNull(results);

        Assert.AreEqual(products[0].ProductUuid, results[0].ProductUuid);
        Assert.AreEqual(products[0].Name, results[0].Name);
        Assert.AreEqual(products[0].Price, results[0].Price);

        Assert.AreEqual(products[1].ProductUuid, results[1].ProductUuid);
        Assert.AreEqual(products[1].Name, results[1].Name);
        Assert.AreEqual(products[1].Price, results[1].Price);
    }

    [TestMethod]
    public async Task RestoreAsync_カテゴリ在庫なしのProductEntityからProductを復元できる()
    {
        // Arrange
        var entity = new ProductEntity
        {
            ProductUuid = Guid.NewGuid(),
            Name = "りんご",
            Price = 150
        };

        // Act
        var result = await _factory.RestoreAsync(entity);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(entity.ProductUuid, result.ProductUuid);
        Assert.AreEqual(entity.Name, result.Name);
        Assert.AreEqual(entity.Price, result.Price);
        Assert.IsNull(result.ProductCategory);
        Assert.IsNull(result.ProductStock);
    }

    [TestMethod]
    public async Task RestoreAsync_カテゴリありのProductEntityからProductを復元できる()
    {
        // Arrange
        var entity = new ProductEntity
        {
            ProductUuid = Guid.NewGuid(),
            Name = "りんご",
            Price = 150,
            ProductCategory = new ProductCategoryEntity
            {
                CategoryUuid = Guid.NewGuid(),
                Name = "食品"
            }
        };

        // Act
        var result = await _factory.RestoreAsync(entity);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(entity.ProductUuid, result.ProductUuid);
        Assert.AreEqual(entity.Name, result.Name);
        Assert.AreEqual(entity.Price, result.Price);

        Assert.IsNotNull(result.ProductCategory);
        Assert.AreEqual(entity.ProductCategory.CategoryUuid, result.ProductCategory.CategoryUuid);
        Assert.AreEqual(entity.ProductCategory.Name, result.ProductCategory.Name);

        Assert.IsNull(result.ProductStock);
    }

    [TestMethod]
    public async Task RestoreAsync_在庫ありのProductEntityからProductを復元できる()
    {
        // Arrange
        var entity = new ProductEntity
        {
            ProductUuid = Guid.NewGuid(),
            Name = "りんご",
            Price = 150,
            ProductStock = new ProductStockEntity
            {
                StockUuid = Guid.NewGuid(),
                Quantity = 10
            }
        };

        // Act
        var result = await _factory.RestoreAsync(entity);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(entity.ProductUuid, result.ProductUuid);
        Assert.AreEqual(entity.Name, result.Name);
        Assert.AreEqual(entity.Price, result.Price);

        Assert.IsNull(result.ProductCategory);

        Assert.IsNotNull(result.ProductStock);
        Assert.AreEqual(entity.ProductStock.Quantity, result.ProductStock.Quantity);
    }

    [TestMethod]
    public async Task RestoreAsync_カテゴリ在庫ありのProductEntityからProductを復元できる()
    {
        // Arrange
        var entity = new ProductEntity
        {
            ProductUuid = Guid.NewGuid(),
            Name = "りんご",
            Price = 150,
            ProductCategory = new ProductCategoryEntity
            {
                CategoryUuid = Guid.NewGuid(),
                Name = "食品"
            },
            ProductStock = new ProductStockEntity
            {
                StockUuid = Guid.NewGuid(),
                Quantity = 10
            }
        };

        // Act
        var result = await _factory.RestoreAsync(entity);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(entity.ProductUuid, result.ProductUuid);
        Assert.AreEqual(entity.Name, result.Name);
        Assert.AreEqual(entity.Price, result.Price);

        Assert.IsNotNull(result.ProductCategory);
        Assert.AreEqual(entity.ProductCategory.CategoryUuid, result.ProductCategory.CategoryUuid);
        Assert.AreEqual(entity.ProductCategory.Name, result.ProductCategory.Name);

        Assert.IsNotNull(result.ProductStock);
        Assert.AreEqual(entity.ProductStock.Quantity, result.ProductStock.Quantity);
    }

    [TestMethod]
    public async Task RestoreAsync_ProductEntityリストからProductリストを復元できる()
    {
        // Arrange
        var entities = new List<ProductEntity>
        {
            new ProductEntity
            {
                ProductUuid = Guid.NewGuid(),
                Name = "りんご",
                Price = 150
            },
            new ProductEntity
            {
                ProductUuid = Guid.NewGuid(),
                Name = "みかん",
                Price = 120
            }
        };

        // Act
        var results = await _factory.RestoreAsync(entities);

        // Assert
        Assert.IsNotNull(results);

        Assert.AreEqual(entities[0].ProductUuid, results[0].ProductUuid);
        Assert.AreEqual(entities[0].Name, results[0].Name);
        Assert.AreEqual(entities[0].Price, results[0].Price);

        Assert.AreEqual(entities[1].ProductUuid, results[1].ProductUuid);
        Assert.AreEqual(entities[1].Name, results[1].Name);
        Assert.AreEqual(entities[1].Price, results[1].Price);
    }
}