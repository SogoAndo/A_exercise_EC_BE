using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace A_exercise_EC_BE.Infrastructure.Tests.Security;

[TestClass]
[TestCategory("Infrastructure/Security")]
public class PBKDF2CustomerPasswordVerifierTests
{
    [TestMethod]
    public void Verify_WithMatchingIdentityV3Hash_ReturnsTrue()
    {
        var passwordHasher = new PasswordHasher<CustomerPasswordContext>();
        var context = new CustomerPasswordContext();
        var hash = passwordHasher.HashPassword(context, "Password123");
        var verifier = new PBKDF2CustomerPasswordVerifier(passwordHasher);

        var result = verifier.Verify(hash, "Password123");

        Assert.IsTrue(result);
    }

    [TestMethod]
    public void Verify_WithIncorrectPassword_ReturnsFalse()
    {
        var passwordHasher = new PasswordHasher<CustomerPasswordContext>();
        var context = new CustomerPasswordContext();
        var hash = passwordHasher.HashPassword(context, "Password123");
        var verifier = new PBKDF2CustomerPasswordVerifier(passwordHasher);

        var result = verifier.Verify(hash, "WrongPassword");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Verify_WithMalformedHash_ReturnsFalse()
    {
        var verifier = new PBKDF2CustomerPasswordVerifier(
            new PasswordHasher<CustomerPasswordContext>());

        var result = verifier.Verify("not-a-password-hash", "Password123");

        Assert.IsFalse(result);
    }

    [TestMethod]
    public void Verify_WithoutCustomer_PerformsDummyHashVerificationAndReturnsFalse()
    {
        var passwordHasher = new PasswordHasher<CustomerPasswordContext>();
        var verifier = new PBKDF2CustomerPasswordVerifier(passwordHasher);

        var result = verifier.Verify(null, "Password123");

        Assert.IsFalse(result);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void Verify_WithoutPasswordHash_ThrowsDomainException(string passwordHash)
    {
        var passwordHasherMock = new Mock<IPasswordHasher<CustomerPasswordContext>>();
        passwordHasherMock
            .Setup(hasher => hasher.HashPassword(
                It.IsAny<CustomerPasswordContext>(),
                It.IsAny<string>()))
            .Returns("dummy-password-hash");
        var verifier = new PBKDF2CustomerPasswordVerifier(passwordHasherMock.Object);

        Assert.ThrowsExactly<DomainException>(
            () => verifier.Verify(passwordHash, "Password123"));
        passwordHasherMock.Verify(
            hasher => hasher.VerifyHashedPassword(
                It.IsAny<CustomerPasswordContext>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void Verify_WithoutProvidedPassword_ThrowsDomainException(
        string providedPassword)
    {
        var passwordHasherMock = new Mock<IPasswordHasher<CustomerPasswordContext>>();
        passwordHasherMock
            .Setup(hasher => hasher.HashPassword(
                It.IsAny<CustomerPasswordContext>(),
                It.IsAny<string>()))
            .Returns("dummy-password-hash");
        var verifier = new PBKDF2CustomerPasswordVerifier(passwordHasherMock.Object);

        Assert.ThrowsExactly<DomainException>(
            () => verifier.Verify("password-hash", providedPassword));
        passwordHasherMock.Verify(
            hasher => hasher.VerifyHashedPassword(
                It.IsAny<CustomerPasswordContext>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [TestMethod]
    public void Verify_WhenRehashIsNeeded_ReturnsTrue()
    {
        var passwordHasherMock = new Mock<IPasswordHasher<CustomerPasswordContext>>();
        passwordHasherMock
            .Setup(hasher => hasher.HashPassword(
                It.IsAny<CustomerPasswordContext>(),
                It.IsAny<string>()))
            .Returns("dummy-password-hash");
        passwordHasherMock
            .Setup(hasher => hasher.VerifyHashedPassword(
                It.IsAny<CustomerPasswordContext>(),
                "old-password-hash",
                "Password123"))
            .Returns(PasswordVerificationResult.SuccessRehashNeeded);
        var verifier = new PBKDF2CustomerPasswordVerifier(passwordHasherMock.Object);

        var result = verifier.Verify("old-password-hash", "Password123");

        Assert.IsTrue(result);
    }
}
