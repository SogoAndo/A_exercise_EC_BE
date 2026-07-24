using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace A_exercise_EC_BE.Infrastructures.Tests.Contexts;

/// <summary>
/// AppDbContextのモデル定義テスト。
/// </summary>
[TestClass]
[TestCategory("Infrastructure/Contexts")]
public class AppDbContextTests
{
    /// <summary>
    /// 商品関連テーブルのモデル定義が
    /// 共有データベースの定義と一致することを確認する。
    /// </summary>
    [TestMethod]
    public void Model_MatchesSharedProductTables()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var product =
            context.Model.FindEntityType(
                typeof(ProductEntity));

        var category =
            context.Model.FindEntityType(
                typeof(ProductCategoryEntity));

        var stock =
            context.Model.FindEntityType(
                typeof(ProductStockEntity));

        // Assert
        Assert.IsNotNull(product);
        Assert.IsNotNull(category);
        Assert.IsNotNull(stock);

        /*
         * テーブル名
         */
        Assert.AreEqual(
            "product",
            product.GetTableName());

        Assert.AreEqual(
            "product_category",
            category.GetTableName());

        Assert.AreEqual(
            "product_stock",
            stock.GetTableName());

        /*
         * 商品テーブルのカラム設定
         */
        Assert.AreEqual(
            100,
            product
                .FindProperty(
                    nameof(ProductEntity.Name))
                ?.GetMaxLength());

        Assert.IsFalse(
            product
                .FindProperty(
                    nameof(ProductEntity.Name))
                ?.IsNullable);

        Assert.AreEqual(
            200,
            product
                .FindProperty(
                    nameof(ProductEntity.ImageUrl))
                ?.GetMaxLength());

        Assert.IsTrue(
            product
                .FindProperty(
                    nameof(ProductEntity.ImageUrl))
                ?.IsNullable);

        /*
         * カテゴリテーブルのカラム設定
         */
        Assert.AreEqual(
            30,
            category
                .FindProperty(
                    nameof(ProductCategoryEntity.Name))
                ?.GetMaxLength());

        Assert.IsFalse(
            category
                .FindProperty(
                    nameof(ProductCategoryEntity.Name))
                ?.IsNullable);

        /*
         * UUIDのユニーク制約
         */
        AssertHasUniqueIndex(
            product,
            nameof(ProductEntity.ProductUuid));

        AssertHasUniqueIndex(
            category,
            nameof(ProductCategoryEntity.CategoryUuid));

        AssertHasUniqueIndex(
            stock,
            nameof(ProductStockEntity.StockUuid));
    }



    /// <summary>
    /// 商品と商品カテゴリの外部キー設定が
    /// 正しいことを確認する。
    /// </summary>
    [TestMethod]
    public void Model_ConfiguresProductCategoryRelationship()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var product =
            context.Model.FindEntityType(
                typeof(ProductEntity));

        // Assert
        Assert.IsNotNull(product);

        var foreignKey =
            product
                .GetForeignKeys()
                .SingleOrDefault(key =>
                    key.Properties.Count == 1
                    && key.Properties[0].Name
                    == nameof(
                        ProductEntity.ProductCategoryId));

        Assert.IsNotNull(foreignKey);

        Assert.AreEqual(
            typeof(ProductCategoryEntity),
            foreignKey.PrincipalEntityType.ClrType);

        Assert.AreEqual(
            "fk_product_category",
            foreignKey.GetConstraintName());

        Assert.AreEqual(
            DeleteBehavior.Restrict,
            foreignKey.DeleteBehavior);

        Assert.AreEqual(
            nameof(ProductEntity.ProductCategory),
            foreignKey.DependentToPrincipal?.Name);

        Assert.AreEqual(
            nameof(ProductCategoryEntity.Products),
            foreignKey.PrincipalToDependent?.Name);
    }

    /// <summary>
    /// 商品と商品在庫の一対一関係が
    /// 正しいことを確認する。
    /// </summary>
    [TestMethod]
    public void Model_ConfiguresProductStockRelationship()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var stock =
            context.Model.FindEntityType(
                typeof(ProductStockEntity));

        // Assert
        Assert.IsNotNull(stock);

        var foreignKey =
            stock
                .GetForeignKeys()
                .SingleOrDefault(key =>
                    key.Properties.Count == 1
                    && key.Properties[0].Name
                    == nameof(
                        ProductStockEntity.ProductId));

        Assert.IsNotNull(foreignKey);

        Assert.AreEqual(
            typeof(ProductEntity),
            foreignKey.PrincipalEntityType.ClrType);

        Assert.AreEqual(
            "fk_product_stock_product",
            foreignKey.GetConstraintName());

        Assert.AreEqual(
            DeleteBehavior.Restrict,
            foreignKey.DeleteBehavior);

        Assert.IsTrue(
            foreignKey.IsUnique,
            "商品と商品在庫の関係は一対一である必要があります。");

        Assert.AreEqual(
            nameof(ProductStockEntity.Product),
            foreignKey.DependentToPrincipal?.Name);

        Assert.AreEqual(
            nameof(ProductEntity.ProductStock),
            foreignKey.PrincipalToDependent?.Name);
    }

    /// <summary>
    /// 商品の削除フラグにデフォルト値が
    /// 設定されていることを確認する。
    /// </summary>
    [TestMethod]
    public void Model_ConfiguresProductDeleteFlagDefaultValue()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var product =
            context.Model.FindEntityType(
                typeof(ProductEntity));

        // Assert
        Assert.IsNotNull(product);

        var deleteFlg =
            product.FindProperty(
                nameof(ProductEntity.DeleteFlg));

        Assert.IsNotNull(deleteFlg);

        Assert.AreEqual(
            0,
            deleteFlg.GetDefaultValue());
    }

    /// <summary>
    /// 購入機能に必要なEntityが
    /// AppDbContextへ登録されていることを確認する。
    /// </summary>
    [TestMethod]
    public void Model_ContainsPurchaseRelatedEntities()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var paymentMethod =
            context.Model.FindEntityType(
                typeof(PaymentMethodEntity));

        var orderStatus =
            context.Model.FindEntityType(
                typeof(OrderStatusEntity));

        var orders =
            context.Model.FindEntityType(
                typeof(OrdersEntity));

        var ordersDetail =
            context.Model.FindEntityType(
                typeof(OrdersDetailEntity));

        // Assert
        Assert.IsNotNull(paymentMethod);
        Assert.IsNotNull(orderStatus);
        Assert.IsNotNull(orders);
        Assert.IsNotNull(ordersDetail);
    }

    /// <summary>
    /// 購入機能に必要なEntityが、
    /// 想定するテーブルへマッピングされていることを確認する。
    /// </summary>
    [TestMethod]
    public void Model_MatchesPurchaseRelatedTableNames()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var paymentMethod =
            context.Model.FindEntityType(
                typeof(PaymentMethodEntity));

        var orderStatus =
            context.Model.FindEntityType(
                typeof(OrderStatusEntity));

        var orders =
            context.Model.FindEntityType(
                typeof(OrdersEntity));

        var ordersDetail =
            context.Model.FindEntityType(
                typeof(OrdersDetailEntity));

        // Assert
        Assert.IsNotNull(paymentMethod);
        Assert.IsNotNull(orderStatus);
        Assert.IsNotNull(orders);
        Assert.IsNotNull(ordersDetail);

        Assert.AreEqual(
            "payment_method",
            paymentMethod.GetTableName());

        Assert.AreEqual(
            "order_status",
            orderStatus.GetTableName());

        Assert.AreEqual(
            "orders",
            orders.GetTableName());

        Assert.AreEqual(
            "orders_detail",
            ordersDetail.GetTableName());
    }

    /// <summary>
    /// AppDbContextに必要なDbSetが
    /// 定義されていることを確認する。
    /// </summary>
    [TestMethod]
    public void DbSets_AreAvailable()
    {
        // Arrange
        using var context = CreateContext();

        // Act・Assert
        Assert.IsNotNull(context.Customers);
        Assert.IsNotNull(context.ProductCategories);
        Assert.IsNotNull(context.Products);
        Assert.IsNotNull(context.ProductStocks);
        Assert.IsNotNull(context.PaymentMethods);
        Assert.IsNotNull(context.OrderStatuses);
        Assert.IsNotNull(context.Orders);
        Assert.IsNotNull(context.OrdersDetails);
    }

    /// <summary>
    /// 指定したプロパティに単一カラムの
    /// ユニークインデックスが設定されていることを確認する。
    /// </summary>
    /// <param name="entityType">
    /// 確認対象のEntity型情報
    /// </param>
    /// <param name="propertyName">
    /// 確認対象のプロパティ名
    /// </param>
    private static void AssertHasUniqueIndex(
        IReadOnlyEntityType entityType,
        string propertyName
    )
    {
        var index =
            entityType
                .GetIndexes()
                .SingleOrDefault(candidate =>
                    candidate.Properties.Count == 1
                    && candidate.Properties[0].Name
                    == propertyName);

        Assert.IsNotNull(
            index,
            $"{propertyName}のインデックスが存在しません。");

        Assert.IsTrue(
            index.IsUnique,
            $"{propertyName}のインデックスがユニークではありません。");
    }

    /// <summary>
    /// テスト用のAppDbContextを生成する。
    /// </summary>
    /// <returns>
    /// AppDbContext
    /// </returns>
    private static AppDbContext CreateContext()
    {
        var options =
            new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(
                    "Host=localhost;"
                    + "Database=model_validation;"
                    + "Username=test;"
                    + "Password=test")
                .Options;

        return new AppDbContext(options);
    }
}