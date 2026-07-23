namespace A_exercise_EC_BE.Application.Usecases.Customers;

/// <summary>
/// UC002 顧客ログインUseCaseのインターフェース。
/// </summary>
public interface ILoginCustomerUsecase
{
    Task<CustomerLoginResult> LoginAsync(CustomerLoginRequest request);
}
