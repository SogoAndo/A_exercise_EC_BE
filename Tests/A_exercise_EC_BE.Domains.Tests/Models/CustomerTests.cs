using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;

namespace A_exercise_EC_BE.Domains.Tests.Models;

/// <summary>
/// Customerクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class CustomerTests
{
    /// <summary>
    /// ヘルパー：有効な顧客を作成する
    /// </summary>
    private Customer CreateCustomer(
        Guid? customerUuid = null,
        string customerName = "山田太郎",
        string customerKana = "ヤマダタロウ",
        string address1 = "東京都千代田区",
        string? address2 = "1-1-1",
        string phoneNumber = "09012345678",
        string mailAddress = "test@example.com",
        string userName = "yamada",
        string passWord = "password123",
        DateTime? createdAt = null)
    {
        return new Customer(
            customerUuid ?? Guid.NewGuid(),
            customerName,
            customerKana,
            address1,
            address2,
            phoneNumber,
            mailAddress,
            userName,
            passWord,
            createdAt ?? DateTime.Now.AddDays(-1));
    }

    [TestMethod(DisplayName = "コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var customerUuid = Guid.NewGuid();
        var customerName = "山田太郎";
        var customerKana = "ヤマダタロウ";
        var address1 = "東京都千代田区";
        var address2 = "1-1-1";
        var phoneNumber = "09012345678";
        var mailAddress = "test@example.com";
        var userName = "yamada";
        var passWord = "password123";
        var createdAt = DateTime.Now.AddDays(-1);

        // インスタンスを生成する
        var customer = new Customer(
            customerUuid,
            customerName,
            customerKana,
            address1,
            address2,
            phoneNumber,
            mailAddress,
            userName,
            passWord,
            createdAt);

        // 顧客識別IDを検証する
        Assert.AreEqual(customerUuid, customer.CustomerUuid);

        // 顧客名を検証する
        Assert.AreEqual(customerName, customer.Name);

        // 顧客カナ氏名を検証する
        Assert.AreEqual(customerKana, customer.Kana);

        // 住所1を検証する
        Assert.AreEqual(address1, customer.Address1);

        // 住所2を検証する
        Assert.AreEqual(address2, customer.Address2);

        // 電話番号を検証する
        Assert.AreEqual(phoneNumber, customer.PhoneNumber);

        // メールアドレスを検証する
        Assert.AreEqual(mailAddress, customer.MailAddress);

        // ユーザー名を検証する
        Assert.AreEqual(userName, customer.Username);

        // パスワードを検証する
        Assert.AreEqual(passWord, customer.Password);

        // 登録日時を検証する
        Assert.AreEqual(createdAt, customer.CreatedAt);
    }

    [TestMethod(DisplayName = "新規作成の場合UUIDが自動生成される")]
    public void NewInstance_ShouldGenerateUuidAutomatically()
    {
        // データを用意する
        var customerName = "山田太郎";
        var customerKana = "ヤマダタロウ";
        var address1 = "東京都千代田区";
        string? address2 = null;
        var phoneNumber = "09012345678";
        var mailAddress = "test@example.com";
        var userName = "yamada";
        var passWord = "password123";
        var createdAt = DateTime.Now.AddDays(-1);

        // インスタンスを生成する
        var customer = new Customer(
            customerName,
            customerKana,
            address1,
            address2,
            phoneNumber,
            mailAddress,
            userName,
            passWord,
            createdAt);

        // 顧客識別IDが自動生成されていることを検証する
        Assert.AreNotEqual(Guid.Empty, customer.CustomerUuid);

        // 顧客名を検証する
        Assert.AreEqual(customerName, customer.Name);

        // 顧客カナ氏名を検証する
        Assert.AreEqual(customerKana, customer.Kana);

        // 住所1を検証する
        Assert.AreEqual(address1, customer.Address1);

        // 住所2を検証する
        Assert.IsNull(customer.Address2);

        // 電話番号を検証する
        Assert.AreEqual(phoneNumber, customer.PhoneNumber);

        // メールアドレスを検証する
        Assert.AreEqual(mailAddress, customer.MailAddress);

        // ユーザー名を検証する
        Assert.AreEqual(userName, customer.Username);

        // パスワードを検証する
        Assert.AreEqual(passWord, customer.Password);

        // 登録日時を検証する
        Assert.AreEqual(createdAt, customer.CreatedAt);
    }

    [TestMethod(DisplayName = "顧客識別IDが空の場合、DomainExceptionがスローされる")]
    public void EmptyCustomerUuid_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(customerUuid: Guid.Empty);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("顧客識別IDが不正です", ex.Message);
    }

    [TestMethod(DisplayName = "顧客名がnullの場合、DomainExceptionがスローされる")]
    public void NullCustomerName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(customerName: null!);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("顧客名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "顧客名が空白の場合、DomainExceptionがスローされる")]
    public void EmptyCustomerName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(customerName: "");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("顧客名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "顧客名が空白文字のみの場合、DomainExceptionがスローされる")]
    public void WhiteSpaceCustomerName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(customerName: "   ");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("顧客名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "顧客名が20文字の場合、インスタンス生成される")]
    public void CustomerName_With20Chars_ShouldCreateInstance()
    {
        // データを用意する
        var customerName = new string('あ', 20);

        // インスタンスを生成する
        var customer = CreateCustomer(customerName: customerName);

        // 顧客名を検証する
        Assert.AreEqual(customerName, customer.Name);
    }

    [TestMethod(DisplayName = "顧客名が21文字以上の場合、DomainExceptionがスローされる")]
    public void CustomerName_LongerThan20Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var customerName = new string('あ', 21);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(customerName: customerName);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("顧客名は20文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "顧客カナ氏名が空白の場合、DomainExceptionがスローされる")]
    public void EmptyCustomerKana_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(customerKana: "");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("顧客カナ氏名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "顧客カナ氏名が21文字以上の場合、DomainExceptionがスローされる")]
    public void CustomerKana_LongerThan20Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var customerKana = new string('ア', 21);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(customerKana: customerKana);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("顧客カナ氏名は20文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "住所1が空白の場合、DomainExceptionがスローされる")]
    public void EmptyAddress1_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(address1: "");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("住所1は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "住所1が101文字以上の場合、DomainExceptionがスローされる")]
    public void Address1_LongerThan100Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var address1 = new string('あ', 101);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(address1: address1);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("住所1は100文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "住所2がnullの場合、インスタンス生成される")]
    public void NullAddress2_ShouldCreateInstance()
    {
        // インスタンスを生成する
        var customer = CreateCustomer(address2: null);

        // 住所2を検証する
        Assert.IsNull(customer.Address2);
    }

    [TestMethod(DisplayName = "住所2が101文字以上の場合、DomainExceptionがスローされる")]
    public void Address2_LongerThan100Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var address2 = new string('あ', 101);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(address2: address2);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("住所2は100文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "電話番号が空白の場合、DomainExceptionがスローされる")]
    public void EmptyPhoneNumber_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(phoneNumber: "");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("電話番号は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "電話番号が21文字以上の場合、DomainExceptionがスローされる")]
    public void PhoneNumber_LongerThan20Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var phoneNumber = new string('1', 21);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(phoneNumber: phoneNumber);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("電話番号は20文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "メールアドレスが空白の場合、DomainExceptionがスローされる")]
    public void EmptyMailAddress_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(mailAddress: "");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("メールアドレスは必須です", ex.Message);
    }

    [TestMethod(DisplayName = "メールアドレスが201文字以上の場合、DomainExceptionがスローされる")]
    public void MailAddress_LongerThan200Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var mailAddress = new string('a', 201);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(mailAddress: mailAddress);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("メールアドレスは200文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "ユーザー名が空白の場合、DomainExceptionがスローされる")]
    public void EmptyUserName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(userName: "");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("ユーザー名は必須です", ex.Message);
    }

    [TestMethod(DisplayName = "ユーザー名が31文字以上の場合、DomainExceptionがスローされる")]
    public void UserName_LongerThan30Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var userName = new string('a', 31);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(userName: userName);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("ユーザー名は30文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "パスワードが空白の場合、DomainExceptionがスローされる")]
    public void EmptyPassword_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(passWord: "");
        });

        // 例外メッセージを検証する
        Assert.AreEqual("パスワードは必須です", ex.Message);
    }

    [TestMethod(DisplayName = "パスワードが256文字以上の場合、DomainExceptionがスローされる")]
    public void Password_LongerThan255Chars_ShouldThrowDomainException()
    {
        // データを用意する
        var passWord = new string('a', 256);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(passWord: passWord);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("パスワードは255文字以内で入力してください", ex.Message);
    }

    [TestMethod(DisplayName = "登録日時が初期値の場合、DomainExceptionがスローされる")]
    public void DefaultCreatedAt_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(createdAt: default(DateTime));
        });

        Assert.AreEqual("登録日時が不正です", ex.Message);
    }

    [TestMethod(DisplayName = "登録日時が未来日の場合、DomainExceptionがスローされる")]
    public void FutureCreatedAt_ShouldThrowDomainException()
    {
        // データを用意する
        var createdAt = DateTime.Now.AddDays(1);

        var ex = Assert.ThrowsExactly<DomainException>(() =>
        {
            _ = CreateCustomer(createdAt: createdAt);
        });

        // 例外メッセージを検証する
        Assert.AreEqual("登録日時に未来日は指定できません", ex.Message);
    }

    [TestMethod(DisplayName = "UUIDで等価と判定される")]
    public void Equals_WithSameUuid_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid();
        var customer1 = CreateCustomer(customerUuid: uuid, customerName: "山田太郎");
        var customer2 = CreateCustomer(customerUuid: uuid, customerName: "佐藤花子");

        // 等価性を検証する
        var result = customer1.Equals(customer2);

        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod(DisplayName = "異なるUUIDで非等価と判定される")]
    public void Equals_WithDifferentUuid_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var customer1 = CreateCustomer(customerName: "山田太郎");
        var customer2 = CreateCustomer(customerName: "佐藤花子");

        // 等価性を検証する
        var result = customer1.Equals(customer2);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "nullと比較した場合、非等価と判定される")]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var customer = CreateCustomer();

        // 等価性を検証する
        var result = customer.Equals(null);

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "異なる型と比較した場合、非等価と判定される")]
    public void Equals_WithDifferentType_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var customer = CreateCustomer();

        // 等価性を検証する
        var result = customer.Equals("customer");

        // 非等価であることを評価する
        Assert.IsFalse(result);
    }

    [TestMethod(DisplayName = "同じUUIDの場合、同じハッシュコードが返される")]
    public void GetHashCode_WithSameUuid_ShouldReturnSameHashCode()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid();
        var customer1 = CreateCustomer(customerUuid: uuid, customerName: "山田太郎");
        var customer2 = CreateCustomer(customerUuid: uuid, customerName: "佐藤花子");

        // ハッシュコードを取得する
        var hashCode1 = customer1.GetHashCode();
        var hashCode2 = customer2.GetHashCode();

        // ハッシュコードを検証する
        Assert.AreEqual(hashCode1, hashCode2);
    }

    [TestMethod(DisplayName = "ToStringで顧客情報が文字列化される")]
    public void ToString_ShouldContainCustomerProperties()
    {
        // データを用意する
        var customerUuid = Guid.NewGuid();
        var createdAt = DateTime.Now.AddDays(-1);

        // インスタンスを生成する
        var customer = CreateCustomer(
            customerUuid: customerUuid,
            customerName: "山田太郎",
            customerKana: "ヤマダタロウ",
            address1: "東京都千代田区",
            address2: "1-1-1",
            phoneNumber: "09012345678",
            mailAddress: "test@example.com",
            createdAt: createdAt);

        // 文字列化する
        var result = customer.ToString();

        // 文字列に顧客情報が含まれることを検証する
        StringAssert.Contains(result, customerUuid.ToString());
        StringAssert.Contains(result, "山田太郎");
        StringAssert.Contains(result, "ヤマダタロウ");
        StringAssert.Contains(result, "東京都千代田区");
        StringAssert.Contains(result, "1-1-1");
        StringAssert.Contains(result, "09012345678");
        StringAssert.Contains(result, "test@example.com");
        StringAssert.Contains(result, createdAt.ToString());
    }
}