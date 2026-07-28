using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Entities;

namespace A_exercise_EC_BE.Infrastructures.Tests.Adapters;

[TestClass]
public class OrdersFactoryTests
{
    private OrdersFactory _factory = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _factory = new OrdersFactory(
            new OrdersEntityAdapter(),
            new CustomerEntityAdapter(),
            new OrderStatusEntityAdapter(),
            new PaymentMethodEntityAdapter(),
            new OrdersDetailEntityAdapter());
    }

    [TestMethod]
    public async Task ConvertAsync_Ordersを集約込みでOrdersEntityに変換できる()
    {
        var customer = new Customer(
            Guid.NewGuid(), "山田太郎", "ヤマダタロウ",
            "東京都", "1-2-3", "090-1234-5678",
            "test@example.com", "yamada", "password",
            new DateTime(2026, 7, 9, 10, 0, 0));

        var orderStatus = new OrderStatus(1, "注文受付");
        var paymentMethod = new PaymentMethod(2, "クレジットカード");
        var details = new List<OrdersDetail>
{
    new OrdersDetail(1, CreateProduct("商品A"), 3),
    new OrdersDetail(2, CreateProduct("商品B"), 5)
};

        var order = new Orders(
            Guid.NewGuid(),
            new DateTime(2026, 7, 9, 12, 0, 0),
            5000,
            customer,
            orderStatus,
            paymentMethod,
            details);

        var result = await _factory.ConvertAsync(order);

        Assert.IsNotNull(result);
        Assert.AreEqual(order.OrderUuid, result.OrderUuid);
        Assert.AreEqual(order.OrderDate, result.OrderDate);
        Assert.AreEqual(order.AmountTotal, result.AmountTotal);

        Assert.IsNotNull(result.Customer);
        Assert.AreEqual(customer.CustomerUuid, result.Customer.CustomerUuid);
        Assert.AreEqual(customer.Name, result.Customer.Name);

        Assert.IsNotNull(result.OrderStatus);
        Assert.AreEqual(orderStatus.Id, result.OrderStatus.Id);
        Assert.AreEqual(orderStatus.Name, result.OrderStatus.Name);

        Assert.IsNotNull(result.PaymentMethod);
        Assert.AreEqual(paymentMethod.Name, result.PaymentMethod.Name);

        Assert.IsNotNull(result.OrderDetails);
        Assert.HasCount(2, result.OrderDetails);
        Assert.AreEqual(3, result.OrderDetails[0].Count);
        Assert.AreEqual(5, result.OrderDetails[1].Count);
    }

    [TestMethod]
    public async Task RestoreAsync_OrdersEntityから集約込みでOrdersを復元できる()
    {
        var entity = new OrdersEntity
        {
            OrderUuid = Guid.NewGuid(),
            OrderDate = new DateTime(2026, 7, 9, 12, 0, 0),
            AmountTotal = 5000,
            Customer = new CustomerEntity
            {
                CustomerUuid = Guid.NewGuid(),
                Name = "山田太郎",
                Kana = "ヤマダタロウ",
                Address1 = "東京都",
                Address2 = "1-2-3",
                PhoneNumber = "090-1234-5678",
                MailAddress = "test@example.com",
                Username = "yamada",
                Password = "password",
                CreatedAt = new DateTime(2026, 7, 9, 10, 0, 0)
            },
            OrderStatus = new OrderStatusEntity
            {
                Id = 1,
                Name = "注文受付"
            },
            PaymentMethod = new PaymentMethodEntity
            {
                Id = 2,
                Name = "クレジットカード"
            },
            OrderDetails = new List<OrdersDetailEntity>
            {
                CreateOrdersDetailEntity(1, 3, "商品A"),
                CreateOrdersDetailEntity(2, 5, "商品B")
            }
        };

        var result = await _factory.RestoreAsync(entity);

        Assert.IsNotNull(result);
        Assert.AreEqual(entity.OrderUuid, result.OrderUuid);
        Assert.AreEqual(entity.OrderDate, result.OrderDate);
        Assert.AreEqual(entity.AmountTotal, result.AmountTotal);

        Assert.AreEqual(entity.Customer.CustomerUuid, result.Customer.CustomerUuid);
        Assert.AreEqual(entity.Customer.Name, result.Customer.Name);

        Assert.AreEqual(entity.OrderStatus.Id, result.OrderStatus.Id);
        Assert.AreEqual(entity.OrderStatus.Name, result.OrderStatus.Name);

        Assert.AreEqual(entity.PaymentMethod.Name, result.PaymentMethod.Name);

        Assert.IsNotNull(result.OrdersDetails);
        Assert.HasCount(2, result.OrdersDetails);
        Assert.AreEqual(3, result.OrdersDetails[0].Count);
        Assert.AreEqual(5, result.OrdersDetails[1].Count);
    }

    [TestMethod]
    public async Task RestoreAsync_OrdersEntityリストからOrdersリストを復元できる()
    {
        var entities = new List<OrdersEntity>
        {
            CreateOrdersEntity("注文受付", 3),
            CreateOrdersEntity("発送済み", 5)
        };

        var results = await _factory.RestoreAsync(entities);

        Assert.IsNotNull(results);
        Assert.HasCount(2, results);

        Assert.AreEqual(entities[0].OrderUuid, results[0].OrderUuid);
        Assert.AreEqual(entities[0].OrderStatus.Name, results[0].OrderStatus.Name);
        Assert.AreEqual(entities[0].OrderDetails[0].Count, results[0].OrdersDetails[0].Count);

        Assert.AreEqual(entities[1].OrderUuid, results[1].OrderUuid);
        Assert.AreEqual(entities[1].OrderStatus.Name, results[1].OrderStatus.Name);
        Assert.AreEqual(entities[1].OrderDetails[0].Count, results[1].OrdersDetails[0].Count);
    }

    private static OrdersEntity CreateOrdersEntity(string statusName, int count)
    {
        return new OrdersEntity
        {
            OrderUuid = Guid.NewGuid(),
            OrderDate = new DateTime(2026, 7, 9, 12, 0, 0),
            AmountTotal = 5000,
            Customer = new CustomerEntity
            {
                CustomerUuid = Guid.NewGuid(),
                Name = "山田太郎",
                Kana = "ヤマダタロウ",
                Address1 = "東京都",
                Address2 = "1-2-3",
                PhoneNumber = "090-1234-5678",
                MailAddress = "test@example.com",
                Username = "yamada",
                Password = "password",
                CreatedAt = new DateTime(2026, 7, 9, 10, 0, 0)
            },
            OrderStatus = new OrderStatusEntity
            {
                Id = 1,
                Name = statusName
            },
            PaymentMethod = new PaymentMethodEntity
            {
                Id = 2,
                Name = "クレジットカード"
            },
            OrderDetails = new List<OrdersDetailEntity>
            {
                CreateOrdersDetailEntity(
                    1,
                    count,
                    "テスト商品")
            }
        };
    }

    private static OrdersDetailEntity CreateOrdersDetailEntity(
        int id,
        int count,
        string productName)
    {
        return new OrdersDetailEntity
        {
            Id = id,
            Count = count,
            Product = new ProductEntity
            {
                ProductUuid = Guid.NewGuid(),
                Name = productName,
                Price = 1000,
                DeleteFlg = 0
            }
        };
    }

    private static Product CreateProduct(string productName)
    {
        var product = new Product(
            Guid.NewGuid(),
            productName,
            1000
        );

        product.ChangeCategory(
            new ProductCategory(Guid.NewGuid(), "食品")
        );

        product.ChangeStock(
            new ProductStock(Guid.NewGuid(), 10)
        );

        return product;
    }
}