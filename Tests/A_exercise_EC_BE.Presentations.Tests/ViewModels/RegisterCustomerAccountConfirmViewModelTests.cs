using A_exercise_EC_BE.Presentations.ViewModels.Accounts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_exercise_EC_BE.Presentations.Tests.ViewModels.Accounts;

/// <summary>
/// RegisterCustomerAccountConfirmViewModelの単体テスト
/// </summary>
[TestClass]
public class RegisterCustomerAccountConfirmViewModelTest
{
    /// <summary>
    /// デフォルト値でインスタンスを生成できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountConfirmViewModel_デフォルト値でインスタンスを生成できる")]
    public void
        RegisterCustomerAccountConfirmViewModel_DefaultValue_CanCreateInstance()
    {
        // Act
        var model =
            new RegisterCustomerAccountConfirmViewModel();

        // Assert
        Assert.AreEqual(
            string.Empty,
            model.Title);

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
            model.PasswordMask);
    }

    /// <summary>
    /// 各プロパティに値を設定できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountConfirmViewModel_各プロパティに値を設定できる")]
    public void
        RegisterCustomerAccountConfirmViewModel_SetProperties_CanGetSameValues()
    {
        // Act
        var model =
            new RegisterCustomerAccountConfirmViewModel
            {
                Title =
                    "顧客アカウント登録確認",

                Name =
                    "山田太郎",

                Kana =
                    "ヤマダタロウ",

                Address1 =
                    "東京都千代田区1-1",

                Address2 =
                    "テストマンション101号室",

                PhoneNumber =
                    "090-1234-5678",

                MailAddress =
                    "yamada@example.com",

                Username =
                    "yamada01",

                PasswordMask =
                    "********"
            };

        // Assert
        Assert.AreEqual(
            "顧客アカウント登録確認",
            model.Title);

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
            "********",
            model.PasswordMask);
    }

    /// <summary>
    /// 住所2にnullを設定できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountConfirmViewModel_Address2にnullを設定できる")]
    public void
        RegisterCustomerAccountConfirmViewModel_Address2IsNull_CanGetNull()
    {
        // Arrange
        var model =
            new RegisterCustomerAccountConfirmViewModel
            {
                Address2 =
                    "テストマンション101号室"
            };

        // Act
        model.Address2 = null;

        // Assert
        Assert.IsNull(
            model.Address2);
    }
}