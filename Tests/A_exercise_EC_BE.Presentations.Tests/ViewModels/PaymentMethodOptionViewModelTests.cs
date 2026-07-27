using A_exercise_EC_BE.Presentations.ViewModels.PaymentMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace A_exercise_EC_BE.Presentations.Tests
    .ViewModels.PaymentMethods;

/// <summary>
/// PaymentMethodOptionViewModelの単体テスト。
/// </summary>
[TestClass]
[TestCategory("Presentation/ViewModels/PaymentMethods")]
public class PaymentMethodOptionViewModelTests
{
    /// <summary>
    /// デフォルト値でインスタンスを生成できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "PaymentMethodOptionViewModel_デフォルト値でインスタンスを生成できる")]
    public void
        PaymentMethodOptionViewModel_DefaultValues_CanCreateInstance()
    {
        // Act
        var model =
            new PaymentMethodOptionViewModel();

        // Assert
        Assert.AreEqual(
            0,
            model.Value);

        Assert.AreEqual(
            string.Empty,
            model.Label);
    }

    /// <summary>
    /// 各プロパティに値を設定し、
    /// 同じ値を取得できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "PaymentMethodOptionViewModel_各プロパティに値を設定できる")]
    public void
        PaymentMethodOptionViewModel_SetProperties_CanGetSameValues()
    {
        // Arrange
        const int expectedValue =
            1;

        const string expectedLabel =
            "クレジットカード";

        // Act
        var model =
            new PaymentMethodOptionViewModel
            {
                Value =
                    expectedValue,

                Label =
                    expectedLabel
            };

        // Assert
        Assert.AreEqual(
            expectedValue,
            model.Value);

        Assert.AreEqual(
            expectedLabel,
            model.Label);
    }

    /// <summary>
    /// Labelへ空文字を設定できること。
    /// </summary>
    [TestMethod(
        DisplayName =
            "PaymentMethodOptionViewModel_Labelに空文字を設定できる")]
    public void
        PaymentMethodOptionViewModel_LabelIsEmpty_CanSetEmptyString()
    {
        // Arrange
        var model =
            new PaymentMethodOptionViewModel
            {
                Label =
                    "銀行振込"
            };

        // Act
        model.Label =
            string.Empty;

        // Assert
        Assert.AreEqual(
            string.Empty,
            model.Label);
    }
}