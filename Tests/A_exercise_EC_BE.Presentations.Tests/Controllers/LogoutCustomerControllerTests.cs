using System.Reflection;
using A_exercise_EC_BE.Applications.Usecases.Customers;
using A_exercise_EC_BE.Presentations.Authentication;
using A_exercise_EC_BE.Presentations.Controllers;
using A_exercise_EC_BE.Presentations.ViewModels.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace A_exercise_EC_BE.Presentations.Tests.Controllers;

[TestClass]
[TestCategory("Presentations/Controllers")]
public class LogoutCustomerControllerTests
{
    [TestMethod]
    public async Task LogoutAsync_ReturnsLoggedOutResponse()
    {
        var logoutUsecaseMock =
            new Mock<ILogoutCustomerUsecase>();
        logoutUsecaseMock
            .Setup(usecase => usecase.LogoutAsync())
            .ReturnsAsync(
                CustomerLogoutResult.CreateLoggedOut());
        var controller = new LogoutCustomerController(
            logoutUsecaseMock.Object);

        var actionResult = await controller.LogoutAsync();

        var okResult =
            actionResult.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        var response =
            okResult.Value
                as CustomerLogoutResponseViewModel;
        Assert.IsNotNull(response);
        Assert.IsTrue(response.LoggedOut);
        logoutUsecaseMock.Verify(
            usecase => usecase.LogoutAsync(),
            Times.Once);
    }

    [TestMethod]
    public void LogoutAsync_UsesExpectedRoute()
    {
        var controllerRoute = typeof(
            LogoutCustomerController)
            .GetCustomAttribute<RouteAttribute>();
        var logoutMethod = GetLogoutMethod();
        var httpPost =
            logoutMethod.GetCustomAttribute<
                HttpPostAttribute>();

        Assert.IsNotNull(controllerRoute);
        Assert.AreEqual(
            "/",
            controllerRoute.Template);
        Assert.IsNotNull(httpPost);
        Assert.AreEqual(
            "logout",
            httpPost.Template);
    }

    [TestMethod]
    public void LogoutAsync_RequiresCustomerJwtAuthentication()
    {
        var authorize = GetLogoutMethod()
            .GetCustomAttribute<AuthorizeAttribute>();

        Assert.IsNotNull(authorize);
        Assert.AreEqual(
            CustomerJwtAuthenticationDefaults
                .AuthenticationScheme,
            authorize.AuthenticationSchemes);
    }

    private static MethodInfo GetLogoutMethod()
        => typeof(LogoutCustomerController).GetMethod(
            nameof(LogoutCustomerController.LogoutAsync))
            ?? throw new InvalidOperationException(
                "顧客ログアウトActionが見つかりません。");
}
