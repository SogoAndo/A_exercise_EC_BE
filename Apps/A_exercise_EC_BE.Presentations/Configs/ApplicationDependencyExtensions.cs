using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using A_exercise_EC_BE.Infrastructures.Contexts;
using A_exercise_EC_BE.Infrastructures.Adapters;
using A_exercise_EC_BE.Infrastructures.Repositories;
using A_exercise_EC_BE.Infrastructures.Security;
using A_exercise_EC_BE.Domains.Repositories;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Infrastructures.Shared;
using A_exercise_EC_BE.Applications.Security;
using A_exercise_EC_BE.Applications.Usecases;
using A_exercise_EC_BE.Applications.Usecases.Customers;
using A_exercise_EC_BE.Applications.Usecases.Accounts;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.Authentication;

namespace A_exercise_EC_BE.Presentations.Configs;
/// <summary>
/// 依存関係(DI)の設定
/// インフラストラクチャ層、アプリケーション層、プレゼンテーション層
/// をまとめて追加する拡張クラス
/// </summary>
public static class ApplicationDependencyExtensions
{
    /// <summary>
    /// アプリ全体の依存関係を一括追加する拡張メソッド
    /// </summary>
    /// <param name="services">サービスコレクション</param>
    /// <param name="config">構成情報</param>
    /// <returns>IServiceCollection(チェーン可能)</returns>
    public static IServiceCollection AddApplicationDependencies(
        this IServiceCollection services, IConfiguration config)
    {
        // インフラストラクチャ層の依存関係を追加
        services.AddInfrastructureDependencies(config);
        // アプリケーション層の依存関係を追加
        services.AddApplicationLayerDependencies(config);
        // プレゼンテーション層の依存関係を追加
        services.AddPresentationLayerDependencies(config);
        return services;
    }

    /// <summary>
    /// インフラストラクチャ層の依存関係を追加
    /// </summary>
    private static IServiceCollection AddInfrastructureDependencies(
        this IServiceCollection services, IConfiguration config)
    {
        // DbContext の登録
        var connectstr = config.GetConnectionString("PostgreSQLConnection");
        services.AddDbContext<AppDbContext>(options =>
        {
            options.LogTo(Console.WriteLine, LogLevel.Debug);
            options.UseNpgsql(connectstr);
        });

        services.AddScoped<CustomerEntityAdapter>();
        services.AddScoped<OrderStatusEntityAdapter>();
        services.AddScoped<PaymentMethodEntityAdapter>();

        services.AddScoped<OrdersEntityAdapter>();
        services.AddScoped<OrdersDetailEntityAdapter>();

        services.AddScoped<ProductCategoryEntityAdapter>();
        services.AddScoped<ProductStockEntityAdapter>();
        services.AddScoped<ProductEntityAdapter>();

        // Product・ProductCategory・ProductStock復元用Factory
        services.AddScoped<ProductFactory>();
        services.AddScoped<OrdersFactory>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<
            IPasswordHasher<CustomerPasswordContext>,
            PasswordHasher<CustomerPasswordContext>>();
        services.AddScoped<
            ICustomerPasswordVerifier,
            PBKDF2CustomerPasswordVerifier>();
        services.AddScoped<
            ICustomerAccessTokenIssuer,
            CustomerJwtTokenIssuer>();
        services.AddSingleton<
            IValidateOptions<CustomerJwtOptions>,
            CustomerJwtOptionsValidator>();
        services
            .AddOptions<CustomerJwtOptions>()
            .Bind(
                config.GetSection(
                    CustomerJwtOptions.SectionName))
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// アプリケーション層の依存関係を追加
    /// </summary>
    private static IServiceCollection AddApplicationLayerDependencies(
    this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
        services.AddScoped<IRegisterCustomerAccountUsecase, RegisterCustomerAccountUsecase>();
        services.AddScoped<ILoginCustomerUsecase, LoginCustomerUsecase>();
        services.AddScoped<ILogoutCustomerUsecase, LogoutCustomerUsecase>();

        return services;
    }

    /// <summary>
    /// プレゼンテーション層の依存関係を追加
    /// </summary>
    private static IServiceCollection AddPresentationLayerDependencies(
    this IServiceCollection services, IConfiguration config)
    {
        // コントローラをサービスコレクションに登録する
        services.AddControllers();

        services
            .AddAuthentication(
                CustomerJwtAuthenticationDefaults.AuthenticationScheme)
            .AddJwtBearer(
                CustomerJwtAuthenticationDefaults.AuthenticationScheme,
                _ => { });
        services
            .AddOptions<JwtBearerOptions>(
                CustomerJwtAuthenticationDefaults.AuthenticationScheme)
            .Configure<IOptions<CustomerJwtOptions>>(
                (options, customerJwtOptions) =>
                {
                    var customerJwt = customerJwtOptions.Value;
                    customerJwt.Validate();

                    options.MapInboundClaims = false;
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidIssuer = customerJwt.Issuer,
                            ValidateAudience = true,
                            ValidAudience = customerJwt.Audience,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    customerJwt.SigningKey)),
                            ValidateLifetime = true,
                            RequireExpirationTime = true,
                            ClockSkew = TimeSpan.Zero,
                            ValidAlgorithms =
                            [
                                SecurityAlgorithms.HmacSha256
                            ]
                        };
                });

        services.AddAuthorization();

        // RegisterBookViewModelからドメインオブジェクト:Bookへ変換するアダプタ
        services.AddScoped<RegisterCustomerAccountViewModelAdapter>();
        return services;
    }

    /// <summary>
    /// テストプロジェクトにServiceProviderを提供するヘルパメソッド
    /// </summary>
    /// <param name="config"></param>
    /// <param name="configureServices"></param>
    /// <param name="configureLogging"></param>
    /// <returns></returns>
    public static ServiceProvider BuildAppProvider(
       IConfiguration config,
       Action<IServiceCollection>? configureServices = null,
       Action<ILoggingBuilder>? configureLogging = null)
    {
        //ServiceProvider：生成されたインスタンスを検索して返す機能（たくさんのインスタンスの中で使いたいやつだけを返してくれる）
        var services = new ServiceCollection();
        services.AddLogging(b =>
        {
            if (configureLogging is not null) configureLogging(b);
            else b.AddConsole().SetMinimumLevel(LogLevel.Warning);
        });
        services.AddApplicationDependencies(config);
        configureServices?.Invoke(services);

        return services.BuildServiceProvider(validateScopes: true);
    }
}
