using A_exercise_EC_BE.Applications.Usecases.Accounts;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Presentations.Adapters;
using A_exercise_EC_BE.Presentations.Controllers;
using A_exercise_EC_BE.Presentations.ViewModels.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace A_exercise_EC_BE.Presentations.Tests.Controllers;

/// <summary>
/// RegisterCustomerAccountControllerの単体テスト
/// </summary>
[TestClass]
[TestCategory("Controllers")]
public class RegisterCustomerAccountControllerTests
{
    private const string Username =
        "yamada";

    private const string MailAddress =
        "yamada@example.com";

    private Mock<IRegisterCustomerAccountUsecase>
        _usecaseMock = null!;

    private Mock<ILogger<RegisterCustomerAccountController>>
        _loggerMock = null!;

    private RegisterCustomerAccountViewModelAdapter
        _adapter = null!;

    private RegisterCustomerAccountController
        _controller = null!;

    /// <summary>
    /// テストの前処理
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _usecaseMock =
            new Mock<IRegisterCustomerAccountUsecase>();

        _loggerMock =
            new Mock<
                ILogger<
                    RegisterCustomerAccountController>>();

        _adapter =
            new RegisterCustomerAccountViewModelAdapter();

        _controller =
            new RegisterCustomerAccountController(
                _usecaseMock.Object,
                _adapter,
                _loggerMock.Object);
    }

    /// <summary>
    /// 入力画面情報を取得できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "GetForm_入力画面情報を取得できる")]
    public void GetForm_ReturnsOk()
    {
        // Act
        var actual =
            _controller.GetForm();

        // Assert
        var result =
            AssertResult<OkObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status200OK,
            result.StatusCode);

        Assert.AreEqual(
            "顧客アカウント登録(入力)",
            GetProperty<string>(
                result.Value,
                "title"));

        var model =
            GetProperty<object>(
                result.Value,
                "model");

        Assert.IsInstanceOfType(
            model,
            typeof(
                RegisterCustomerAccountViewModel));

        _usecaseMock.Verify(
            x => x.ExistsByUsernameAsync(
                It.IsAny<string>()),
            Times.Never);

        _usecaseMock.Verify(
            x => x.ExistsByMailAddressAsync(
                It.IsAny<string>()),
            Times.Never);

        _usecaseMock.Verify(
            x => x.RegisterCustomerAccountAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// 入力画面情報の生成中に例外が発生した場合、
    /// InternalServerErrorを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "GetForm_Ok生成時に例外が発生した場合はInternalServerErrorを返す")]
    public void GetForm_WhenOkThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var expected =
            new InvalidOperationException(
                "Okの生成に失敗しました。");

        var controller =
            new ThrowingOkController(
                _usecaseMock.Object,
                _adapter,
                _loggerMock.Object,
                expected);

        // Act
        var actual =
            controller.GetForm();

        // Assert
        var result =
            AssertResult<ObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes
                .Status500InternalServerError,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "SYSTEM_ERROR",
            "画面情報の取得に失敗しました");

        VerifyErrorLog(
            expected,
            "顧客アカウント登録入力画面の" +
            "初期表示情報取得中にエラーが発生しました。");
    }

    /// <summary>
    /// 使用できるアカウント名の場合、
    /// Okを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "ValidateUsername_使用できるアカウント名の場合はOkを返す")]
    public async Task
        ValidateUsername_WhenAvailable_ReturnsOk()
    {
        // Arrange
        _usecaseMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    Username))
            .Returns(Task.CompletedTask);

        // Act
        var actual =
            await _controller
                .ValidateUsername(
                    Username);

        // Assert
        var result =
            AssertResult<OkObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status200OK,
            result.StatusCode);

        Assert.IsFalse(
            GetProperty<bool>(
                result.Value,
                "exists"));

        Assert.AreEqual(
            "使用できるアカウント名です",
            GetProperty<string>(
                result.Value,
                "message"));

        _usecaseMock.Verify(
            x => x.ExistsByUsernameAsync(
                Username),
            Times.Once);
    }

    /// <summary>
    /// アカウント名が既に存在する場合、
    /// Conflictを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "ValidateUsername_アカウント名が既に存在する場合はConflictを返す")]
    public async Task
        ValidateUsername_WhenAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var expected =
            new ExistsException(
                "このアカウント名は既に使用されています");

        _usecaseMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    Username))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await _controller
                .ValidateUsername(
                    Username);

        // Assert
        var result =
            AssertResult<ConflictObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            result.StatusCode);

        Assert.AreEqual(
            "USERNAME_ALREADY_EXISTS",
            GetProperty<string>(
                result.Value,
                "code"));

        Assert.IsTrue(
            GetProperty<bool>(
                result.Value,
                "exists"));

        Assert.AreEqual(
            expected.Message,
            GetProperty<string>(
                result.Value,
                "message"));
    }

    /// <summary>
    /// アカウント名が入力値不正の場合、
    /// BadRequestを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "ValidateUsername_入力値不正の場合はBadRequestを返す")]
    public async Task
        ValidateUsername_WhenDomainExceptionOccurs_ReturnsBadRequest()
    {
        // Arrange
        var expected =
            new DomainException(
                "アカウント名を入力してください");

        _usecaseMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    Username))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await _controller
                .ValidateUsername(
                    Username);

        // Assert
        var result =
            AssertResult<BadRequestObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "VALIDATION_ERROR",
            expected.Message);
    }

    /// <summary>
    /// アカウント名確認中に予期しない例外が
    /// 発生した場合、InternalServerErrorを
    /// 返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "ValidateUsername_予期しない例外発生時はInternalServerErrorを返す")]
    public async Task
        ValidateUsername_WhenUnexpectedExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        var expected =
            new InvalidOperationException(
                "DB接続に失敗しました。");

        _usecaseMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    Username))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await _controller
                .ValidateUsername(
                    Username);

        // Assert
        var result =
            AssertResult<ObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes
                .Status500InternalServerError,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "SYSTEM_ERROR",
            "システムエラーが発生しました。" +
            "管理者に連絡してください");

        VerifyErrorLog(
            expected,
            "アカウント名の存在確認中に" +
            "エラーが発生しました。");
    }

    /// <summary>
    /// 使用できるメールアドレスの場合、
    /// Okを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "ValidateMailAddress_使用できるメールアドレスの場合はOkを返す")]
    public async Task
        ValidateMailAddress_WhenAvailable_ReturnsOk()
    {
        // Arrange
        _usecaseMock
            .Setup(
                x => x.ExistsByMailAddressAsync(
                    MailAddress))
            .Returns(Task.CompletedTask);

        // Act
        var actual =
            await _controller
                .ValidateMailAddress(
                    MailAddress);

        // Assert
        var result =
            AssertResult<OkObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status200OK,
            result.StatusCode);

        Assert.IsFalse(
            GetProperty<bool>(
                result.Value,
                "exists"));

        Assert.AreEqual(
            "使用できるメールアドレスです",
            GetProperty<string>(
                result.Value,
                "message"));

        _usecaseMock.Verify(
            x => x.ExistsByMailAddressAsync(
                MailAddress),
            Times.Once);
    }

    /// <summary>
    /// メールアドレスが既に存在する場合、
    /// Conflictを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "ValidateMailAddress_メールアドレスが既に存在する場合はConflictを返す")]
    public async Task
        ValidateMailAddress_WhenAlreadyExists_ReturnsConflict()
    {
        // Arrange
        var expected =
            new ExistsException(
                "このメールアドレスは既に使用されています");

        _usecaseMock
            .Setup(
                x => x.ExistsByMailAddressAsync(
                    MailAddress))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await _controller
                .ValidateMailAddress(
                    MailAddress);

        // Assert
        var result =
            AssertResult<ConflictObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            result.StatusCode);

        Assert.AreEqual(
            "MAIL_ADDRESS_ALREADY_EXISTS",
            GetProperty<string>(
                result.Value,
                "code"));

        Assert.IsTrue(
            GetProperty<bool>(
                result.Value,
                "exists"));

        Assert.AreEqual(
            expected.Message,
            GetProperty<string>(
                result.Value,
                "message"));
    }

    /// <summary>
    /// メールアドレスが入力値不正の場合、
    /// BadRequestを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "ValidateMailAddress_入力値不正の場合はBadRequestを返す")]
    public async Task
        ValidateMailAddress_WhenDomainExceptionOccurs_ReturnsBadRequest()
    {
        // Arrange
        var expected =
            new DomainException(
                "メールアドレスを入力してください");

        _usecaseMock
            .Setup(
                x => x.ExistsByMailAddressAsync(
                    MailAddress))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await _controller
                .ValidateMailAddress(
                    MailAddress);

        // Assert
        var result =
            AssertResult<BadRequestObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "VALIDATION_ERROR",
            expected.Message);
    }

    /// <summary>
    /// メールアドレス確認中に予期しない例外が
    /// 発生した場合、InternalServerErrorを
    /// 返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "ValidateMailAddress_予期しない例外発生時はInternalServerErrorを返す")]
    public async Task
        ValidateMailAddress_WhenUnexpectedExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        var expected =
            new InvalidOperationException(
                "DB接続に失敗しました。");

        _usecaseMock
            .Setup(
                x => x.ExistsByMailAddressAsync(
                    MailAddress))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await _controller
                .ValidateMailAddress(
                    MailAddress);

        // Assert
        var result =
            AssertResult<ObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes
                .Status500InternalServerError,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "SYSTEM_ERROR",
            "システムエラーが発生しました。" +
            "管理者に連絡してください");

        VerifyErrorLog(
            expected,
            "メールアドレスの存在確認中に" +
            "エラーが発生しました。");
    }

    /// <summary>
    /// ConfirmのModelStateが不正の場合、
    /// BadRequestを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Confirm_ModelStateが不正の場合はBadRequestを返す")]
    public async Task
        Confirm_WhenModelStateIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        /*
         * ErrorMessageが空白の分岐と、
         * ErrorMessageが設定済みの分岐を
         * 1回のテストで両方通す。
         */
        _controller.ModelState.AddModelError(
        "Name",
        string.Empty);

        _controller.ModelState.AddModelError(
            "MailAddress",
            "メールアドレスが不正です");

        // Act
        var actual =
            await _controller.Confirm(
                model);

        // Assert
        var result =
            AssertResult<BadRequestObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            result.StatusCode);

        Assert.AreEqual(
            "VALIDATION_ERROR",
            GetProperty<string>(
                result.Value,
                "code"));

        var messages =
            GetProperty<List<string>>(
                result.Value,
                "messages");

        Assert.HasCount(
            2,
            messages);

        CollectionAssert.Contains(
            messages,
            "入力内容が正しくありません");

        CollectionAssert.Contains(
            messages,
            "メールアドレスが不正です");

        VerifyConfirmUsecaseWasNotCalled();
    }

    /// <summary>
    /// 入力内容を確認できる場合、
    /// Okを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Confirm_入力内容を確認できる場合はOkを返す")]
    public async Task
        Confirm_WhenValid_ReturnsOk()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        SetupNoDuplicates();

        // Act
        var actual =
            await _controller.Confirm(
                model);

        // Assert
        var result =
            AssertResult<OkObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status200OK,
            result.StatusCode);

        Assert.IsInstanceOfType(
            result.Value,
            typeof(
                RegisterCustomerAccountConfirmViewModel));

        _usecaseMock.Verify(
            x => x.ExistsByUsernameAsync(
                Username),
            Times.Once);

        _usecaseMock.Verify(
            x => x.ExistsByMailAddressAsync(
                MailAddress),
            Times.Once);

        _usecaseMock.Verify(
            x => x.RegisterCustomerAccountAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// 確認処理で入力内容が重複する場合、
    /// Conflictを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Confirm_入力内容が重複する場合はConflictを返す")]
    public async Task
        Confirm_WhenExistsExceptionOccurs_ReturnsConflict()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        var expected =
            new ExistsException(
                "このアカウント名は既に使用されています");

        _usecaseMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    Username))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await _controller.Confirm(
                model);

        // Assert
        var result =
            AssertResult<ConflictObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "CUSTOMER_ACCOUNT_ALREADY_EXISTS",
            expected.Message);

        _usecaseMock.Verify(
            x => x.ExistsByMailAddressAsync(
                It.IsAny<string>()),
            Times.Never);
    }

    /// <summary>
    /// 確認処理で入力値不正が発生した場合、
    /// BadRequestを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Confirm_DomainException発生時はBadRequestを返す")]
    public async Task
        Confirm_WhenDomainExceptionOccurs_ReturnsBadRequest()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        var expected =
            new DomainException(
                "アカウント名を入力してください");

        _usecaseMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    Username))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await _controller.Confirm(
                model);

        // Assert
        var result =
            AssertResult<BadRequestObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "VALIDATION_ERROR",
            expected.Message);
    }

    /// <summary>
    /// 確認処理で内部例外が発生した場合、
    /// InternalServerErrorを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Confirm_InternalException発生時はInternalServerErrorを返す")]
    public async Task
        Confirm_WhenInternalExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        var expected =
            new InternalException(
                "確認処理に失敗しました。");

        _usecaseMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    Username))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await _controller.Confirm(
                model);

        // Assert
        var result =
            AssertResult<ObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes
                .Status500InternalServerError,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "INTERNAL_ERROR",
            "確認処理に失敗しました。" +
            "管理者に連絡してください");

        VerifyErrorLog(
            expected,
            "顧客アカウント登録確認処理中に" +
            "内部エラーが発生しました。");
    }

    /// <summary>
    /// 確認処理で予期しない例外が発生した場合、
    /// InternalServerErrorを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Confirm_予期しない例外発生時はInternalServerErrorを返す")]
    public async Task
        Confirm_WhenUnexpectedExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        var expected =
            new InvalidOperationException(
                "予期しないエラーです。");

        _usecaseMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    Username))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await _controller.Confirm(
                model);

        // Assert
        var result =
            AssertResult<ObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes
                .Status500InternalServerError,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "SYSTEM_ERROR",
            "システムエラーが発生しました。" +
            "管理者に連絡してください");

        VerifyErrorLog(
            expected,
            "顧客アカウント登録確認処理中に" +
            "予期しないエラーが発生しました。");
    }

    /// <summary>
    /// CompleteのModelStateが不正の場合、
    /// BadRequestを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Complete_ModelStateが不正の場合はBadRequestを返す")]
    public async Task
        Complete_WhenModelStateIsInvalid_ReturnsBadRequest()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        _controller.ModelState.AddModelError(
            "Username",
            "アカウント名が不正です");

        // Act
        var actual =
            await _controller.Complete(
                model);

        // Assert
        var result =
            AssertResult<BadRequestObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            result.StatusCode);

        Assert.AreEqual(
            "VALIDATION_ERROR",
            GetProperty<string>(
                result.Value,
                "code"));

        var messages =
            GetProperty<List<string>>(
                result.Value,
                "messages");

        CollectionAssert.AreEqual(
            new List<string>
            {
                "アカウント名が不正です"
            },
            messages);

        _usecaseMock.Verify(
            x => x.RegisterCustomerAccountAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// 顧客アカウントを登録できる場合、
    /// Createdを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Complete_顧客アカウントを登録できる場合はCreatedを返す")]
    public async Task
        Complete_WhenValid_ReturnsCreated()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        Customer? registeredCustomer =
            null;

        _usecaseMock
            .Setup(
                x => x.RegisterCustomerAccountAsync(
                    It.IsAny<Customer>()))
            .Callback<Customer>(
                customer =>
                    registeredCustomer =
                        customer)
            .Returns(Task.CompletedTask);

        // Act
        var actual =
            await _controller.Complete(
                model);

        // Assert
        var result =
            AssertResult<CreatedResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status201Created,
            result.StatusCode);

        Assert.IsNotNull(
            registeredCustomer);

        Assert.AreEqual(
            $"/account/" +
            $"{registeredCustomer.CustomerUuid}",
            result.Location);

        Assert.IsInstanceOfType(
            result.Value,
            typeof(
                RegisterCustomerAccountCompleteViewModel));

        _usecaseMock.Verify(
            x => x.RegisterCustomerAccountAsync(
                It.Is<Customer>(
                    customer =>
                        customer.Name ==
                            "山田太郎" &&
                        customer.Kana ==
                            "ヤマダタロウ" &&
                        customer.MailAddress ==
                            MailAddress &&
                        customer.Username ==
                            Username &&
                        customer.Password ==
                            "password")),
            Times.Once);
    }

    /// <summary>
    /// 登録処理で入力内容が重複する場合、
    /// Conflictを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Complete_ExistsException発生時はConflictを返す")]
    public async Task
        Complete_WhenExistsExceptionOccurs_ReturnsConflict()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        var expected =
            new ExistsException(
                "このアカウント名は既に使用されています");

        SetupRegisterThrows(
            expected);

        // Act
        var actual =
            await _controller.Complete(
                model);

        // Assert
        var result =
            AssertResult<ConflictObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status409Conflict,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "CUSTOMER_ACCOUNT_ALREADY_EXISTS",
            expected.Message);
    }

    /// <summary>
    /// 登録処理で入力値不正が発生した場合、
    /// BadRequestを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Complete_DomainException発生時はBadRequestを返す")]
    public async Task
        Complete_WhenDomainExceptionOccurs_ReturnsBadRequest()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        var expected =
            new DomainException(
                "パスワードが不正です");

        SetupRegisterThrows(
            expected);

        // Act
        var actual =
            await _controller.Complete(
                model);

        // Assert
        var result =
            AssertResult<BadRequestObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes.Status400BadRequest,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "VALIDATION_ERROR",
            expected.Message);
    }

    /// <summary>
    /// 登録処理で内部例外が発生した場合、
    /// InternalServerErrorを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Complete_InternalException発生時はInternalServerErrorを返す")]
    public async Task
        Complete_WhenInternalExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        var expected =
            new InternalException(
                "登録処理に失敗しました。");

        SetupRegisterThrows(
            expected);

        // Act
        var actual =
            await _controller.Complete(
                model);

        // Assert
        var result =
            AssertResult<ObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes
                .Status500InternalServerError,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "INTERNAL_ERROR",
            "登録処理に失敗しました。" +
            "管理者に連絡してください");

        VerifyErrorLog(
            expected,
            "顧客アカウント登録中に" +
            "内部エラーが発生しました。");
    }

    /// <summary>
    /// 登録処理で予期しない例外が発生した場合、
    /// InternalServerErrorを返すこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "Complete_予期しない例外発生時はInternalServerErrorを返す")]
    public async Task
        Complete_WhenUnexpectedExceptionOccurs_ReturnsInternalServerError()
    {
        // Arrange
        var model =
            CreateValidViewModel();

        var expected =
            new InvalidOperationException(
                "予期しないエラーです。");

        SetupRegisterThrows(
            expected);

        // Act
        var actual =
            await _controller.Complete(
                model);

        // Assert
        var result =
            AssertResult<ObjectResult>(
                actual);

        Assert.AreEqual(
            StatusCodes
                .Status500InternalServerError,
            result.StatusCode);

        AssertErrorResponse(
            result,
            "SYSTEM_ERROR",
            "登録処理に失敗しました。" +
            "管理者に連絡してください");

        VerifyErrorLog(
            expected,
            "顧客アカウント登録中に" +
            "予期しないエラーが発生しました。");
    }

    /// <summary>
    /// 重複が存在しないように設定する
    /// </summary>
    private void SetupNoDuplicates()
    {
        _usecaseMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    Username))
            .Returns(Task.CompletedTask);

        _usecaseMock
            .Setup(
                x => x.ExistsByMailAddressAsync(
                    MailAddress))
            .Returns(Task.CompletedTask);
    }

    /// <summary>
    /// 顧客登録時に指定した例外を
    /// スローするよう設定する
    /// </summary>
    private void SetupRegisterThrows(
        Exception exception)
    {
        _usecaseMock
            .Setup(
                x => x.RegisterCustomerAccountAsync(
                    It.IsAny<Customer>()))
            .ThrowsAsync(
                exception);
    }

    /// <summary>
    /// Confirmのユースケースが
    /// 呼ばれていないことを検証する
    /// </summary>
    private void
        VerifyConfirmUsecaseWasNotCalled()
    {
        _usecaseMock.Verify(
            x => x.ExistsByUsernameAsync(
                It.IsAny<string>()),
            Times.Never);

        _usecaseMock.Verify(
            x => x.ExistsByMailAddressAsync(
                It.IsAny<string>()),
            Times.Never);

        _usecaseMock.Verify(
            x => x.RegisterCustomerAccountAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// 正常なViewModelを生成する
    /// </summary>
    private static
        RegisterCustomerAccountViewModel
        CreateValidViewModel()
    {
        return new RegisterCustomerAccountViewModel
        {
            Name =
                "山田太郎",

            Kana =
                "ヤマダタロウ",

            Address1 =
                "東京都千代田区1-1",

            Address2 =
                "テストマンション101",

            PhoneNumber =
                "09012345678",

            MailAddress =
                MailAddress,

            Username =
                Username,

            Password =
                "password"
        };
    }

    /// <summary>
    /// IActionResultの具象型を検証して取得する
    /// </summary>
    private static TResult
        AssertResult<TResult>(
            IActionResult actual)
        where TResult : IActionResult
    {
        Assert.IsInstanceOfType(
            actual,
            typeof(TResult));

        return (TResult)actual;
    }

    /// <summary>
    /// 匿名オブジェクトのプロパティを取得する
    /// </summary>
    private static T
        GetProperty<T>(
            object? value,
            string propertyName)
    {
        if (value is null)
        {
            throw new AssertFailedException(
                "レスポンスのValueがnullです。");
        }

        var property =
            value.GetType()
                .GetProperty(
                    propertyName);

        if (property is null)
        {
            throw new AssertFailedException(
                $"{propertyName}が存在しません。");
        }

        var propertyValue =
            property.GetValue(
                value);

        if (propertyValue is null)
        {
            throw new AssertFailedException(
                $"{propertyName}がnullです。");
        }

        return (T)propertyValue;
    }

    /// <summary>
    /// codeとmessageを持つエラーレスポンスを
    /// 検証する
    /// </summary>
    private static void
        AssertErrorResponse(
            ObjectResult result,
            string expectedCode,
            string expectedMessage)
    {
        Assert.AreEqual(
            expectedCode,
            GetProperty<string>(
                result.Value,
                "code"));

        Assert.AreEqual(
            expectedMessage,
            GetProperty<string>(
                result.Value,
                "message"));
    }

    /// <summary>
    /// Errorレベルのログが出力されたことを
    /// 検証する
    /// </summary>
    private void VerifyErrorLog(
        Exception expectedException,
        string expectedMessage)
    {
        _loggerMock.Verify(
            logger =>
                logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>(
                        (
                            state,
                            _) =>
                            state.ToString() !=
                                null &&
                            state.ToString()!
                                .Contains(
                                    expectedMessage)),
                    It.Is<Exception>(
                        exception =>
                            ReferenceEquals(
                                exception,
                                expectedException)),
                    It.IsAny<
                        Func<
                            It.IsAnyType,
                            Exception?,
                            string>>()),
            Times.Once);
    }

    /// <summary>
    /// GetForm内のOk呼び出しを
    /// 例外化するテスト用Controller
    /// </summary>
    private sealed class
        ThrowingOkController
        : RegisterCustomerAccountController
    {
        private readonly Exception
            _exception;

        public ThrowingOkController(
            IRegisterCustomerAccountUsecase
                usecase,
            RegisterCustomerAccountViewModelAdapter
                adapter,
            ILogger<
                RegisterCustomerAccountController>
                logger,
            Exception exception)
            : base(
                usecase,
                adapter,
                logger)
        {
            _exception =
                exception;
        }

        public override
            OkObjectResult Ok(
                object? value)
        {
            throw _exception;
        }
    }
}