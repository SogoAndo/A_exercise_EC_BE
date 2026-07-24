using A_exercise_EC_BE.Presentations.ViewModels.Accounts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_exercise_EC_BE.Presentations.Tests.ViewModels.Accounts;

/// <summary>
/// RegisterCustomerAccountCompleteViewModelの単体テスト
/// </summary>
[TestClass]
public class RegisterCustomerAccountCompleteViewModelTest
{
    /// <summary>
    /// デフォルト値でインスタンスを生成できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountCompleteViewModel_デフォルト値でインスタンスを生成できる")]
    public void
        RegisterCustomerAccountCompleteViewModel_DefaultValue_CanCreateInstance()
    {
        // Act
        var model =
            new RegisterCustomerAccountCompleteViewModel();

        // Assert
        Assert.AreEqual(
            string.Empty,
            model.Title);

        Assert.AreEqual(
            string.Empty,
            model.Message);

        Assert.AreEqual(
            Guid.Empty,
            model.CustomerUuid);

        Assert.AreEqual(
            string.Empty,
            model.Name);

        Assert.AreEqual(
            string.Empty,
            model.Username);

        Assert.AreEqual(
            default,
            model.CreatedAt);
    }

    /// <summary>
    /// 各プロパティに値を設定できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountCompleteViewModel_各プロパティに値を設定できる")]
    public void
        RegisterCustomerAccountCompleteViewModel_SetProperties_CanGetSameValues()
    {
        // Arrange
        var customerUuid =
            Guid.NewGuid();

        var createdAt =
            new DateTime(
                2026,
                7,
                24,
                10,
                30,
                0,
                DateTimeKind.Utc);

        // Act
        var model =
            new RegisterCustomerAccountCompleteViewModel
            {
                Title =
                    "顧客アカウント登録完了",

                Message =
                    "顧客アカウントの登録が完了しました",

                CustomerUuid =
                    customerUuid,

                Name =
                    "山田太郎",

                Username =
                    "yamada01",

                CreatedAt =
                    createdAt
            };

        // Assert
        Assert.AreEqual(
            "顧客アカウント登録完了",
            model.Title);

        Assert.AreEqual(
            "顧客アカウントの登録が完了しました",
            model.Message);

        Assert.AreEqual(
            customerUuid,
            model.CustomerUuid);

        Assert.AreEqual(
            "山田太郎",
            model.Name);

        Assert.AreEqual(
            "yamada01",
            model.Username);

        Assert.AreEqual(
            createdAt,
            model.CreatedAt);
    }
}