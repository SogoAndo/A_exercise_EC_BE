using System.Reflection;
using A_exercise_EC_BE.Presentations.Authentication;
using A_exercise_EC_BE.Presentations.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Moq;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace A_exercise_EC_BE.Presentations.Tests.Configs;

[TestClass]
[TestCategory("Presentations/Configs")]
public class CustomerJwtAuthorizeOperationFilterTests
{
    private readonly CustomerJwtAuthorizeOperationFilter filter = new();

    [TestMethod]
    public void Apply_WhenEndpointAllowsAnonymous_DoesNotAddSecurity()
    {
        var operation = new OpenApiOperation();

        filter.Apply(
            operation,
            CreateContext(new AllowAnonymousAttribute()));

        Assert.IsTrue(
            operation.Security is null
            || operation.Security.Count == 0);
    }

    [TestMethod]
    public void Apply_WhenEndpointDoesNotRequireAuthorization_DoesNotAddSecurity()
    {
        var operation = new OpenApiOperation();

        filter.Apply(operation, CreateContext());

        Assert.IsTrue(
            operation.Security is null
            || operation.Security.Count == 0);
    }

    [TestMethod]
    public void Apply_WhenEndpointRequiresAuthorization_AddsCustomerJwtSecurity()
    {
        var operation = new OpenApiOperation();

        filter.Apply(
            operation,
            CreateContext(new AuthorizeAttribute()));

        Assert.IsNotNull(operation.Security);
        Assert.HasCount(1, operation.Security);
        var requirement = operation.Security.Single();
        var scheme = requirement.Keys.Single();
        Assert.AreEqual(
            CustomerJwtAuthenticationDefaults.AuthenticationScheme,
            scheme.Reference.Id);
    }

    [TestMethod]
    public void Apply_WhenSecurityAlreadyExists_AppendsCustomerJwtSecurity()
    {
        var operation = new OpenApiOperation
        {
            Security = []
        };
        operation.Security.Add(new OpenApiSecurityRequirement());

        filter.Apply(
            operation,
            CreateContext(new AuthorizeAttribute()));

        Assert.HasCount(2, operation.Security);
    }

    private static OperationFilterContext CreateContext(
        params object[] metadata)
    {
        var actionDescriptor = new ActionDescriptor
        {
            EndpointMetadata = metadata.ToList()
        };
        var apiDescription = new ApiDescription
        {
            ActionDescriptor = actionDescriptor
        };

        return new OperationFilterContext(
            apiDescription,
            Mock.Of<ISchemaGenerator>(),
            new SchemaRepository(),
            new OpenApiDocument(),
            typeof(CustomerJwtAuthorizeOperationFilterTests)
                .GetMethod(
                    nameof(DummyAction),
                    BindingFlags.NonPublic
                    | BindingFlags.Static)!);
    }

    private static void DummyAction()
    {
    }
}
