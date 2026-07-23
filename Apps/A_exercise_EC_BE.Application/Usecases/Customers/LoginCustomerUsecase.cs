using System.Net.Mail;
using A_exercise_EC_BE.Application.Security;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Repositories;

namespace A_exercise_EC_BE.Application.Usecases.Customers;

/// <summary>
/// UC002 顧客ログインUseCase。
/// </summary>
public class LoginCustomerUsecase(
    ICustomerRepository customerRepository,
    ICustomerPasswordVerifier passwordVerifier) : ILoginCustomerUsecase
{
    private const int MinPasswordLength = 5;
    private const int MaxPasswordLength = 20;
    private const string AuthenticationFailedMessage =
        "メールアドレスまたはパスワードが正しくありません。";

    public async Task<CustomerLoginResult> LoginAsync(CustomerLoginRequest request)
    {
        ValidateRequest(request);

        var customer = await customerRepository.FindByMailAddressAsync(
            request.MailAddress);
        var isPasswordValid = passwordVerifier.Verify(
            customer?.Password,
            request.Password);

        if (customer is null || !isPasswordValid)
        {
            throw new UnauthorizedAccessException(AuthenticationFailedMessage);
        }

        return new CustomerLoginResult(
            customer.CustomerUuid,
            customer.Username,
            customer.Name);
    }

    private static void ValidateRequest(CustomerLoginRequest request)
    {
        if (request is null)
        {
            throw new DomainException("ログイン情報を入力してください。");
        }

        ValidateMailAddress(request.MailAddress);
        ValidatePassword(request.Password);
    }

    private static void ValidateMailAddress(string mailAddress)
    {
        if (string.IsNullOrWhiteSpace(mailAddress))
        {
            throw new DomainException("メールアドレスを入力してください。");
        }

        if (!MailAddress.TryCreate(mailAddress, out var parsedAddress)
            || !string.Equals(
                parsedAddress.Address,
                mailAddress,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainException(
                "正しいメールアドレス形式で入力してください。");
        }
    }

    private static void ValidatePassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new DomainException("パスワードを入力してください。");
        }

        if (password.Length < MinPasswordLength
            || password.Length > MaxPasswordLength)
        {
            throw new DomainException(
                "パスワードは5～20文字で入力してください。");
        }
    }
}
