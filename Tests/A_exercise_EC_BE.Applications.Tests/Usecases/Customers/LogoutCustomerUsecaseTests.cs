using A_exercise_EC_BE.Applications.Usecases.Customers;

namespace A_exercise_EC_BE.Applications.Tests.Usecases.Customers;

[TestClass]
[TestCategory("Applications/Usecases/Customers")]
public class LogoutCustomerUsecaseTests
{
    [TestMethod]
    public async Task LogoutAsync_ReturnsLoggedOutResult()
    {
        var usecase = new LogoutCustomerUsecase();

        var result = await usecase.LogoutAsync();

        Assert.IsTrue(result.LoggedOut);
    }
}
