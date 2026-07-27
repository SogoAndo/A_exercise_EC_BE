using A_exercise_EC_BE.Applications.Usecases.Products;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.Configs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace A_exercise_EC_BE.Presentations.Tests.Configs;

/// <summary>
/// UC004 商品詳細取得のDI登録テスト。
/// </summary>
[TestClass]
[TestCategory("Presentations/Configs")]
public class ProductDetailDependencyRegistrationTests
{
    [TestMethod(
        DisplayName =
            "商品詳細取得に必要な依存関係を登録する")]
    public void
        BuildAppProvider_RegistersProductDetailDependencies()
    {
        using var provider =
            ApplicationDependencyExtensions
                .BuildAppProvider(
                    CreateConfiguration());
        using var scope =
            provider.CreateScope();

        var usecase =
            scope.ServiceProvider
                .GetRequiredService<
                    IGetProductDetailUsecase>();
        var adapter =
            scope.ServiceProvider
                .GetRequiredService<
                    ProductDetailViewModelAdapter>();

        Assert.IsInstanceOfType<
            GetProductDetailUsecase>(
                usecase);
        Assert.IsInstanceOfType<
            ProductDetailViewModelAdapter>(
                adapter);
    }

    private static IConfiguration
        CreateConfiguration()
    {
        var values =
            new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgreSQLConnection"] =
                    "Host=localhost;Database=test;"
                    + "Username=test;Password=test",
                ["CustomerJwt:Issuer"] =
                    "A_exercise_EC_BE.Tests",
                ["CustomerJwt:Audience"] =
                    "A_exercise_EC_FE.Tests",
                ["CustomerJwt:SigningKey"] =
                    "test-signing-key-"
                    + "must-be-long-enough-1234567890",
                ["CustomerJwt:ExpirationMinutes"] =
                    "30"
            };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                values)
            .Build();
    }
}
