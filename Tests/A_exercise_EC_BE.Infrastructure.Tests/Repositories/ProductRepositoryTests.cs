using A_exercise_EC_BE.Infrastructure.Adapters;
using A_exercise_EC_BE.Infrastructure.Contexts;
using A_exercise_EC_BE.Infrastructure.Entities;
using A_exercise_EC_BE.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace A_exercise_EC_BE.Infrastructure.Tests.Repositories;

[TestClass]
[TestCategory("Infrastructure/Repositories")]
public class ProductRepositoryTests
{
    private AppDbContext _context = null!;
    private ProductRepository _repository = null!;
    private Guid _stationeryCategoryUuid;
    private Guid _activeProductUuid;
    private Guid _deletedProductUuid;

    [TestInitialize]
    public async Task Initialize()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        var factory = new ProductFactory(
            new ProductEntityAdapter(
                new ProductCategoryEntityAdapter(),
                new ProductStockEntityAdapter()));
        _repository = new ProductRepository(_context, factory);

        await SeedAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _context.DisposeAsync();
    }

    [TestMethod]
    public async Task FindAllAsync_ReturnsOnlyActiveProducts()
    {
        var products = await _repository.FindAllAsync();

        Assert.HasCount(2, products);
        Assert.IsTrue(products.All(product => product.DeleteFlg == 0));
    }

    [TestMethod]
    public async Task SelectByProductCategoryIdAsync_ReturnsActiveProductsInCategory()
    {
        var products = await _repository.SelectByProductCategoryIdAsync(
            _stationeryCategoryUuid);

        Assert.HasCount(1, products);
        Assert.AreEqual(_activeProductUuid, products[0].ProductUuid);
    }

    [TestMethod]
    public async Task SelectByProductCategoryIdAsync_WithoutMatch_ReturnsEmptyList()
    {
        var products = await _repository.SelectByProductCategoryIdAsync(Guid.NewGuid());

        Assert.IsEmpty(products);
    }

    [TestMethod]
    public async Task FindByIdAsync_WithActiveProduct_ReturnsProduct()
    {
        var product = await _repository.FindByIdAsync(_activeProductUuid);

        Assert.IsNotNull(product);
        Assert.AreEqual(_activeProductUuid, product.ProductUuid);
        Assert.AreEqual("文房具", product.ProductCategory.Name);
        Assert.AreEqual(80, product.ProductStock.Quantity);
    }

    [TestMethod]
    public async Task FindByIdAsync_WithDeletedProduct_ReturnsNull()
    {
        var product = await _repository.FindByIdAsync(_deletedProductUuid);

        Assert.IsNull(product);
    }

    private async Task SeedAsync()
    {
        _stationeryCategoryUuid = Guid.NewGuid();
        _activeProductUuid = Guid.NewGuid();
        _deletedProductUuid = Guid.NewGuid();

        var stationery = new ProductCategoryEntity
        {
            Id = 1,
            CategoryUuid = _stationeryCategoryUuid,
            Name = "文房具"
        };
        var accessories = new ProductCategoryEntity
        {
            Id = 2,
            CategoryUuid = Guid.NewGuid(),
            Name = "雑貨"
        };

        _context.Products.AddRange(
            CreateProduct(1, _activeProductUuid, "水性ボールペン", stationery, 0, 80),
            CreateProduct(2, _deletedProductUuid, "削除済み商品", stationery, 1, 10),
            CreateProduct(3, Guid.NewGuid(), "折り畳み傘", accessories, 0, 25));

        await _context.SaveChangesAsync();
    }

    private static ProductEntity CreateProduct(
        int id,
        Guid productUuid,
        string name,
        ProductCategoryEntity category,
        int deleteFlg,
        int quantity) => new()
        {
            Id = id,
            ProductUuid = productUuid,
            Name = name,
            Price = 120,
            ProductCategoryId = category.Id,
            ProductCategory = category,
            DeleteFlg = deleteFlg,
            ProductStock = new ProductStockEntity
            {
                Id = id,
                StockUuid = Guid.NewGuid(),
                ProductId = id,
                Quantity = quantity
            }
        };
}
