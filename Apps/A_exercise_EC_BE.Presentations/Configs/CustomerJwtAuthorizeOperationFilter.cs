using A_exercise_EC_BE.Presentations.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace A_exercise_EC_BE.Presentations.Configs;

/// <summary>
/// 顧客JWT認証が必要なAPIをSwaggerへ反映する。
/// </summary>
public sealed class CustomerJwtAuthorizeOperationFilter
    : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        var endpointMetadata =
            context.ApiDescription.ActionDescriptor.EndpointMetadata;

        var allowsAnonymous =
            endpointMetadata.OfType<IAllowAnonymous>().Any();
        var requiresAuthorization =
            endpointMetadata.OfType<IAuthorizeData>().Any();

        if (allowsAnonymous || !requiresAuthorization)
        {
            return;
        }

        operation.Security ??= [];
        operation.Security.Add(
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        CustomerJwtAuthenticationDefaults
                            .AuthenticationScheme,
                        context.Document)
                ] = []
            });
    }
}
