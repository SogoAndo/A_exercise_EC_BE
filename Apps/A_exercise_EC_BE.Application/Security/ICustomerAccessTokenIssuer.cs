namespace A_exercise_EC_BE.Application.Security;

/// <summary>
/// 顧客認証用のアクセストークンを発行する。
/// </summary>
public interface ICustomerAccessTokenIssuer
{
    CustomerAccessToken Issue(Guid customerUuid);
}
