namespace A_exercise_EC_BE.Applications.Usecases.Customers;

/// <summary>
/// UC008 顧客ログアウトUseCaseのインターフェース。
/// </summary>
public interface ILogoutCustomerUsecase
{
    /// <summary>
    /// 顧客ログアウトを実行する。
    /// </summary>
    /// <returns>顧客ログアウト結果。</returns>
    Task<CustomerLogoutResult> LogoutAsync();
}
