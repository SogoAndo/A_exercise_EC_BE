using A_exercise_EC_BE.Applications.Security;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Infrastructures.Security;
using Moq;

namespace A_exercise_EC_BE.Infrastructures.Tests.Security;

[TestClass]
[TestCategory("Infrastructure/Security")]
public class PBKDF2CustomerPasswordVerifierTests
{
    [TestMethod]
    public void Constructor_WithNullPasswordHashingService_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new PBKDF2CustomerPasswordVerifier(null!));
    }

    [TestMethod]
    public void Verify_WithHashCreatedAtCustomerRegistration_ReturnsTrue()
    {
        var passwordHashingService = new PasswordHashingService();
        var hash = passwordHashingService.Hash("Password123");
        var verifier = new PBKDF2CustomerPasswordVerifier(
            passwordHashingService);

        var result = verifier.Verify(hash, "Password123");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Verify_WithIncorrectPassword_ReturnsFalse()
    {
        var passwordHashingService = new PasswordHashingService();
        var hash = passwordHashingService.Hash("Password123");
        var verifier = new PBKDF2CustomerPasswordVerifier(
            passwordHashingService);

        var result = verifier.Verify(hash, "WrongPassword");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Verify_WithMalformedHash_ReturnsFalse()
    {
        var verifier = new PBKDF2CustomerPasswordVerifier(
            new PasswordHashingService());

        var result = verifier.Verify("not-a-password-hash", "Password123");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Verify_WithoutCustomer_PerformsDummyHashVerificationAndReturnsFalse()
    {
        var passwordHashingServiceMock = CreatePasswordHashingServiceMock();
        var verifier = new PBKDF2CustomerPasswordVerifier(
            passwordHashingServiceMock.Object);

        var result = verifier.Verify(null, "Password123");

        Assert.IsFalse(result);
        passwordHashingServiceMock.Verify(
            service => service.Verify(
                "Password123",
                "dummy-password-hash"),
            Times.Once);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void Verify_WithoutPasswordHash_PerformsDummyHashVerificationAndReturnsFalse(
        string passwordHash)
    {
        var passwordHashingServiceMock = CreatePasswordHashingServiceMock();
        var verifier = new PBKDF2CustomerPasswordVerifier(
            passwordHashingServiceMock.Object);

        var result = verifier.Verify(
            passwordHash,
            "Password123");

        Assert.IsFalse(result);
        passwordHashingServiceMock.Verify(
            service => service.Verify(
                "Password123",
                "dummy-password-hash"),
            Times.Once);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void Verify_WithoutProvidedPassword_ThrowsDomainException(
        string providedPassword)
    {
        var passwordHashingServiceMock = CreatePasswordHashingServiceMock();
        var verifier = new PBKDF2CustomerPasswordVerifier(
            passwordHashingServiceMock.Object);

        Assert.ThrowsExactly<DomainException>(
            () => verifier.Verify("password-hash", providedPassword));
        passwordHashingServiceMock.Verify(
            service => service.Verify(
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    private static Mock<IPasswordHashingService>
        CreatePasswordHashingServiceMock()
    {
        var passwordHashingServiceMock =
            new Mock<IPasswordHashingService>();
        passwordHashingServiceMock
            .Setup(service => service.Hash(
                It.IsAny<string>()))
            .Returns("dummy-password-hash");
        return passwordHashingServiceMock;
    }
}
