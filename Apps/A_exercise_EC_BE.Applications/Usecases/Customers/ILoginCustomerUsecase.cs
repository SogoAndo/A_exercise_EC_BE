namespace A_exercise_EC_BE.Applications.Usecases.Customers;

/// <summary>
/// UC002 顧客ログインUseCaseのインターフェース。
/// </summary>
public interface ILoginCustomerUsecase
{
    Task<CustomerLoginResult> LoginAsync(CustomerLoginRequest request);
}
