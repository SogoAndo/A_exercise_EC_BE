using System.ComponentModel.DataAnnotations;
using System.Reflection;
using A_exercise_EC_BE.Presentations.ViewModels.Accounts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_exercise_EC_BE.Presentations.Tests.ViewModels.Accounts;

/// <summary>
/// RegisterCustomerAccountViewModelの単体テスト
/// </summary>
[TestClass]
public class RegisterCustomerAccountViewModelTest
{
    /// <summary>
    /// デフォルト値でインスタンスを生成できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountViewModel_デフォルト値でインスタンスを生成できる")]
    public void
        RegisterCustomerAccountViewModel_DefaultValue_CanCreateInstance()
    {
        // Act
        var model =
            new RegisterCustomerAccountViewModel();

        // Assert
        Assert.AreEqual(
            string.Empty,
            model.Name);

        Assert.AreEqual(
            string.Empty,
            model.Kana);

        Assert.AreEqual(
            string.Empty,
            model.Address1);

        Assert.IsNull(
            model.Address2);

        Assert.AreEqual(
            string.Empty,
            model.PhoneNumber);

        Assert.AreEqual(
            string.Empty,
            model.MailAddress);

        Assert.AreEqual(
            string.Empty,
            model.Username);

        Assert.AreEqual(
            string.Empty,
            model.Password);
    }

    /// <summary>
    /// 各プロパティに値を設定できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountViewModel_各プロパティに値を設定できる")]
    public void
        RegisterCustomerAccountViewModel_SetProperties_CanGetSameValues()
    {
        // Act
        var model =
            new RegisterCustomerAccountViewModel
            {
                Name = "山田太郎",
                Kana = "ヤマダタロウ",
                Address1 = "東京都千代田区1-1",
                Address2 = "テストマンション101号室",
                PhoneNumber = "090-1234-5678",
                MailAddress = "yamada@example.com",
                Username = "yamada01",
                Password = "password"
            };

        // Assert
        Assert.AreEqual(
            "山田太郎",
            model.Name);

        Assert.AreEqual(
            "ヤマダタロウ",
            model.Kana);

        Assert.AreEqual(
            "東京都千代田区1-1",
            model.Address1);

        Assert.AreEqual(
            "テストマンション101号室",
            model.Address2);

        Assert.AreEqual(
            "090-1234-5678",
            model.PhoneNumber);

        Assert.AreEqual(
            "yamada@example.com",
            model.MailAddress);

        Assert.AreEqual(
            "yamada01",
            model.Username);

        Assert.AreEqual(
            "password",
            model.Password);
    }

    /// <summary>
    /// 必須項目が空文字の場合、
    /// Requiredの入力エラーになること
    /// </summary>
    [TestMethod]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Name),
        "氏名を入力してください",
        DisplayName =
            "Nameが空文字の場合は必須入力エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Kana),
        "氏名カナを入力してください",
        DisplayName =
            "Kanaが空文字の場合は必須入力エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Address1),
        "住所1を入力してください",
        DisplayName =
            "Address1が空文字の場合は必須入力エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.PhoneNumber),
        "電話番号を入力してください",
        DisplayName =
            "PhoneNumberが空文字の場合は必須入力エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.MailAddress),
        "メールアドレスを入力してください",
        DisplayName =
            "MailAddressが空文字の場合は必須入力エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Username),
        "アカウント名を入力してください",
        DisplayName =
            "Usernameが空文字の場合は必須入力エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Password),
        "パスワードを入力してください",
        DisplayName =
            "Passwordが空文字の場合は必須入力エラーになる")]
    public void
        RegisterCustomerAccountViewModel_RequiredPropertyIsEmpty_HasRequiredError(
            string propertyName,
            string expectedMessage)
    {
        // Arrange
        var model =
            CreateValidModel();

        SetProperty(
            model,
            propertyName,
            string.Empty);

        // Act
        var results =
            ValidateModel(model);

        // Assert
        AssertHasValidationError(
            results,
            propertyName,
            expectedMessage);
    }

    /// <summary>
    /// 最小文字数未満の場合、
    /// StringLengthの入力エラーになること
    /// </summary>
    [TestMethod]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Name),
        "山",
        "氏名は2文字以上20文字以内で入力してください",
        DisplayName =
            "Nameが2文字未満の場合は文字数エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Kana),
        "ヤ",
        "氏名カナは2文字以上20文字以内で入力してください",
        DisplayName =
            "Kanaが2文字未満の場合は文字数エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.MailAddress),
        "abc",
        "メールアドレスは4文字以上200文字以内で入力してください",
        DisplayName =
            "MailAddressが4文字未満の場合は文字数エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Username),
        "abcd",
        "アカウント名は5文字以上30文字以内で入力してください",
        DisplayName =
            "Usernameが5文字未満の場合は文字数エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Password),
        "1234",
        "パスワードは5文字以上20文字以内で入力してください",
        DisplayName =
            "Passwordが5文字未満の場合は文字数エラーになる")]
    public void
        RegisterCustomerAccountViewModel_PropertyIsTooShort_HasLengthError(
            string propertyName,
            string invalidValue,
            string expectedMessage)
    {
        // Arrange
        var model =
            CreateValidModel();

        SetProperty(
            model,
            propertyName,
            invalidValue);

        // Act
        var results =
            ValidateModel(model);

        // Assert
        AssertHasValidationError(
            results,
            propertyName,
            expectedMessage);
    }

    /// <summary>
    /// 最大文字数を超える場合、
    /// StringLengthの入力エラーになること
    /// </summary>
    [TestMethod]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Name),
        21,
        "氏名は2文字以上20文字以内で入力してください",
        DisplayName =
            "Nameが20文字を超える場合は文字数エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Kana),
        21,
        "氏名カナは2文字以上20文字以内で入力してください",
        DisplayName =
            "Kanaが20文字を超える場合は文字数エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Address1),
        101,
        "住所1は100文字以内で入力してください",
        DisplayName =
            "Address1が100文字を超える場合は文字数エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Address2),
        101,
        "住所2は100文字以内で入力してください",
        DisplayName =
            "Address2が100文字を超える場合は文字数エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.PhoneNumber),
        21,
        "電話番号は20文字以内で入力してください",
        DisplayName =
            "PhoneNumberが20文字を超える場合は文字数エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.MailAddress),
        201,
        "メールアドレスは4文字以上200文字以内で入力してください",
        DisplayName =
            "MailAddressが200文字を超える場合は文字数エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Username),
        31,
        "アカウント名は5文字以上30文字以内で入力してください",
        DisplayName =
            "Usernameが30文字を超える場合は文字数エラーになる")]
    [DataRow(
        nameof(RegisterCustomerAccountViewModel.Password),
        21,
        "パスワードは5文字以上20文字以内で入力してください",
        DisplayName =
            "Passwordが20文字を超える場合は文字数エラーになる")]
    public void
        RegisterCustomerAccountViewModel_PropertyIsTooLong_HasLengthError(
            string propertyName,
            int invalidLength,
            string expectedMessage)
    {
        // Arrange
        var model =
            CreateValidModel();

        SetProperty(
            model,
            propertyName,
            new string('a', invalidLength));

        // Act
        var results =
            ValidateModel(model);

        // Assert
        AssertHasValidationError(
            results,
            propertyName,
            expectedMessage);
    }

    /// <summary>
    /// メールアドレスの形式が不正な場合、
    /// EmailAddressの入力エラーになること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountViewModel_メールアドレスの形式が不正な場合は形式エラーになる")]
    public void
        RegisterCustomerAccountViewModel_MailAddressIsInvalid_HasFormatError()
    {
        // Arrange
        var model =
            CreateValidModel();

        model.MailAddress =
            "invalid-mail-address";

        // Act
        var results =
            ValidateModel(model);

        // Assert
        AssertHasValidationError(
            results,
            nameof(
                RegisterCustomerAccountViewModel
                    .MailAddress),
            "メールアドレスの形式が正しくありません");
    }

    /// <summary>
    /// 住所2がnullの場合でも、
    /// 入力チェックが正常終了すること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountViewModel_Address2がnullの場合でも入力チェックが正常終了する")]
    public void
        RegisterCustomerAccountViewModel_Address2IsNull_IsValid()
    {
        // Arrange
        var model =
            CreateValidModel();

        model.Address2 = null;

        // Act
        var results =
            ValidateModel(model);

        // Assert
        Assert.IsEmpty(
            results);
    }

    /// <summary>
    /// 各プロパティが正常な場合、
    /// 入力チェックが正常終了すること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountViewModel_正常な入力値の場合は入力チェックが正常終了する")]
    public void
        RegisterCustomerAccountViewModel_ValidValues_IsValid()
    {
        // Arrange
        var model =
            CreateValidModel();

        // Act
        var results =
            ValidateModel(model);

        // Assert
        Assert.IsEmpty(
            results);
    }

    /// <summary>
    /// Passwordプロパティに
    /// DataType.Passwordが設定されていること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountViewModel_PasswordにDataType.Passwordが設定されている")]
    public void
        RegisterCustomerAccountViewModel_Password_HasPasswordDataType()
    {
        // Arrange
        var property =
            typeof(RegisterCustomerAccountViewModel)
                .GetProperty(
                    nameof(
                        RegisterCustomerAccountViewModel
                            .Password));

        Assert.IsNotNull(property);

        // Act
        var attribute =
            property.GetCustomAttribute<
                DataTypeAttribute>();

        // Assert
        Assert.IsNotNull(attribute);

        Assert.AreEqual(
            DataType.Password,
            attribute.DataType);
    }

    /// <summary>
    /// 正常な入力値を持つViewModelを生成する
    /// </summary>
    /// <returns>
    /// 正常なViewModel
    /// </returns>
    private static RegisterCustomerAccountViewModel
        CreateValidModel()
    {
        return new RegisterCustomerAccountViewModel
        {
            Name = "山田太郎",
            Kana = "ヤマダタロウ",
            Address1 = "東京都千代田区1-1",
            Address2 = "テストマンション101号室",
            PhoneNumber = "090-1234-5678",
            MailAddress = "yamada@example.com",
            Username = "yamada01",
            Password = "password"
        };
    }

    /// <summary>
    /// ViewModelの入力チェックを実行する
    /// </summary>
    /// <param name="model">
    /// 検証対象
    /// </param>
    /// <returns>
    /// 入力チェック結果
    /// </returns>
    private static List<ValidationResult>
        ValidateModel(
            RegisterCustomerAccountViewModel model)
    {
        var results =
            new List<ValidationResult>();

        var context =
            new ValidationContext(model);

        Validator.TryValidateObject(
            model,
            context,
            results,
            validateAllProperties: true);

        return results;
    }

    /// <summary>
    /// 指定したプロパティに値を設定する
    /// </summary>
    /// <param name="model">
    /// 設定対象
    /// </param>
    /// <param name="propertyName">
    /// プロパティ名
    /// </param>
    /// <param name="value">
    /// 設定値
    /// </param>
    private static void SetProperty(
        RegisterCustomerAccountViewModel model,
        string propertyName,
        string value)
    {
        var property =
            typeof(RegisterCustomerAccountViewModel)
                .GetProperty(propertyName);

        if (property is null)
        {
            throw new InvalidOperationException(
                $"{propertyName}が見つかりません。");
        }

        property.SetValue(
            model,
            value);
    }

    /// <summary>
    /// 指定した入力エラーが含まれることを検証する
    /// </summary>
    /// <param name="results">
    /// 入力チェック結果
    /// </param>
    /// <param name="propertyName">
    /// 対象プロパティ名
    /// </param>
    /// <param name="expectedMessage">
    /// 期待するメッセージ
    /// </param>
    private static void AssertHasValidationError(
        IEnumerable<ValidationResult> results,
        string propertyName,
        string expectedMessage)
    {
        var hasExpectedError =
            results.Any(
                result =>
                    result.ErrorMessage ==
                        expectedMessage &&
                    result.MemberNames.Contains(
                        propertyName));

        Assert.IsTrue(
            hasExpectedError,
            $"期待した入力エラーがありません。" +
            $" Property={propertyName}," +
            $" Message={expectedMessage}");
    }
}