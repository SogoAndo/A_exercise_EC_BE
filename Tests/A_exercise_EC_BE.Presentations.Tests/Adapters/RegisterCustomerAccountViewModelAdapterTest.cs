using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.ViewModels.Accounts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_exercise_EC_BE.Presentations.Tests.Adapters;

/// <summary>
/// RegisterCustomerAccountViewModelAdapterの単体テスト
/// </summary>
[TestClass]
public class RegisterCustomerAccountViewModelAdapterTest
{
    private RegisterCustomerAccountViewModelAdapter
        _adapter = null!;

    /// <summary>
    /// 各テスト実行前の初期化処理
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _adapter =
            new RegisterCustomerAccountViewModelAdapter();
    }

    /// <summary>
    /// Convertで入力用ViewModelを
    /// Customerへ変換できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "Convert_入力用ViewModelをCustomerへ変換できる")]
    public void Convert_CanConvertToCustomer()
    {
        // Arrange
        var viewModel =
            new RegisterCustomerAccountViewModel
            {
                Name =
                    "  山田太郎  ",

                Kana =
                    "  ヤマダタロウ  ",

                Address1 =
                    "  東京都千代田区丸の内1-1  ",

                Address2 =
                    "  テストマンション101号室  ",

                PhoneNumber =
                    "  09012345678  ",

                MailAddress =
                    "  yamada@example.com  ",

                Username =
                    "  yamada01  ",

                Password =
                    "password"
            };

        var beforeConvert =
            DateTime.Now;

        // Act
        var actual =
            _adapter.Convert(viewModel);

        var afterConvert =
            DateTime.Now;

        // Assert
        Assert.IsNotNull(actual);

        Assert.AreNotEqual(
            Guid.Empty,
            actual.CustomerUuid);

        Assert.AreEqual(
            "山田太郎",
            actual.Name);

        Assert.AreEqual(
            "ヤマダタロウ",
            actual.Kana);

        Assert.AreEqual(
            "東京都千代田区丸の内1-1",
            actual.Address1);

        Assert.AreEqual(
            "テストマンション101号室",
            actual.Address2);

        Assert.AreEqual(
            "09012345678",
            actual.PhoneNumber);

        Assert.AreEqual(
            "yamada@example.com",
            actual.MailAddress);

        Assert.AreEqual(
            "yamada01",
            actual.Username);

        /*
         * パスワードにはTrimが実行されないため、
         * 入力値がそのまま設定されることを確認する。
         */
        Assert.AreEqual(
            "password",
            actual.Password);

        Assert.IsTrue(
            actual.CreatedAt >= beforeConvert);

        Assert.IsTrue(
            actual.CreatedAt <= afterConvert);
    }

    /// <summary>
    /// ConvertでAddress2が空白の場合、
    /// nullへ正規化されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "Convert_Address2が空白の場合はnullへ正規化される")]
    public void Convert_WhenAddress2IsWhiteSpace_NormalizesToNull()
    {
        // Arrange
        var viewModel =
            CreateViewModel(
                address2: "   ");

        // Act
        var actual =
            _adapter.Convert(viewModel);

        // Assert
        Assert.IsNull(actual.Address2);
    }

    /// <summary>
    /// ConvertでAddress2がnullの場合、
    /// nullへ正規化されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "Convert_Address2がnullの場合はnullへ正規化される")]
    public void Convert_WhenAddress2IsNull_NormalizesToNull()
    {
        // Arrange
        var viewModel =
            CreateViewModel(
                address2: null);

        // Act
        var actual =
            _adapter.Convert(viewModel);

        // Assert
        Assert.IsNull(actual.Address2);
    }

    /// <summary>
    /// ConvertでviewModelがnullの場合、
    /// InternalExceptionをスローすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "Convert_viewModelがnullの場合はInternalExceptionをスローする")]
    public void
        Convert_WhenViewModelIsNull_ThrowsExactlyInternalException()
    {
        // Act
        var exception =
            Assert.ThrowsExactly<InternalException>(
                () =>
                {
                    _adapter.Convert(null!);
                });

        // Assert
        Assert.AreEqual(
            "引数viewModelがnullです。",
            exception.Message);
    }

    /// <summary>
    /// ToConfirmViewModelで入力用ViewModelを
    /// 確認画面用ViewModelへ変換できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "ToConfirmViewModel_確認画面用ViewModelへ変換できる")]
    public void
        ToConfirmViewModel_CanConvertToConfirmViewModel()
    {
        // Arrange
        var viewModel =
            new RegisterCustomerAccountViewModel
            {
                Name =
                    "  山田太郎  ",

                Kana =
                    "  ヤマダタロウ  ",

                Address1 =
                    "  東京都千代田区丸の内1-1  ",

                Address2 =
                    "  テストマンション101号室  ",

                PhoneNumber =
                    "  09012345678  ",

                MailAddress =
                    "  yamada@example.com  ",

                Username =
                    "  yamada01  ",

                Password =
                    "password"
            };

        // Act
        var actual =
            _adapter.ToConfirmViewModel(
                viewModel);

        // Assert
        Assert.IsNotNull(actual);

        Assert.AreEqual(
            "顧客アカウント登録(確認)",
            actual.Title);

        Assert.AreEqual(
            "山田太郎",
            actual.Name);

        Assert.AreEqual(
            "ヤマダタロウ",
            actual.Kana);

        Assert.AreEqual(
            "東京都千代田区丸の内1-1",
            actual.Address1);

        Assert.AreEqual(
            "テストマンション101号室",
            actual.Address2);

        Assert.AreEqual(
            "09012345678",
            actual.PhoneNumber);

        Assert.AreEqual(
            "yamada@example.com",
            actual.MailAddress);

        Assert.AreEqual(
            "yamada01",
            actual.Username);

        Assert.AreEqual(
            "********",
            actual.PasswordMask);
    }

    /// <summary>
    /// ToConfirmViewModelでAddress2が空白の場合、
    /// nullへ正規化されること
    /// </summary>
    [TestMethod(
        DisplayName =
            "ToConfirmViewModel_Address2が空白の場合はnullへ正規化される")]
    public void
        ToConfirmViewModel_WhenAddress2IsWhiteSpace_NormalizesToNull()
    {
        // Arrange
        var viewModel =
            CreateViewModel(
                address2: "   ");

        // Act
        var actual =
            _adapter.ToConfirmViewModel(
                viewModel);

        // Assert
        Assert.IsNull(actual.Address2);
    }

    /// <summary>
    /// ToConfirmViewModelでviewModelがnullの場合、
    /// InternalExceptionをスローすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "ToConfirmViewModel_viewModelがnullの場合はInternalExceptionをスローする")]
    public void
        ToConfirmViewModel_WhenViewModelIsNull_ThrowsExactlyInternalException()
    {
        // Act
        var exception =
            Assert.ThrowsExactly<InternalException>(
                () =>
                {
                    _adapter.ToConfirmViewModel(
                        null!);
                });

        // Assert
        Assert.AreEqual(
            "引数viewModelがnullです。",
            exception.Message);
    }

    /// <summary>
    /// ToCompleteViewModelでCustomerを
    /// 完了画面用ViewModelへ変換できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "ToCompleteViewModel_完了画面用ViewModelへ変換できる")]
    public void
        ToCompleteViewModel_CanConvertToCompleteViewModel()
    {
        // Arrange
        var createdAt =
            new DateTime(
                2026,
                7,
                24,
                9,
                30,
                0);

        var customer =
            new Customer(
                "山田太郎",
                "ヤマダタロウ",
                "東京都千代田区丸の内1-1",
                "テストマンション101号室",
                "09012345678",
                "yamada@example.com",
                "yamada01",
                "password",
                createdAt);

        var expectedCustomerUuid =
            customer.CustomerUuid;

        // Act
        var actual =
            _adapter.ToCompleteViewModel(
                customer);

        // Assert
        Assert.IsNotNull(actual);

        Assert.AreEqual(
            "顧客アカウント登録(完了)",
            actual.Title);

        Assert.AreEqual(
            "顧客アカウントの登録が完了しました。",
            actual.Message);

        Assert.AreEqual(
            expectedCustomerUuid,
            actual.CustomerUuid);

        Assert.AreEqual(
            "山田太郎",
            actual.Name);

        Assert.AreEqual(
            "yamada01",
            actual.Username);

        Assert.AreEqual(
            createdAt,
            actual.CreatedAt);
    }

    /// <summary>
    /// ToCompleteViewModelでcustomerがnullの場合、
    /// InternalExceptionをスローすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "ToCompleteViewModel_customerがnullの場合はInternalExceptionをスローする")]
    public void
        ToCompleteViewModel_WhenCustomerIsNull_ThrowsExactlyInternalException()
    {
        // Act
        var exception =
            Assert.ThrowsExactly<InternalException>(
                () =>
                {
                    _adapter.ToCompleteViewModel(
                        null!);
                });

        // Assert
        Assert.AreEqual(
            "引数customerがnullです。",
            exception.Message);
    }

    /// <summary>
    /// テスト用の入力ViewModelを生成する
    /// </summary>
    /// <param name="address2">
    /// 建物名・部屋番号
    /// </param>
    /// <returns>
    /// テスト用の入力ViewModel
    /// </returns>
    private static RegisterCustomerAccountViewModel
        CreateViewModel(
            string? address2)
    {
        return new RegisterCustomerAccountViewModel
        {
            Name =
                "山田太郎",

            Kana =
                "ヤマダタロウ",

            Address1 =
                "東京都千代田区丸の内1-1",

            Address2 =
                address2,

            PhoneNumber =
                "09012345678",

            MailAddress =
                "yamada@example.com",

            Username =
                "yamada01",

            Password =
                "password"
        };
    }
}