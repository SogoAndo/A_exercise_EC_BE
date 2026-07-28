using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using A_exercise_EC_BE.Applications.Usecases.Purchases;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.Authentication;
using A_exercise_EC_BE.Presentations.Controllers;
using A_exercise_EC_BE.Presentations.ViewModels.Purchases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace A_exercise_EC_BE.Presentations.Tests.Controllers;

[TestClass]
[TestCategory("Presentations/Controllers")]
public class PurchaseHistoryControllersTests
{
    [TestMethod(DisplayName = "購入履歴一覧を200で返す")]
    public async Task ListGetAsync_ReturnsPurchaseHistory()
    {
        var customerUuid = Guid.NewGuid();
        var order = CreateOrder(customerUuid);
        var usecase = new Mock<ISearchPurchaseHistoryUsecase>();
        usecase
            .Setup(x => x.SearchAsync(customerUuid))
            .ReturnsAsync([order]);
        var controller = new PurchaseHistoryListController(
            usecase.Object,
            new PurchaseHistoryViewModelAdapter());
        SetCustomer(controller, customerUuid);

        var actionResult = await controller.GetAsync();

        var ok = actionResult.Result as OkObjectResult;
        Assert.IsNotNull(ok);
        var response =
            ok.Value as PurchaseHistoryResultViewModel;
        Assert.IsNotNull(response);
        Assert.HasCount(1, response.OrderList);
        Assert.AreEqual(
            order.OrderUuid,
            response.OrderList[0].OrderUuid);
        Assert.AreEqual(
            order.OrderStatus.Name,
            response.OrderList[0].OrderStatus);
        Assert.AreEqual(
            order.OrderDate.ToString("yyyy/MM/dd HH:mm:ss"),
            response.OrderList[0].OrderDate);
        Assert.AreEqual(
            order.AmountTotal,
            response.OrderList[0].TotalPrice);
        Assert.AreEqual(
            $"/purchase/history/{order.OrderUuid:D}",
            response.OrderList[0].DetailUrl);
        Assert.IsNull(response.Message);
        usecase.Verify(
            x => x.SearchAsync(customerUuid),
            Times.Once);
    }

    [TestMethod(DisplayName = "購入履歴0件を空配列とメッセージで返す")]
    public async Task ListGetAsync_WhenEmpty_ReturnsEmptyResult()
    {
        var customerUuid = Guid.NewGuid();
        var usecase = new Mock<ISearchPurchaseHistoryUsecase>();
        usecase
            .Setup(x => x.SearchAsync(customerUuid))
            .ReturnsAsync([]);
        var controller = new PurchaseHistoryListController(
            usecase.Object,
            new PurchaseHistoryViewModelAdapter());
        SetCustomer(controller, customerUuid);

        var actionResult = await controller.GetAsync();

        var ok = actionResult.Result as OkObjectResult;
        Assert.IsNotNull(ok);
        var response =
            ok.Value as PurchaseHistoryResultViewModel;
        Assert.IsNotNull(response);
        Assert.IsEmpty(response.OrderList);
        Assert.AreEqual(
            "購入履歴がありません。",
            response.Message);
    }

    [TestMethod(DisplayName = "購入履歴詳細を200で返す")]
    public async Task DetailGetAsync_ReturnsPurchaseHistoryDetail()
    {
        var customerUuid = Guid.NewGuid();
        var order = CreateOrder(customerUuid);
        var usecase = new Mock<ISearchPurchaseHistoryUsecase>();
        usecase
            .Setup(x => x.FindDetailAsync(
                customerUuid,
                order.OrderUuid))
            .ReturnsAsync(order);
        var controller = new PurchaseHistoryDetailController(
            usecase.Object,
            new PurchaseHistoryViewModelAdapter());
        SetCustomer(controller, customerUuid);

        var actionResult = await controller.GetAsync(
            order.OrderUuid);

        var ok = actionResult.Result as OkObjectResult;
        Assert.IsNotNull(ok);
        var response =
            ok.Value as PurchaseHistoryDetailViewModel;
        Assert.IsNotNull(response);
        Assert.AreEqual(order.OrderUuid, response.OrderUuid);
        Assert.AreEqual(
            order.OrderDate.ToString("yyyy/MM/dd HH:mm:ss"),
            response.OrderDate);
        Assert.AreEqual(order.AmountTotal, response.TotalPrice);
        Assert.AreEqual(
            order.OrderStatus.Id,
            response.OrderStatusId);
        Assert.AreEqual(
            order.OrderStatus.Name,
            response.OrderStatusName);
        Assert.HasCount(1, response.OrderItems);
        Assert.AreEqual(
            order.OrdersDetails[0].Product.ProductUuid,
            response.OrderItems[0].ProductUuid);
        Assert.AreEqual(
            order.OrdersDetails[0].Product.Name,
            response.OrderItems[0].ProductName);
        Assert.AreEqual(
            order.OrdersDetails[0].Product.Price,
            response.OrderItems[0].Price);
        Assert.AreEqual(
            order.OrdersDetails[0].Count,
            response.OrderItems[0].Quantity);
        Assert.AreEqual(200, response.OrderItems[0].Subtotal);
    }

    [TestMethod(DisplayName = "購入履歴詳細がない場合は404を返す")]
    public async Task DetailGetAsync_WhenNotFound_Returns404()
    {
        var customerUuid = Guid.NewGuid();
        var orderUuid = Guid.NewGuid();
        var usecase = new Mock<ISearchPurchaseHistoryUsecase>();
        usecase
            .Setup(x => x.FindDetailAsync(
                customerUuid,
                orderUuid))
            .ReturnsAsync((Orders?)null);
        var controller = new PurchaseHistoryDetailController(
            usecase.Object,
            new PurchaseHistoryViewModelAdapter());
        SetCustomer(controller, customerUuid);

        var actionResult = await controller.GetAsync(orderUuid);

        Assert.IsInstanceOfType<NotFoundObjectResult>(
            actionResult.Result);
        var notFound = (NotFoundObjectResult)actionResult.Result;
        var message = notFound.Value?
            .GetType()
            .GetProperty("message")?
            .GetValue(notFound.Value);
        Assert.AreEqual(
            "購入履歴が見つかりませんでした。",
            message);
        usecase.Verify(
            x => x.FindDetailAsync(customerUuid, orderUuid),
            Times.Once);
    }

    [TestMethod(DisplayName = "顧客UUID claimがない場合は認証例外にする")]
    public async Task ListGetAsync_WhenSubjectIsMissing_ThrowsUnauthorized()
    {
        var usecase = new Mock<ISearchPurchaseHistoryUsecase>();
        var controller = new PurchaseHistoryListController(
            usecase.Object,
            new PurchaseHistoryViewModelAdapter())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        await Assert.ThrowsExactlyAsync<UnauthorizedAccessException>(
            () => controller.GetAsync());

        usecase.Verify(
            x => x.SearchAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [TestMethod(DisplayName = "購入履歴詳細は顧客UUID claimがない場合に認証例外にする")]
    public async Task DetailGetAsync_WhenSubjectIsMissing_ThrowsUnauthorized()
    {
        var usecase = new Mock<ISearchPurchaseHistoryUsecase>();
        var controller = new PurchaseHistoryDetailController(
            usecase.Object,
            new PurchaseHistoryViewModelAdapter())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        await Assert.ThrowsExactlyAsync<UnauthorizedAccessException>(
            () => controller.GetAsync(Guid.NewGuid()));

        usecase.Verify(
            x => x.FindDetailAsync(
                It.IsAny<Guid>(),
                It.IsAny<Guid>()),
            Times.Never);
    }

    [TestMethod(DisplayName = "不正な顧客UUID claimでは購入履歴を検索しない")]
    public async Task ListGetAsync_WhenSubjectIsInvalid_ThrowsUnauthorized()
    {
        var usecase = new Mock<ISearchPurchaseHistoryUsecase>();
        var controller = new PurchaseHistoryListController(
            usecase.Object,
            new PurchaseHistoryViewModelAdapter());
        SetCustomerSubject(controller, "invalid-customer-uuid");

        await Assert.ThrowsExactlyAsync<UnauthorizedAccessException>(
            () => controller.GetAsync());

        usecase.Verify(
            x => x.SearchAsync(It.IsAny<Guid>()),
            Times.Never);
    }

    [TestMethod(DisplayName = "購入履歴の商品名はJSONで安全にエスケープされる")]
    public async Task DetailGetAsync_WhenProductNameContainsHtml_EscapesJson()
    {
        var customerUuid = Guid.NewGuid();
        var order = CreateOrder(
            customerUuid,
            "<script>alert('xss')</script>");
        var usecase = new Mock<ISearchPurchaseHistoryUsecase>();
        usecase
            .Setup(x => x.FindDetailAsync(
                customerUuid,
                order.OrderUuid))
            .ReturnsAsync(order);
        var controller = new PurchaseHistoryDetailController(
            usecase.Object,
            new PurchaseHistoryViewModelAdapter());
        SetCustomer(controller, customerUuid);

        var actionResult = await controller.GetAsync(
            order.OrderUuid);
        var ok = actionResult.Result as OkObjectResult;
        Assert.IsNotNull(ok);

        var json = JsonSerializer.Serialize(ok.Value);

        Assert.IsFalse(
            json.Contains(
                "<script>",
                StringComparison.Ordinal));
        StringAssert.Contains(
            json,
            "\\u003Cscript\\u003E");
    }

    [TestMethod(DisplayName = "一覧・詳細Controllerのルートと顧客認証を設定する")]
    public void Controllers_UseExpectedRoutesAndAuthentication()
    {
        AssertControllerContract(
            typeof(PurchaseHistoryListController),
            nameof(PurchaseHistoryListController.GetAsync),
            null);
        AssertControllerContract(
            typeof(PurchaseHistoryDetailController),
            nameof(PurchaseHistoryDetailController.GetAsync),
            "{orderUuid:guid}");
    }

    private static void AssertControllerContract(
        Type controllerType,
        string methodName,
        string? actionTemplate)
    {
        var route = controllerType.GetCustomAttribute<RouteAttribute>();
        var method = controllerType.GetMethod(methodName)
            ?? throw new InvalidOperationException(
                "購入履歴Actionが見つかりません。");
        var httpGet = method.GetCustomAttribute<HttpGetAttribute>();
        var authorize =
            method.GetCustomAttribute<AuthorizeAttribute>();

        Assert.AreEqual("purchase/history", route?.Template);
        Assert.AreEqual(actionTemplate, httpGet?.Template);
        Assert.AreEqual(
            CustomerJwtAuthenticationDefaults.AuthenticationScheme,
            authorize?.AuthenticationSchemes);
    }

    private static void SetCustomer(
        ControllerBase controller,
        Guid customerUuid)
        => SetCustomerSubject(
            controller,
            customerUuid.ToString("D"));

    private static void SetCustomerSubject(
        ControllerBase controller,
        string subject)
    {
        var identity = new ClaimsIdentity(
            [
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    subject)
            ],
            CustomerJwtAuthenticationDefaults.AuthenticationScheme);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(identity)
            }
        };
    }

    private static Orders CreateOrder(
        Guid customerUuid,
        string productName = "ボールペン")
    {
        var product = new Product(
            Guid.NewGuid(),
            productName,
            100);
        var customer = new Customer(
            customerUuid,
            "顧客太郎",
            "コキャクタロウ",
            "東京都千代田区",
            null,
            "09012345678",
            "customer@example.com",
            "customer",
            "hashed-password",
            DateTime.Now.AddDays(-2));

        return new Orders(
            Guid.NewGuid(),
            DateTime.Now.AddDays(-1),
            200,
            customer,
            new OrderStatus(1, "受付済み"),
            new PaymentMethod(1, "クレジットカード"),
            [
                new OrdersDetail(1, product, 2)
            ]);
    }
}
