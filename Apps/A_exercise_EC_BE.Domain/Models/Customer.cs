using A_exercise_EC_BE.Domain.Exceptions;

namespace A_exercise_EC_BE.Domain.Models;

/// <summary>
/// 管理側と共有する顧客アカウントを表すドメインオブジェクト。
/// </summary>
public class Customer
{
    private const int MaxNameLength = 20;
    private const int MaxKanaLength = 20;
    private const int MaxAddressLength = 100;
    private const int MaxPhoneNumberLength = 20;
    private const int MaxMailAddressLength = 200;
    private const int MaxUsernameLength = 30;
    private const int MaxPasswordHashLength = 255;

    public Guid CustomerUuid { get; }
    public string Name { get; }
    public string? Kana { get; }
    public string Address1 { get; }
    public string? Address2 { get; }
    public string PhoneNumber { get; }
    public string MailAddress { get; }
    public string Username { get; }
    public string PasswordHash { get; }
    public DateTime CreatedAt { get; }

    public Customer(
        Guid customerUuid,
        string name,
        string? kana,
        string address1,
        string? address2,
        string phoneNumber,
        string mailAddress,
        string username,
        string passwordHash,
        DateTime createdAt)
    {
        ValidateUuid(customerUuid);
        ValidateRequiredText(name, MaxNameLength, "顧客名");
        ValidateOptionalText(kana, MaxKanaLength, "顧客名カナ");
        ValidateRequiredText(address1, MaxAddressLength, "住所1");
        ValidateOptionalText(address2, MaxAddressLength, "住所2");
        ValidateRequiredText(phoneNumber, MaxPhoneNumberLength, "電話番号");
        ValidateRequiredText(mailAddress, MaxMailAddressLength, "メールアドレス");
        ValidateRequiredText(username, MaxUsernameLength, "ユーザー名");
        ValidateRequiredText(passwordHash, MaxPasswordHashLength, "パスワードハッシュ");

        if (createdAt == default)
        {
            throw new DomainException("登録日時が不正です");
        }

        CustomerUuid = customerUuid;
        Name = name;
        Kana = kana;
        Address1 = address1;
        Address2 = address2;
        PhoneNumber = phoneNumber;
        MailAddress = mailAddress;
        Username = username;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    private static void ValidateUuid(Guid customerUuid)
    {
        if (customerUuid == Guid.Empty)
        {
            throw new DomainException("顧客識別IDが不正です");
        }
    }

    private static void ValidateRequiredText(string value, int maxLength, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName}は必須です");
        }

        if (value.Length > maxLength)
        {
            throw new DomainException($"{fieldName}は{maxLength}文字以内で入力してください");
        }
    }

    private static void ValidateOptionalText(string? value, int maxLength, string fieldName)
    {
        if (value is not null && value.Length > maxLength)
        {
            throw new DomainException($"{fieldName}は{maxLength}文字以内で入力してください");
        }
    }
}
