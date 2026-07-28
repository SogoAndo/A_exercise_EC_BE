using A_exercise_EC_BE.Applications.Usecases.Purchases;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.ViewModels.Purchases;

namespace A_exercise_EC_BE.Presentations.Tests.Adapters;

[TestClass]
[TestCategory("Presentations/Adapters")]
public class PurchaseViewModelAdapterTests
{
    private readonly PurchaseViewModelAdapter adapter = new();

    [TestMethod]
    public void ConvertToRequest_WhenViewModelIsNull_ThrowsInternalException()
    {
        var exception = Assert.ThrowsExactly<InternalException>(
            () => adapter.ConvertToRequest(
                Guid.NewGuid(),
                null!));

        Assert.AreEqual(
            "引数viewModelがnullです。",
            exception.Message);
    }

    [TestMethod]
    public void ConvertToRequest_WhenItemsIsNull_ReturnsEmptyItems()
    {
        var viewModel = new ConfirmPurchaseViewModel
        {
            PaymentMethodId = 4,
            Items = null!
        };

        var result = adapter.ConvertToRequest(
            Guid.NewGuid(),
            viewModel);

        Assert.IsEmpty(result.Items);
    }

    [TestMethod]
    public void ConvertToRequest_WhenItemsExist_MapsItems()
    {
        var productUuid = Guid.NewGuid();
        var customerUuid = Guid.NewGuid();
        var viewModel = new ConfirmPurchaseViewModel
        {
            PaymentMethodId = 4,
            Items =
            [
                new ConfirmPurchaseItemViewModel
                {
                    ProductUuid = productUuid,
                    Quantity = 2
                }
            ]
        };

        var result = adapter.ConvertToRequest(
            customerUuid,
            viewModel);

        Assert.AreEqual(customerUuid, result.CustomerUuid);
        Assert.AreEqual(4, result.PaymentMethodId);
        Assert.HasCount(1, result.Items);
        Assert.AreEqual(productUuid, result.Items.First().ProductUuid);
        Assert.AreEqual(2, result.Items.First().Quantity);
    }

    [TestMethod]
    public void ConvertToCompleteViewModel_WhenResultIsNull_ThrowsInternalException()
    {
        var exception = Assert.ThrowsExactly<InternalException>(
            () => adapter.ConvertToCompleteViewModel(null!));

        Assert.AreEqual(
            "引数resultがnullです。",
            exception.Message);
    }

    [TestMethod]
    public void ConvertToCompleteViewModel_WhenResultIsValid_MapsAllValues()
    {
        var orderUuid = Guid.NewGuid();
        var result = new ConfirmPurchaseResult(
            orderUuid,
            new DateTime(2026, 7, 27, 10, 30, 0),
            500);

        var actual = adapter.ConvertToCompleteViewModel(result);

        Assert.AreEqual("購入が完了しました", actual.CompleteMessage);
        Assert.AreEqual(orderUuid, actual.OrderUuid);
        Assert.AreEqual("2026/07/27 10:30:00", actual.OrderDate);
        Assert.AreEqual(500, actual.TotalPrice);
    }
}
