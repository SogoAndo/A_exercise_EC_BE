using A_exercise_EC_BE.Infrastructures.Security;
using Microsoft.Extensions.Options;

namespace A_exercise_EC_BE.Presentations.Configs;

/// <summary>
/// 顧客JWT設定を起動時に検証する。
/// </summary>
public sealed class CustomerJwtOptionsValidator
    : IValidateOptions<CustomerJwtOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(
        string? name,
        CustomerJwtOptions options)
    {
        try
        {
            options.Validate();
            return ValidateOptionsResult.Success;
        }
        catch (InvalidOperationException exception)
        {
            return ValidateOptionsResult.Fail(
                exception.Message);
        }
    }
}
