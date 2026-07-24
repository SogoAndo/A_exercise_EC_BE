using System.Text;
using A_exercise_EC_BE.Applications.Security;
using A_exercise_EC_BE.Applications.Usecases.Customers;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Infrastructures.Repositories;
using A_exercise_EC_BE.Infrastructures.Security;
using A_exercise_EC_BE.Presentations.Authentication;
using A_exercise_EC_BE.Presentations.Configs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace A_exercise_EC_BE.Presentations.Tests.Configs;

[TestClass]
[TestCategory("Presentation/Configs")]
public class ApplicationDependencyExtensionsTests
{
    private const string Issuer = "https://ec.example.test";
    private const string Audience = "https://ec.example.test";
    private const string SigningKey =
        "customer-only-signing-key-0123456789-abcdef";

    [TestMethod]
    public void BuildAppProvider_RegistersCustomerLoginDependencies()
    {
        using var provider =
            ApplicationDependencyExtensions.BuildAppProvider(
                CreateConfiguration());
        using var scope = provider.CreateScope();
        var services = scope.ServiceProvider;

        Assert.IsInstanceOfType<CustomerRepository>(
            services.GetRequiredService<ICustomerRepository>());
        Assert.IsInstanceOfType<PBKDF2CustomerPasswordVerifier>(
            services.GetRequiredService<ICustomerPasswordVerifier>());
        Assert.IsInstanceOfType<CustomerJwtTokenIssuer>(
            services.GetRequiredService<ICustomerAccessTokenIssuer>());
        Assert.IsInstanceOfType<LoginCustomerUsecase>(
            services.GetRequiredService<ILoginCustomerUsecase>());
    }

    [TestMethod]
    public void BuildAppProvider_RegistersCustomerLogoutDependencies()
    {
        using var provider =
            ApplicationDependencyExtensions.BuildAppProvider(
                CreateConfiguration());
        using var scope = provider.CreateScope();
        var services = scope.ServiceProvider;

        Assert.IsInstanceOfType<LogoutCustomerUsecase>(
            services.GetRequiredService<ILogoutCustomerUsecase>());
    }

    [TestMethod]
    public async Task BuildAppProvider_ConfiguresCustomerJwtAuthentication()
    {
        using var provider =
            ApplicationDependencyExtensions.BuildAppProvider(
                CreateConfiguration());
        var schemeProvider =
            provider.GetRequiredService<IAuthenticationSchemeProvider>();

        var scheme = await schemeProvider.GetSchemeAsync(
            CustomerJwtAuthenticationDefaults.AuthenticationScheme);
        var defaultAuthenticateScheme =
            await schemeProvider.GetDefaultAuthenticateSchemeAsync();
        var defaultChallengeScheme =
            await schemeProvider.GetDefaultChallengeSchemeAsync();
        var options = provider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(CustomerJwtAuthenticationDefaults.AuthenticationScheme);
        var validation = options.TokenValidationParameters;

        Assert.IsNotNull(scheme);
        Assert.AreEqual(
            CustomerJwtAuthenticationDefaults.AuthenticationScheme,
            defaultAuthenticateScheme?.Name);
        Assert.AreEqual(
            CustomerJwtAuthenticationDefaults.AuthenticationScheme,
            defaultChallengeScheme?.Name);
        Assert.AreEqual(Issuer, validation.ValidIssuer);
        Assert.AreEqual(Audience, validation.ValidAudience);
        Assert.AreEqual(TimeSpan.Zero, validation.ClockSkew);
        Assert.IsTrue(validation.ValidateIssuerSigningKey);
        Assert.IsTrue(validation.ValidateLifetime);
        CollectionAssert.Contains(
            validation.ValidAlgorithms.ToList(),
            SecurityAlgorithms.HmacSha256);
        var signingKey =
            (SymmetricSecurityKey)validation.IssuerSigningKey;
        CollectionAssert.AreEqual(
            Encoding.UTF8.GetBytes(SigningKey),
            signingKey.Key);
    }

    private static IConfiguration CreateConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:PostgreSQLConnection"] =
                "Host=localhost;Database=test;Username=test;Password=test",
            ["CustomerJwt:Issuer"] = Issuer,
            ["CustomerJwt:Audience"] = Audience,
            ["CustomerJwt:SigningKey"] = SigningKey
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }
}
