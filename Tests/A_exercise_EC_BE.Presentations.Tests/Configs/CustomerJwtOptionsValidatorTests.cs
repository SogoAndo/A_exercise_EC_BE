using A_exercise_EC_BE.Infrastructures.Security;
using A_exercise_EC_BE.Presentations.Configs;

namespace A_exercise_EC_BE.Presentations.Tests.Configs;

[TestClass]
[TestCategory("Presentation/Configs")]
public class CustomerJwtOptionsValidatorTests
{
    private const string ValidSigningKey =
        "customer-only-signing-key-0123456789-abcdef";

    [TestMethod]
    public void Validate_WithValidOptions_ReturnsSuccess()
    {
        var validator = new CustomerJwtOptionsValidator();
        var options = CreateValidOptions();

        var result = validator.Validate(
            null,
            options);

        Assert.IsTrue(result.Succeeded);
    }

    [TestMethod]
    public void Validate_WithoutIssuer_ReturnsFailure()
    {
        var validator = new CustomerJwtOptionsValidator();
        var options = new CustomerJwtOptions
        {
            Audience = "FullnessEcCustomer",
            SigningKey = ValidSigningKey
        };

        var result = validator.Validate(
            null,
            options);

        Assert.IsTrue(result.Failed);
        CollectionAssert.Contains(
            result.Failures.ToList(),
            "CustomerJwt:Issuerを設定してください。");
    }

    [TestMethod]
    public void Validate_WithShortSigningKey_ReturnsFailure()
    {
        var validator = new CustomerJwtOptionsValidator();
        var options = new CustomerJwtOptions
        {
            Issuer = "FullnessEcApi",
            Audience = "FullnessEcCustomer",
            SigningKey = "short-key"
        };

        var result = validator.Validate(
            null,
            options);

        Assert.IsTrue(result.Failed);
        CollectionAssert.Contains(
            result.Failures.ToList(),
            "CustomerJwt:SigningKeyは32バイト以上で設定してください。");
    }

    private static CustomerJwtOptions CreateValidOptions()
        => new()
        {
            Issuer = "FullnessEcApi",
            Audience = "FullnessEcCustomer",
            SigningKey = ValidSigningKey
        };
}
