namespace A_exercise_EC_BE.Applications.Usecases.Customers;

/// <summary>
/// UC008 顧客ログアウトUseCase。
/// </summary>
public sealed class LogoutCustomerUsecase : ILogoutCustomerUsecase
{
    /// <inheritdoc />
    public Task<CustomerLogoutResult> LogoutAsync()
        => Task.FromResult(
            CustomerLogoutResult.CreateLoggedOut());
}
