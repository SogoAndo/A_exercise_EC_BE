using A_exercise_EC_BE.Application.Security;
using A_exercise_EC_BE.Application.Usecases.Customers;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Domain.Repositories;
using Moq;

namespace A_exercise_EC_BE.Application.Tests.Usecases.Customers;

[TestClass]
[TestCategory("Application/Usecases")]
public class LoginCustomerUsecaseTests
{
    private Mock<ICustomerRepository> _repositoryMock = null!;
    private Mock<ICustomerPasswordVerifier> _passwordVerifierMock = null!;
    private LoginCustomerUsecase _usecase = null!;

    [TestInitialize]
    public void Initialize()
    {
        _repositoryMock = new Mock<ICustomerRepository>();
        _passwordVerifierMock = new Mock<ICustomerPasswordVerifier>();
        _usecase = new LoginCustomerUsecase(
            _repositoryMock.Object,
            _passwordVerifierMock.Object);
    }

    [TestMethod]
    public async Task LoginAsync_WithValidCredentials_ReturnsCustomer()
    {
        var customer = CreateCustomer();
        var request = new CustomerLoginRequest(
            customer.MailAddress,
            "Password123");
        _repositoryMock
            .Setup(repository => repository.FindByMailAddressAsync(request.MailAddress))
            .ReturnsAsync(customer);
        _passwordVerifierMock
            .Setup(verifier => verifier.Verify(customer.Password, request.Password))
            .Returns(true);

        var result = await _usecase.LoginAsync(request);

        Assert.AreEqual(customer.CustomerUuid, result.CustomerUuid);
        Assert.AreEqual(customer.Username, result.Username);
        Assert.AreEqual(customer.Name, result.CustomerName);
        _repositoryMock.Verify(
            repository => repository.FindByMailAddressAsync(request.MailAddress),
            Times.Once);
        _passwordVerifierMock.Verify(
            verifier => verifier.Verify(customer.Password, request.Password),
            Times.Once);
    }

    [TestMethod]
    public async Task LoginAsync_WithNullRequest_ThrowsDomainException()
    {
        await Assert.ThrowsExactlyAsync<DomainException>(
            () => _usecase.LoginAsync(null!));

        VerifyDependenciesWereNotCalled();
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("invalid-address")]
    [DataRow("taro@example.com extra")]
    public async Task LoginAsync_WithInvalidMailAddress_ThrowsDomainException(
        string mailAddress)
    {
        var request = new CustomerLoginRequest(mailAddress, "Password123");

        await Assert.ThrowsExactlyAsync<DomainException>(
            () => _usecase.LoginAsync(request));

        VerifyDependenciesWereNotCalled();
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("1234")]
    [DataRow("123456789012345678901")]
    public async Task LoginAsync_WithInvalidPassword_ThrowsDomainException(
        string password)
    {
        var request = new CustomerLoginRequest("taro@example.com", password);

        await Assert.ThrowsExactlyAsync<DomainException>(
            () => _usecase.LoginAsync(request));

        VerifyDependenciesWereNotCalled();
    }

    [TestMethod]
    public async Task LoginAsync_WithUnknownMailAddress_ThrowsAuthenticationFailure()
    {
        var request = new CustomerLoginRequest(
            "nobody@example.com",
            "Password123");
        _repositoryMock
            .Setup(repository => repository.FindByMailAddressAsync(request.MailAddress))
            .ReturnsAsync((Customer?)null);
        _passwordVerifierMock
            .Setup(verifier => verifier.Verify(null, request.Password))
            .Returns(false);

        var exception = await Assert.ThrowsExactlyAsync<UnauthorizedAccessException>(
            () => _usecase.LoginAsync(request));

        Assert.AreEqual(
            "メールアドレスまたはパスワードが正しくありません。",
            exception.Message);
        _passwordVerifierMock.Verify(
            verifier => verifier.Verify(null, request.Password),
            Times.Once);
    }

    [TestMethod]
    public async Task LoginAsync_WithIncorrectPassword_ThrowsSameAuthenticationFailure()
    {
        var customer = CreateCustomer();
        var request = new CustomerLoginRequest(
            customer.MailAddress,
            "WrongPassword");
        _repositoryMock
            .Setup(repository => repository.FindByMailAddressAsync(request.MailAddress))
            .ReturnsAsync(customer);
        _passwordVerifierMock
            .Setup(verifier => verifier.Verify(customer.Password, request.Password))
            .Returns(false);

        var exception = await Assert.ThrowsExactlyAsync<UnauthorizedAccessException>(
            () => _usecase.LoginAsync(request));

        Assert.AreEqual(
            "メールアドレスまたはパスワードが正しくありません。",
            exception.Message);
    }

    private void VerifyDependenciesWereNotCalled()
    {
        _repositoryMock.Verify(
            repository => repository.FindByMailAddressAsync(It.IsAny<string>()),
            Times.Never);
        _passwordVerifierMock.Verify(
            verifier => verifier.Verify(It.IsAny<string?>(), It.IsAny<string>()),
            Times.Never);
    }

    private static Customer CreateCustomer() => new(
        Guid.NewGuid(),
        "山田太郎",
        "ヤマダタロウ",
        "東京都千代田区",
        null,
        "09012345678",
        "taro@example.com",
        "taro",
        "hashed-password",
        new DateTime(2026, 7, 23, 10, 0, 0));
}
