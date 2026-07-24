using A_exercise_EC_BE.Domains.Exceptions;

namespace A_exercise_EC_BE.Domains.Models;

/// <summary>
/// 顧客を表すドメインオブジェクト
/// </summary>
public class Customer
{
    /// <summary>
    /// 顧客識別ID(UUID)
    /// </summary>
    public Guid CustomerUuid { get; private set; }

    /// <summary>
    /// 顧客名
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// 顧客名カナ
    /// </summary>
    public string Kana { get; private set; } = string.Empty;

    /// <summary>
    /// 住所1
    /// </summary>
    public string Address1 { get; private set; } = string.Empty;

    /// <summary>
    /// 住所2
    /// </summary>
    public string? Address2 { get; private set; }

    /// <summary>
    /// 電話番号
    /// </summary>
    public string PhoneNumber { get; private set; } = string.Empty;

    /// <summary>
    /// メールアドレス
    /// </summary>
    public string MailAddress { get; private set; } = string.Empty;

    /// <summary>
    /// アカウント名
    /// </summary>
    public string Username { get; private set; } = string.Empty;

    /// <summary>
    /// パスワード
    /// </summary>
    public string Password { get; private set; } = string.Empty;

    /// <summary>
    /// 登録日時
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    private const int MaxLengthName = 20;
    private const int MaxLengthKana = 20;
    private const int MaxLengthAddress = 100;
    private const int MaxLengthPhone = 20;
    private const int MaxLengthMail = 200;
    private const int MaxLengthUserName = 30;
    private const int MaxLengthPass = 255;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Customer(
        Guid customerUuid,
        string customerName,
        string customerKana,
        string address1,
        string? address2,
        string phoneNumber,
        string mailAddress,
        string userName,
        string passWord,
        DateTime createdAt)
    {
        ValidateCustomerUuid(customerUuid);
        ValidateName(customerName);
        ValidateKana(customerKana);
        ValidateAddress1(address1);
        ValidateAddress2(address2);
        ValidatePhoneNumber(phoneNumber);
        ValidateMail(mailAddress);
        ValidateUserName(userName);
        ValidatePass(passWord);
        ValidateCreatedAt(createdAt);

        CustomerUuid = customerUuid;
        Name = customerName;
        Kana = customerKana;
        Address1 = address1;
        Address2 = address2;
        PhoneNumber = phoneNumber;
        MailAddress = mailAddress;
        Username = userName;
        Password = passWord;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// ID未定の顧客を作成する場合のコンストラクタ
    /// </summary>
    public Customer(
        string customerName,
        string customerKana,
        string address1,
        string? address2,
        string phoneNumber,
        string mailAddress,
        string userName,
        string passWord,
        DateTime createdAt)
        : this(
            Guid.NewGuid(),
            customerName,
            customerKana,
            address1,
            address2,
            phoneNumber,
            mailAddress,
            userName,
            passWord,
            createdAt)
    {
    }

    /// <summary>
    /// 顧客識別IDの検証
    /// </summary>
    private void ValidateCustomerUuid(Guid customerUuid)
    {
        if (customerUuid == Guid.Empty)
            throw new DomainException("顧客識別IDが不正です");
    }

    /// <summary>
    /// 顧客名の検証
    /// </summary>
    private void ValidateName(string customerName)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("顧客名は必須です");

        if (customerName.Length > MaxLengthName)
            throw new DomainException($"顧客名は{MaxLengthName}文字以内で入力してください");
    }

    /// <summary>
    /// 顧客カナ氏名の検証
    /// </summary>
    private void ValidateKana(string customerKana)
    {
        if (string.IsNullOrWhiteSpace(customerKana))
            throw new DomainException("顧客カナ氏名は必須です");

        if (customerKana.Length > MaxLengthKana)
            throw new DomainException($"顧客カナ氏名は{MaxLengthKana}文字以内で入力してください");
    }

    /// <summary>
    /// 住所1の検証
    /// </summary>
    private void ValidateAddress1(string address1)
    {
        if (string.IsNullOrWhiteSpace(address1))
            throw new DomainException("住所1は必須です");

        if (address1.Length > MaxLengthAddress)
            throw new DomainException($"住所1は{MaxLengthAddress}文字以内で入力してください");
    }

    /// <summary>
    /// 住所2の検証
    /// </summary>
    private void ValidateAddress2(string? address2)
    {
        if (address2 is not null && address2.Length > MaxLengthAddress)
            throw new DomainException($"住所2は{MaxLengthAddress}文字以内で入力してください");
    }

    /// <summary>
    /// 電話番号の検証
    /// </summary>
    private void ValidatePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new DomainException("電話番号は必須です");

        if (phoneNumber.Length > MaxLengthPhone)
            throw new DomainException($"電話番号は{MaxLengthPhone}文字以内で入力してください");
    }

    /// <summary>
    /// メールアドレスの検証
    /// </summary>
    private void ValidateMail(string mailAddress)
    {
        if (string.IsNullOrWhiteSpace(mailAddress))
            throw new DomainException("メールアドレスは必須です");

        if (mailAddress.Length > MaxLengthMail)
            throw new DomainException($"メールアドレスは{MaxLengthMail}文字以内で入力してください");
    }

    /// <summary>
    /// ユーザー名の検証
    /// </summary>
    private void ValidateUserName(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new DomainException("ユーザー名は必須です");

        if (userName.Length > MaxLengthUserName)
            throw new DomainException($"ユーザー名は{MaxLengthUserName}文字以内で入力してください");
    }

    /// <summary>
    /// パスワードの検証
    /// </summary>
    private void ValidatePass(string passWord)
    {
        if (string.IsNullOrWhiteSpace(passWord))
            throw new DomainException("パスワードは必須です");

        if (passWord.Length > MaxLengthPass)
            throw new DomainException($"パスワードは{MaxLengthPass}文字以内で入力してください");
    }

    /// <summary>
    /// 登録日時の検証
    /// </summary>
    private void ValidateCreatedAt(DateTime createdAt)
    {
        if (createdAt == default)
            throw new DomainException("登録日時が不正です");

        if (createdAt > DateTime.Now)
            throw new DomainException("登録日時に未来日は指定できません");
    }

    /// <summary>
    /// 等価性（IDによる比較）
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not Customer other) return false;
        return CustomerUuid == other.CustomerUuid;
    }

    public override int GetHashCode()
        => CustomerUuid.GetHashCode();

    public override string ToString()
        => $"{CustomerUuid}: {Name},{Kana},{Address1}{Address2},{PhoneNumber},{MailAddress},{CreatedAt}";
}