using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using A_exercise_EC_BE.Applications.Usecases.Purchases;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.Authentication;
using A_exercise_EC_BE.Presentations.Controllers;
using A_exercise_EC_BE.Presentations.ViewModels.Purchases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace A_exercise_EC_BE.Presentations.Tests.Controllers;

/// <summary>
/// ConfirmPurchaseControllerの単体テスト。
/// </summary>
[TestClass]
[TestCategory("Presentations/Controllers")]
public class ConfirmPurchaseControllerTests
{
    /// <summary>
    /// 認証済み顧客の購入を確定できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "有効な購入情報の場合は注文情報を201で返す")]
    public async Task
        CompleteAsync_WhenRequestIsValid_ReturnsCreated()
    {
        var customerUuid =
            Guid.Parse(
                "11111111-1111-1111-1111-111111111111");
        var productUuid =
            Guid.Parse(
                "22222222-2222-2222-2222-222222222222");
        var orderUuid =
            Guid.Parse(
                "33333333-3333-3333-3333-333333333333");
        var orderDate =
            new DateTime(
                2026,
                7,
                27,
                10,
                30,
                0);
        var viewModel =
            new ConfirmPurchaseViewModel
            {
                PaymentMethodId = 4,
                Items =
                [
                    new ConfirmPurchaseItemViewModel
                    {
                        ProductUuid = productUuid,
                        Quantity = 2
                    }
                ]
            };
        var usecase =
            new Mock<IConfirmPurchaseUsecase>();
        usecase
            .Setup(target =>
                target.ConfirmAsync(
                    It.Is<ConfirmPurchaseRequest>(
                        request =>
                            request.CustomerUuid
                                == customerUuid
                            && request.PaymentMethodId
                                == 4
                            && request.Items.Count
                                == 1
                            && request.Items.First()
                                .ProductUuid
                                == productUuid
                            && request.Items.First()
                                .Quantity
                                == 2)))
            .ReturnsAsync(
                new ConfirmPurchaseResult(
                    orderUuid,
                    orderDate,
                    240));
        var controller =
            new ConfirmPurchaseController(
                usecase.Object,
                new PurchaseViewModelAdapter());
        SetCustomer(
            controller,
            customerUuid);

        var actionResult =
            await controller.CompleteAsync(
                viewModel);

        var created =
            actionResult.Result
                as CreatedResult;
        Assert.IsNotNull(created);
        Assert.AreEqual(
            $"/purchase/history/{orderUuid:D}",
            created.Location);

        var response =
            created.Value
                as PurchaseCompleteViewModel;
        Assert.IsNotNull(response);
        Assert.AreEqual(
            "購入が完了しました",
            response.CompleteMessage);
        Assert.AreEqual(
            orderUuid,
            response.OrderUuid);
        Assert.AreEqual(
            "2026/07/27 10:30:00",
            response.OrderDate);
        Assert.AreEqual(
            240,
            response.TotalPrice);

        usecase.Verify(
            target =>
                target.ConfirmAsync(
                    It.IsAny<
                        ConfirmPurchaseRequest>()),
            Times.Once);
    }

    /// <summary>
    /// 顧客UUID claimがなければ
    /// 購入処理を開始しないこと。
    /// </summary>
    [TestMethod(
        DisplayName =
            "顧客UUID claimがない場合は認証例外にする")]
    public async Task
        CompleteAsync_WhenSubjectIsMissing_ThrowsUnauthorized()
    {
        var usecase =
            new Mock<IConfirmPurchaseUsecase>();
        var controller =
            new ConfirmPurchaseController(
                usecase.Object,
                new PurchaseViewModelAdapter())
            {
                ControllerContext =
                    new ControllerContext
                    {
                        HttpContext =
                            new DefaultHttpContext()
                    }
            };

        await Assert
            .ThrowsExactlyAsync<
                UnauthorizedAccessException>(
                () =>
                    controller.CompleteAsync(
                        new ConfirmPurchaseViewModel
                        {
                            PaymentMethodId = 4,
                            Items =
                            [
                                new ConfirmPurchaseItemViewModel
                                {
                                    ProductUuid =
                                        Guid.NewGuid(),
                                    Quantity = 1
                                }
                            ]
                        }));

        usecase.Verify(
            target =>
                target.ConfirmAsync(
                    It.IsAny<
                        ConfirmPurchaseRequest>()),
            Times.Never);
    }

    /// <summary>
    /// 仕様書のURLと顧客認証を設定すること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "購入完了Actionのルートと顧客認証を設定する")]
    public void
        CompleteAsync_UsesExpectedRouteAndAuthentication()
    {
        var controllerType =
            typeof(ConfirmPurchaseController);
        var route =
            controllerType
                .GetCustomAttribute<
                    RouteAttribute>();
        var method =
            controllerType.GetMethod(
                nameof(
                    ConfirmPurchaseController
                        .CompleteAsync))
            ?? throw new InvalidOperationException(
                "購入確定Actionが見つかりません。");
        var httpPost =
            method.GetCustomAttribute<
                HttpPostAttribute>();
        var authorize =
            method.GetCustomAttribute<
                AuthorizeAttribute>();

        Assert.AreEqual(
            "purchase",
            route?.Template);
        Assert.AreEqual(
            "complete",
            httpPost?.Template);
        Assert.AreEqual(
            CustomerJwtAuthenticationDefaults
                .AuthenticationScheme,
            authorize?.AuthenticationSchemes);
    }

    private static void SetCustomer(
        ControllerBase controller,
        Guid customerUuid)
    {
        var identity =
            new ClaimsIdentity(
                [
                    new Claim(
                        JwtRegisteredClaimNames.Sub,
                        customerUuid.ToString("D"))
                ],
                CustomerJwtAuthenticationDefaults
                    .AuthenticationScheme);

        controller.ControllerContext =
            new ControllerContext
            {
                HttpContext =
                    new DefaultHttpContext
                    {
                        User =
                            new ClaimsPrincipal(
                                identity)
                    }
            };
    }
}
