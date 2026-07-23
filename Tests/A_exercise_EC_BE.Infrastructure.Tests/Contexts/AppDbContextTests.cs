using A_exercise_EC_BE.Infrastructure.Contexts;
using A_exercise_EC_BE.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace A_exercise_EC_BE.Infrastructure.Tests.Contexts;

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
    /// 顧客テーブルのモデル定義が
    /// 共有データベースの定義と一致することを確認する。
    /// </summary>
    [TestMethod]
    public void Model_MatchesSharedCustomerTable()
    {
        // Arrange
        using var context = CreateContext();

        // Act
        var customer =
            context.Model.FindEntityType(
                typeof(CustomerEntity));

        // Assert
        Assert.IsNotNull(customer);

        /*
         * テーブル名
         */
        Assert.AreEqual(
            "customer",
            customer.GetTableName());

        /*
         * 氏名
         */
        Assert.AreEqual(
            20,
            customer
                .FindProperty(
                    nameof(CustomerEntity.Name))
                ?.GetMaxLength());

        Assert.IsFalse(
            customer
                .FindProperty(
                    nameof(CustomerEntity.Name))
                ?.IsNullable);

        /*
         * 氏名カナ
         *
         * OnModelCreatingでIsRequired()を指定していないため、
         * 現在のモデル定義ではNULLを許容する。
         */
        Assert.AreEqual(
            20,
            customer
                .FindProperty(
                    nameof(CustomerEntity.Kana))
                ?.GetMaxLength());

        Assert.IsTrue(
            customer
                .FindProperty(
                    nameof(CustomerEntity.Kana))
                ?.IsNullable);

        /*
         * 住所1
         */
        Assert.AreEqual(
            100,
            customer
                .FindProperty(
                    nameof(CustomerEntity.Address1))
                ?.GetMaxLength());

        Assert.IsFalse(
            customer
                .FindProperty(
                    nameof(CustomerEntity.Address1))
                ?.IsNullable);

        /*
         * 住所2
         */
        Assert.AreEqual(
            100,
            customer
                .FindProperty(
                    nameof(CustomerEntity.Address2))
                ?.GetMaxLength());

        Assert.IsTrue(
            customer
                .FindProperty(
                    nameof(CustomerEntity.Address2))
                ?.IsNullable);

        /*
         * 電話番号
         */
        Assert.AreEqual(
            20,
            customer
                .FindProperty(
                    nameof(CustomerEntity.PhoneNumber))
                ?.GetMaxLength());

        Assert.IsFalse(
            customer
                .FindProperty(
                    nameof(CustomerEntity.PhoneNumber))
                ?.IsNullable);

        /*
         * メールアドレス
         */
        Assert.AreEqual(
            200,
            customer
                .FindProperty(
                    nameof(CustomerEntity.MailAddress))
                ?.GetMaxLength());

        Assert.IsFalse(
            customer
                .FindProperty(
                    nameof(CustomerEntity.MailAddress))
                ?.IsNullable);

        /*
         * ユーザー名
         */
        Assert.AreEqual(
            30,
            customer
                .FindProperty(
                    nameof(CustomerEntity.Username))
                ?.GetMaxLength());

        Assert.IsFalse(
            customer
                .FindProperty(
                    nameof(CustomerEntity.Username))
                ?.IsNullable);

        /*
         * パスワード
         */
        Assert.AreEqual(
            255,
            customer
                .FindProperty(
                    nameof(CustomerEntity.Password))
                ?.GetMaxLength());

        Assert.IsFalse(
            customer
                .FindProperty(
                    nameof(CustomerEntity.Password))
                ?.IsNullable);

        /*
         * CustomerEntity.Passwordが、
         * customerテーブルのpasswordカラムへ
         * マッピングされていることを確認する。
         */
        var customerTable =
            StoreObjectIdentifier.Table(
                "customer",
                schema: null);

        Assert.AreEqual(
            "password",
            customer
                .FindProperty(
                    nameof(CustomerEntity.Password))
                ?.GetColumnName(customerTable));

        /*
         * ユニーク制約
         */
        AssertHasUniqueIndex(
            customer,
            nameof(CustomerEntity.CustomerUuid));

        AssertHasUniqueIndex(
            customer,
            nameof(CustomerEntity.MailAddress));

        AssertHasUniqueIndex(
            customer,
            nameof(CustomerEntity.Username));
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