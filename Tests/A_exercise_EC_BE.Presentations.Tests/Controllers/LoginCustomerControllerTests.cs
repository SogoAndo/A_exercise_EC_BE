using System.Text.Json;
using A_exercise_EC_BE.Applications.Security;
using A_exercise_EC_BE.Applications.Usecases.Customers;
using A_exercise_EC_BE.Presentations.Controllers;
using A_exercise_EC_BE.Presentations.ViewModels.Authentication;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace A_exercise_EC_BE.Presentations.Tests.Controllers;

[TestClass]
[TestCategory("Presentations/Controllers")]
public class LoginCustomerControllerTests
{
    [TestMethod]
    public async Task LoginAsync_WithValidCredentials_ReturnsAccessToken()
    {
        var customerUuid = Guid.NewGuid();
        var expiresAt = new DateTimeOffset(
            2026,
            7,
            24,
            3,
            0,
            0,
            TimeSpan.Zero);
        var viewModel = new CustomerLoginViewModel
        {
            MailAddress = "taro@example.com",
            Password = "Password123"
        };
        var loginResult = new CustomerLoginResult(
            customerUuid,
            "taro123",
            "山田太郎");
        var accessToken = new CustomerAccessToken(
            "customer-access-token",
            expiresAt);
        var loginUsecaseMock = new Mock<ILoginCustomerUsecase>();
        loginUsecaseMock
            .Setup(usecase => usecase.LoginAsync(
                new CustomerLoginRequest(
                    viewModel.MailAddress,
                    viewModel.Password)))
            .ReturnsAsync(loginResult);
        var tokenIssuerMock =
            new Mock<ICustomerAccessTokenIssuer>();
        tokenIssuerMock
            .Setup(issuer => issuer.Issue(customerUuid))
            .Returns(accessToken);
        var controller = new LoginCustomerController(
            loginUsecaseMock.Object,
            tokenIssuerMock.Object);

        var actionResult = await controller.LoginAsync(
            viewModel);

        var okResult =
            actionResult.Result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(
            200,
            okResult.StatusCode);
        var response =
            okResult.Value as CustomerLoginResponseViewModel;
        Assert.IsNotNull(response);
        Assert.AreEqual(
            accessToken.AccessToken,
            response.AccessToken);
        Assert.AreEqual(
            accessToken.ExpiresAt,
            response.ExpiresAt);
        Assert.AreEqual(
            loginResult.Username,
            response.Username);
        Assert.AreNotEqual(
            loginResult.CustomerName,
            response.Username);
        loginUsecaseMock.Verify(
            usecase => usecase.LoginAsync(
                new CustomerLoginRequest(
                    viewModel.MailAddress,
                    viewModel.Password)),
            Times.Once);
        tokenIssuerMock.Verify(
            issuer => issuer.Issue(customerUuid),
            Times.Once);
    }

    [TestMethod]
    public void CustomerLoginResponse_WithWebJsonDefaults_UsesUsername()
    {
        var response = new CustomerLoginResponseViewModel(
            "customer-access-token",
            new DateTimeOffset(
                2026,
                7,
                24,
                3,
                0,
                0,
                TimeSpan.Zero),
            "taro123");
        var options = new JsonSerializerOptions(
            JsonSerializerDefaults.Web);

        var json = JsonSerializer.Serialize(
            response,
            options);
        using var jsonDocument = JsonDocument.Parse(json);
        var root = jsonDocument.RootElement;

        Assert.AreEqual(
            "taro123",
            root.GetProperty("username").GetString());
        Assert.IsTrue(
            root.TryGetProperty(
                "accessToken",
                out _));
        Assert.IsTrue(
            root.TryGetProperty(
                "expiresAt",
                out _));
        Assert.IsFalse(
            root.TryGetProperty(
                "customerName",
                out _));
    }

    [TestMethod]
    public async Task LoginAsync_WhenAuthenticationFails_DoesNotIssueToken()
    {
        var viewModel = new CustomerLoginViewModel
        {
            MailAddress = "taro@example.com",
            Password = "WrongPassword"
        };
        var loginUsecaseMock = new Mock<ILoginCustomerUsecase>();
        loginUsecaseMock
            .Setup(usecase => usecase.LoginAsync(
                It.IsAny<CustomerLoginRequest>()))
            .ThrowsAsync(
                new UnauthorizedAccessException(
                    "メールアドレスまたはパスワードが正しくありません。"));
        var tokenIssuerMock =
            new Mock<ICustomerAccessTokenIssuer>();
        var controller = new LoginCustomerController(
            loginUsecaseMock.Object,
            tokenIssuerMock.Object);

        var exception =
            await Assert.ThrowsExactlyAsync<UnauthorizedAccessException>(
                () => controller.LoginAsync(viewModel));
        Assert.AreEqual(
            "メールアドレスまたはパスワードが正しくありません。",
            exception.Message);
        tokenIssuerMock.Verify(
            issuer => issuer.Issue(It.IsAny<Guid>()),
            Times.Never);
    }
}
