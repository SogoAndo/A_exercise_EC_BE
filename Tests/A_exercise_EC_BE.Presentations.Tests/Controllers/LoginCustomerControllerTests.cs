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
        var response =
            okResult.Value as CustomerLoginResponseViewModel;
        Assert.IsNotNull(response);
        Assert.AreEqual(
            accessToken.AccessToken,
            response.AccessToken);
        Assert.AreEqual(
            accessToken.ExpiresAt,
            response.ExpiresAt);
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

        await Assert.ThrowsExactlyAsync<UnauthorizedAccessException>(
            () => controller.LoginAsync(viewModel));
        tokenIssuerMock.Verify(
            issuer => issuer.Issue(It.IsAny<Guid>()),
            Times.Never);
    }
}
