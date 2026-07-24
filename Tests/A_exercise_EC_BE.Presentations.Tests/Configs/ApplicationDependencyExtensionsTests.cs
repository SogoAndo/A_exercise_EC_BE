using System.Text;
using A_exercise_EC_BE.Applications.Security;
using A_exercise_EC_BE.Applications.Usecases;
using A_exercise_EC_BE.Applications.Usecases.Accounts;
using A_exercise_EC_BE.Applications.Usecases.Customers;
using A_exercise_EC_BE.Applications.Usecases.Products;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Repositories;
using A_exercise_EC_BE.Infrastructures.Security;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.Authentication;
using A_exercise_EC_BE.Presentations.Configs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_exercise_EC_BE.Presentations.Tests.Configs;

/// <summary>
/// ApplicationDependencyExtensionsの単体テスト
/// </summary>
[TestClass]
[TestCategory("Presentation/Configs")]
public class ApplicationDependencyExtensionsTests
{
    private const string Issuer =
        "https://ec.example.test";

    private const string Audience =
        "https://ec.example.test";

    private const string SigningKey =
        "customer-only-signing-key-0123456789-abcdef";

    /// <summary>
    /// AddApplicationDependenciesが
    /// 同じIServiceCollectionを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "AddApplicationDependencies_同じServiceCollectionを返す")]
    public void
        AddApplicationDependencies_ReturnsSameServiceCollection()
    {
        // Arrange
        var services =
            new ServiceCollection();

        var configuration =
            CreateConfiguration();

        // Act
        var actual =
            services.AddApplicationDependencies(
                configuration);

        // Assert
        Assert.AreSame(
            services,
            actual);
    }

    /// <summary>
    /// インフラストラクチャ層の
    /// 依存関係が登録されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_インフラストラクチャ層の依存関係を登録する")]
    public void
        BuildAppProvider_RegistersInfrastructureDependencies()
    {
        // Arrange
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration());

        using var scope =
            provider.CreateScope();

        var services =
            scope.ServiceProvider;

        // Act
        var dbContext =
            services.GetRequiredService<
                AppDbContext>();

        var customerAdapter =
            services.GetRequiredService<
                CustomerEntityAdapter>();

        var customerRepository =
            services.GetRequiredService<
                ICustomerRepository>();

        var productRepository =
            services.GetRequiredService<
                IProductRepository>();

        var unitOfWork =
            services.GetRequiredService<
                IUnitOfWork>();

        // Assert
        Assert.IsInstanceOfType<
            AppDbContext>(
                dbContext);

        Assert.IsInstanceOfType<
            CustomerEntityAdapter>(
                customerAdapter);

        Assert.IsInstanceOfType<
            CustomerRepository>(
                customerRepository);

        Assert.IsInstanceOfType<
            ProductRepository>(
                productRepository);

        Assert.IsNotNull(
            unitOfWork);
    }

    /// <summary>
    /// DbContextへPostgreSQLの
    /// 接続設定が登録されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_DbContextへPostgreSQL接続設定を登録する")]
    public void
        BuildAppProvider_ConfiguresPostgreSqlDbContext()
    {
        // Arrange
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration());

        using var scope =
            provider.CreateScope();

        // Act
        var context =
            scope.ServiceProvider
                .GetRequiredService<
                    AppDbContext>();

        var providerName =
            context.Database.ProviderName;

        var connectionString =
            context.Database
                .GetConnectionString();

        // Assert
        Assert.AreEqual(
            "Npgsql.EntityFrameworkCore.PostgreSQL",
            providerName);

        Assert.AreEqual(
            "Host=localhost;"
            + "Database=test;"
            + "Username=test;"
            + "Password=test",
            connectionString);
    }

    /// <summary>
    /// 顧客ログインに必要な
    /// 依存関係が登録されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_顧客ログインの依存関係を登録する")]
    public void
        BuildAppProvider_RegistersCustomerLoginDependencies()
    {
        // Arrange
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration());

        using var scope =
            provider.CreateScope();

        var services =
            scope.ServiceProvider;

        // Act
        var repository =
            services.GetRequiredService<
                ICustomerRepository>();

        var passwordVerifier =
            services.GetRequiredService<
                ICustomerPasswordVerifier>();

        var tokenIssuer =
            services.GetRequiredService<
                ICustomerAccessTokenIssuer>();

        var usecase =
            services.GetRequiredService<
                ILoginCustomerUsecase>();

        // Assert
        Assert.IsInstanceOfType<
            CustomerRepository>(
                repository);

        Assert.IsInstanceOfType<
            PBKDF2CustomerPasswordVerifier>(
                passwordVerifier);

        Assert.IsInstanceOfType<
            CustomerJwtTokenIssuer>(
                tokenIssuer);

        Assert.IsInstanceOfType<
            LoginCustomerUsecase>(
                usecase);
    }

    /// <summary>
    /// 顧客ログアウトに必要な
    /// 依存関係が登録されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_顧客ログアウトの依存関係を登録する")]
    public void
        BuildAppProvider_RegistersCustomerLogoutDependencies()
    {
        // Arrange
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration());

        using var scope =
            provider.CreateScope();

        // Act
        var actual =
            scope.ServiceProvider
                .GetRequiredService<
                    ILogoutCustomerUsecase>();

        // Assert
        Assert.IsInstanceOfType<
            LogoutCustomerUsecase>(
                actual);
    }

    /// <summary>
    /// 顧客アカウント登録に必要な
    /// 依存関係が登録されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_顧客アカウント登録の依存関係を登録する")]
    public void
        BuildAppProvider_RegistersCustomerRegistrationDependencies()
    {
        // Arrange
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration());

        using var scope =
            provider.CreateScope();

        var services =
            scope.ServiceProvider;

        // Act
        var hashingService =
            services.GetRequiredService<
                IPasswordHashingService>();

        var usecase =
            services.GetRequiredService<
                IRegisterCustomerAccountUsecase>();

        var adapter =
            services.GetRequiredService<
                RegisterCustomerAccountViewModelAdapter>();

        // Assert
        Assert.IsInstanceOfType<
            PasswordHashingService>(
                hashingService);

        Assert.IsInstanceOfType<
            RegisterCustomerAccountUsecase>(
                usecase);

        Assert.IsInstanceOfType<
            RegisterCustomerAccountViewModelAdapter>(
                adapter);
    }

    /// <summary>
    /// 商品カテゴリ検索に必要な
    /// 依存関係が登録されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_商品カテゴリ検索の依存関係を登録する")]
    public void
        BuildAppProvider_RegistersProductSearchDependencies()
    {
        // Arrange
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration());

        using var scope =
            provider.CreateScope();

        // Act
        var actual =
            scope.ServiceProvider
                .GetRequiredService<
                    ISearchProductByCategoryUsecase>();

        // Assert
        Assert.IsInstanceOfType<
            SearchProductByCategoryUsecase>(
                actual);
    }

    /// <summary>
    /// 顧客JWT認証が正しく設定されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_顧客JWT認証を設定する")]
    public async Task
        BuildAppProvider_ConfiguresCustomerJwtAuthentication()
    {
        // Arrange
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration());

        var schemeProvider =
            provider.GetRequiredService<
                IAuthenticationSchemeProvider>();

        // Act
        var scheme =
            await schemeProvider
                .GetSchemeAsync(
                    CustomerJwtAuthenticationDefaults
                        .AuthenticationScheme);

        var defaultAuthenticateScheme =
            await schemeProvider
                .GetDefaultAuthenticateSchemeAsync();

        var defaultChallengeScheme =
            await schemeProvider
                .GetDefaultChallengeSchemeAsync();

        var options =
            provider
                .GetRequiredService<
                    IOptionsMonitor<
                        JwtBearerOptions>>()
                .Get(
                    CustomerJwtAuthenticationDefaults
                        .AuthenticationScheme);

        var validation =
            options.TokenValidationParameters;

        // Assert
        Assert.IsNotNull(
            scheme);

        Assert.AreEqual(
            CustomerJwtAuthenticationDefaults
                .AuthenticationScheme,
            scheme.Name);

        Assert.AreEqual(
            CustomerJwtAuthenticationDefaults
                .AuthenticationScheme,
            defaultAuthenticateScheme?.Name);

        Assert.AreEqual(
            CustomerJwtAuthenticationDefaults
                .AuthenticationScheme,
            defaultChallengeScheme?.Name);

        Assert.IsFalse(
            options.MapInboundClaims);

        Assert.IsTrue(
            validation.ValidateIssuer);

        Assert.AreEqual(
            Issuer,
            validation.ValidIssuer);

        Assert.IsTrue(
            validation.ValidateAudience);

        Assert.AreEqual(
            Audience,
            validation.ValidAudience);

        Assert.IsTrue(
            validation.ValidateIssuerSigningKey);

        Assert.IsTrue(
            validation.ValidateLifetime);

        Assert.IsTrue(
            validation.RequireExpirationTime);

        Assert.AreEqual(
            TimeSpan.Zero,
            validation.ClockSkew);

        CollectionAssert.Contains(
            validation.ValidAlgorithms
                .ToList(),
            SecurityAlgorithms.HmacSha256);

        var signingKey =
            validation.IssuerSigningKey
                as SymmetricSecurityKey;

        Assert.IsNotNull(
            signingKey);

        CollectionAssert.AreEqual(
            Encoding.UTF8.GetBytes(
                SigningKey),
            signingKey.Key);
    }

    /// <summary>
    /// configureLoggingがnullの場合、
    /// 既定のロギング設定が登録されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_configureLoggingがnullの場合は既定ロギングを登録する")]
    public void
        BuildAppProvider_WhenConfigureLoggingIsNull_RegistersDefaultLogging()
    {
        // Arrange・Act
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration(),
                    configureServices: null,
                    configureLogging: null);

        // Assert
        var loggerFactory =
            provider.GetService<
                ILoggerFactory>();

        Assert.IsNotNull(
            loggerFactory);

        var logger =
            loggerFactory.CreateLogger(
                "DefaultLoggerTest");

        Assert.IsNotNull(
            logger);
    }

    /// <summary>
    /// configureLoggingが指定された場合、
    /// 指定された処理が実行されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_configureLogging指定時はコールバックを実行する")]
    public void
        BuildAppProvider_WhenConfigureLoggingIsProvided_InvokesCallback()
    {
        // Arrange
        var callbackWasInvoked =
            false;

        // Act
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration(),
                    configureLogging:
                        logging =>
                        {
                            callbackWasInvoked =
                                true;

                            logging.SetMinimumLevel(
                                LogLevel.Trace);
                        });

        // Assert
        Assert.IsTrue(
            callbackWasInvoked);

        Assert.IsNotNull(
            provider.GetService<
                ILoggerFactory>());
    }

    /// <summary>
    /// configureServicesがnullの場合でも、
    /// ServiceProviderを生成できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_configureServicesがnullでもServiceProviderを生成できる")]
    public void
        BuildAppProvider_WhenConfigureServicesIsNull_ReturnsProvider()
    {
        // Act
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration(),
                    configureServices: null);

        // Assert
        Assert.IsNotNull(
            provider);

        Assert.IsNotNull(
            provider.GetService<
                ILoggerFactory>());
    }

    /// <summary>
    /// configureServicesが指定された場合、
    /// 指定されたサービスが追加されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_configureServices指定時は追加サービスを登録する")]
    public void
        BuildAppProvider_WhenConfigureServicesIsProvided_RegistersAdditionalService()
    {
        // Arrange
        var callbackWasInvoked =
            false;

        var expected =
            new TestDependency(
                "追加サービス");

        // Act
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration(),
                    configureServices:
                        services =>
                        {
                            callbackWasInvoked =
                                true;

                            services.AddSingleton(
                                expected);
                        });

        var actual =
            provider.GetRequiredService<
                TestDependency>();

        // Assert
        Assert.IsTrue(
            callbackWasInvoked);

        Assert.AreSame(
            expected,
            actual);

        Assert.AreEqual(
            "追加サービス",
            actual.Value);
    }

    /// <summary>
    /// configureServicesとconfigureLoggingの
    /// 両方を指定できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "BuildAppProvider_両方のコールバックを指定できる")]
    public void
        BuildAppProvider_WhenBothCallbacksAreProvided_InvokesBothCallbacks()
    {
        // Arrange
        var serviceCallbackCount =
            0;

        var loggingCallbackCount =
            0;

        // Act
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration(),
                    configureServices:
                        services =>
                        {
                            serviceCallbackCount++;

                            services.AddSingleton(
                                new TestDependency(
                                    "両方指定"));
                        },
                    configureLogging:
                        logging =>
                        {
                            loggingCallbackCount++;

                            logging.SetMinimumLevel(
                                LogLevel.Information);
                        });

        // Assert
        Assert.AreEqual(
            1,
            serviceCallbackCount);

        Assert.AreEqual(
            1,
            loggingCallbackCount);

        Assert.AreEqual(
            "両方指定",
            provider
                .GetRequiredService<
                    TestDependency>()
                .Value);
    }

    /// <summary>
    /// テスト用Configurationを生成する
    /// </summary>
    private static IConfiguration
        CreateConfiguration()
    {
        var settings =
            new Dictionary<string, string?>
            {
                [
                    "ConnectionStrings:"
                    + "PostgreSQLConnection"
                ] =
                    "Host=localhost;"
                    + "Database=test;"
                    + "Username=test;"
                    + "Password=test",

                ["CustomerJwt:Issuer"] =
                    Issuer,

                ["CustomerJwt:Audience"] =
                    Audience,

                ["CustomerJwt:SigningKey"] =
                    SigningKey
            };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                settings)
            .Build();
    }

    /// <summary>
    /// configureServices検証用の依存関係
    /// </summary>
    private sealed class TestDependency
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TestDependency(
            string value)
        {
            Value =
                value;
        }

        /// <summary>
        /// テスト値
        /// </summary>
        public string Value
        {
            get;
        }
    }
}