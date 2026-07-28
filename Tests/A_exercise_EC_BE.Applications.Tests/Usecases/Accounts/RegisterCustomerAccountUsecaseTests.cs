using System.Reflection;
using System.Runtime.CompilerServices;
using A_exercise_EC_BE.Applications.Security;
using A_exercise_EC_BE.Applications.Usecases.Accounts;
using A_exercise_EC_BE.Applications.Usecases;
using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace A_exercise_EC_BE.Applications.Tests.Usecases.Accounts;

/// <summary>
/// RegisterCustomerAccountUsecaseの単体テスト
/// </summary>
[TestClass]
[TestCategory("Customers")]
public class RegisterCustomerAccountUsecaseTests
{
    private const string ValidUsername = "yamada";
    private const string ValidMailAddress =
        "yamada@example.com";
    private const string ValidRawPassword =
        "password";

    /*
     * 実際のPBKDF2ハッシュは20文字を超えるため、
     * 20文字を超える文字列を使用する。
     */
    private static readonly string HashedPassword =
        new('h', 64);

    private Mock<ICustomerRepository>
        _customerRepositoryMock = null!;

    private Mock<IPasswordHashingService>
        _passwordHashingServiceMock = null!;

    private Mock<IUnitOfWork>
        _unitOfWorkMock = null!;

    private RegisterCustomerAccountUsecase
        _usecase = null!;

    /// <summary>
    /// テストの前処理
    /// </summary>
    [TestInitialize]
    public void TestInitialize()
    {
        _customerRepositoryMock =
            new Mock<ICustomerRepository>();

        _passwordHashingServiceMock =
            new Mock<IPasswordHashingService>();

        _unitOfWorkMock =
            new Mock<IUnitOfWork>();

        _unitOfWorkMock
            .Setup(x => x.BeginAsync())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.CommitAsync())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(x => x.RollbackAsync())
            .Returns(Task.CompletedTask);

        _usecase =
            new RegisterCustomerAccountUsecase(
                _customerRepositoryMock.Object,
                _passwordHashingServiceMock.Object,
                _unitOfWorkMock.Object);
    }

    /// <summary>
    /// アカウント名が未入力の場合、
    /// DomainExceptionをスローすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExistsByUsernameAsync_アカウント名が未入力の場合はDomainExceptionをスローする")]
    public async Task
        ExistsByUsernameAsync_WhenUsernameIsWhiteSpace_ThrowsExactlyDomainException()
    {
        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<DomainException>(
                async () =>
                {
                    await _usecase
                        .ExistsByUsernameAsync(" ");
                });

        // Assert
        Assert.AreEqual(
            "アカウント名を入力してください",
            exception.Message);

        _customerRepositoryMock.Verify(
            x => x.ExistsByUsernameAsync(
                It.IsAny<string>()),
            Times.Never);
    }

    /// <summary>
    /// アカウント名が存在しない場合、
    /// 例外をスローしないこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExistsByUsernameAsync_アカウント名が存在しない場合は例外をスローしない")]
    public async Task
        ExistsByUsernameAsync_WhenUsernameDoesNotExist_DoesNotThrow()
    {
        // Arrange
        _customerRepositoryMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    ValidUsername))
            .ReturnsAsync(false);

        // Act
        await _usecase
            .ExistsByUsernameAsync(
                ValidUsername);

        // Assert
        _customerRepositoryMock.Verify(
            x => x.ExistsByUsernameAsync(
                ValidUsername),
            Times.Once);
    }

    /// <summary>
    /// アカウント名が既に存在する場合、
    /// ExistsExceptionをスローすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExistsByUsernameAsync_アカウント名が既に存在する場合はExistsExceptionをスローする")]
    public async Task
        ExistsByUsernameAsync_WhenUsernameExists_ThrowsExactlyExistsException()
    {
        // Arrange
        _customerRepositoryMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    ValidUsername))
            .ReturnsAsync(true);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<ExistsException>(
                async () =>
                {
                    await _usecase
                        .ExistsByUsernameAsync(
                            ValidUsername);
                });

        // Assert
        Assert.AreEqual(
            "このアカウント名は既に使用されています",
            exception.Message);

        _customerRepositoryMock.Verify(
            x => x.ExistsByUsernameAsync(
                ValidUsername),
            Times.Once);
    }

    /// <summary>
    /// メールアドレスが未入力の場合、
    /// DomainExceptionをスローすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExistsByMailAddressAsync_メールアドレスが未入力の場合はDomainExceptionをスローする")]
    public async Task
        ExistsByMailAddressAsync_WhenMailAddressIsWhiteSpace_ThrowsExactlyDomainException()
    {
        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<DomainException>(
                async () =>
                {
                    await _usecase
                        .ExistsByMailAddressAsync(" ");
                });

        // Assert
        Assert.AreEqual(
            "メールアドレスを入力してください",
            exception.Message);

        _customerRepositoryMock.Verify(
            x => x.ExistsByMailAddressAsync(
                It.IsAny<string>()),
            Times.Never);
    }

    /// <summary>
    /// メールアドレスが存在しない場合、
    /// 例外をスローしないこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExistsByMailAddressAsync_メールアドレスが存在しない場合は例外をスローしない")]
    public async Task
        ExistsByMailAddressAsync_WhenMailAddressDoesNotExist_DoesNotThrow()
    {
        // Arrange
        _customerRepositoryMock
            .Setup(
                x => x.ExistsByMailAddressAsync(
                    ValidMailAddress))
            .ReturnsAsync(false);

        // Act
        await _usecase
            .ExistsByMailAddressAsync(
                ValidMailAddress);

        // Assert
        _customerRepositoryMock.Verify(
            x => x.ExistsByMailAddressAsync(
                ValidMailAddress),
            Times.Once);
    }

    /// <summary>
    /// メールアドレスが既に存在する場合、
    /// ExistsExceptionをスローすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "ExistsByMailAddressAsync_メールアドレスが既に存在する場合はExistsExceptionをスローする")]
    public async Task
        ExistsByMailAddressAsync_WhenMailAddressExists_ThrowsExactlyExistsException()
    {
        // Arrange
        _customerRepositoryMock
            .Setup(
                x => x.ExistsByMailAddressAsync(
                    ValidMailAddress))
            .ReturnsAsync(true);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<ExistsException>(
                async () =>
                {
                    await _usecase
                        .ExistsByMailAddressAsync(
                            ValidMailAddress);
                });

        // Assert
        Assert.AreEqual(
            "このメールアドレスは既に使用されています",
            exception.Message);

        _customerRepositoryMock.Verify(
            x => x.ExistsByMailAddressAsync(
                ValidMailAddress),
            Times.Once);
    }

    /// <summary>
    /// 顧客アカウントを登録できること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_顧客アカウントを登録できる")]
    public async Task
        RegisterCustomerAccountAsync_CanRegisterCustomerAccount()
    {
        // Arrange
        var customerUuid =
            Guid.NewGuid();

        var createdAt =
            new DateTime(
                2026,
                7,
                24,
                9,
                0,
                0,
                DateTimeKind.Utc);

        var customer =
            CreateCustomer(
                customerUuid: customerUuid,
                name: "山田太郎",
                kana: "ヤマダタロウ",
                address1: "東京都千代田区1-1",
                address2: "テストマンション101",
                phoneNumber: "090-1234-5678",
                mailAddress: ValidMailAddress,
                username: ValidUsername,
                password: ValidRawPassword,
                createdAt: createdAt);

        _customerRepositoryMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    ValidUsername))
            .ReturnsAsync(false);

        _customerRepositoryMock
            .Setup(
                x => x.ExistsByMailAddressAsync(
                    ValidMailAddress))
            .ReturnsAsync(false);

        _passwordHashingServiceMock
            .Setup(
                x => x.Hash(
                    ValidRawPassword))
            .Returns(HashedPassword);

        Customer? createdCustomer = null;

        _customerRepositoryMock
            .Setup(
                x => x.CreateAsync(
                    It.IsAny<Customer>()))
            .Callback<Customer>(
                value =>
                    createdCustomer = value)
            .Returns(Task.CompletedTask);

        // Act
        await _usecase
            .RegisterCustomerAccountAsync(
                customer);

        // Assert
        Assert.IsNotNull(createdCustomer);

        Assert.AreEqual(
            customerUuid,
            createdCustomer.CustomerUuid);

        Assert.AreEqual(
            "山田太郎",
            createdCustomer.Name);

        Assert.AreEqual(
            "ヤマダタロウ",
            createdCustomer.Kana);

        Assert.AreEqual(
            "東京都千代田区1-1",
            createdCustomer.Address1);

        Assert.AreEqual(
            "テストマンション101",
            createdCustomer.Address2);

        Assert.AreEqual(
            "090-1234-5678",
            createdCustomer.PhoneNumber);

        Assert.AreEqual(
            ValidMailAddress,
            createdCustomer.MailAddress);

        Assert.AreEqual(
            ValidUsername,
            createdCustomer.Username);

        Assert.AreEqual(
            HashedPassword,
            createdCustomer.Password);

        Assert.AreEqual(
            createdAt,
            createdCustomer.CreatedAt);

        /*
         * 登録元オブジェクトのパスワードが
         * 上書きされていないことも確認する。
         */
        Assert.AreEqual(
            ValidRawPassword,
            customer.Password);

        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Never);

        _customerRepositoryMock.Verify(
            x => x.ExistsByUsernameAsync(
                ValidUsername),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.ExistsByMailAddressAsync(
                ValidMailAddress),
            Times.Once);

        _passwordHashingServiceMock.Verify(
            x => x.Hash(
                ValidRawPassword),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.CreateAsync(
                It.Is<Customer>(
                    value =>
                        value.CustomerUuid ==
                            customerUuid &&
                        value.Username ==
                            ValidUsername &&
                        value.MailAddress ==
                            ValidMailAddress &&
                        value.Password ==
                            HashedPassword)),
            Times.Once);
    }

    /// <summary>
    /// 引数customerがnullの場合、
    /// InternalExceptionをスローすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_customerがnullの場合はInternalExceptionをスローする")]
    public async Task
        RegisterCustomerAccountAsync_WhenCustomerIsNull_ThrowsExactlyInternalException()
    {
        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<InternalException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            null!);
                });

        // Assert
        Assert.AreEqual(
            "引数customerがnullです。",
            exception.Message);

        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Never);

        _customerRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// パスワードが未入力の場合、
    /// DomainExceptionをスローすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_パスワードが未入力の場合はDomainExceptionをスローする")]
    public async Task
        RegisterCustomerAccountAsync_WhenPasswordIsWhiteSpace_ThrowsExactlyDomainException()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer(
                password: " ");

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<DomainException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            customer);
                });

        // Assert
        Assert.AreEqual(
            "パスワードを入力してください",
            exception.Message);

        VerifyTransactionWasNotStarted();
        VerifyRegistrationWasNotExecuted();
    }

    /// <summary>
    /// パスワードが5文字未満の場合、
    /// DomainExceptionをスローすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_パスワードが5文字未満の場合はDomainExceptionをスローする")]
    public async Task
        RegisterCustomerAccountAsync_WhenPasswordIsTooShort_ThrowsExactlyDomainException()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer(
                password: "1234");

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<DomainException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            customer);
                });

        // Assert
        Assert.AreEqual(
            "パスワードは5文字以上20文字以内で入力してください",
            exception.Message);

        VerifyTransactionWasNotStarted();
        VerifyRegistrationWasNotExecuted();
    }

    /// <summary>
    /// パスワードが20文字を超える場合、
    /// DomainExceptionをスローすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_パスワードが20文字を超える場合はDomainExceptionをスローする")]
    public async Task
        RegisterCustomerAccountAsync_WhenPasswordIsTooLong_ThrowsExactlyDomainException()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer(
                password: new string('a', 21));

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<DomainException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            customer);
                });

        // Assert
        Assert.AreEqual(
            "パスワードは5文字以上20文字以内で入力してください",
            exception.Message);

        VerifyTransactionWasNotStarted();
        VerifyRegistrationWasNotExecuted();
    }

    /// <summary>
    /// アカウント名が既に存在する場合、
    /// ExistsExceptionをスローして
    /// ロールバックすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_アカウント名が既に存在する場合はExistsExceptionをスローしてロールバックする")]
    public async Task
        RegisterCustomerAccountAsync_WhenUsernameAlreadyExists_ThrowsExactlyExistsExceptionAndRollback()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer();

        _customerRepositoryMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    ValidUsername))
            .ReturnsAsync(true);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<ExistsException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            customer);
                });

        // Assert
        Assert.AreEqual(
            "このアカウント名は既に使用されています",
            exception.Message);

        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.ExistsByUsernameAsync(
                ValidUsername),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.ExistsByMailAddressAsync(
                It.IsAny<string>()),
            Times.Never);

        _passwordHashingServiceMock.Verify(
            x => x.Hash(
                It.IsAny<string>()),
            Times.Never);

        _customerRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// メールアドレスが既に存在する場合、
    /// ExistsExceptionをスローして
    /// ロールバックすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_メールアドレスが既に存在する場合はExistsExceptionをスローしてロールバックする")]
    public async Task
        RegisterCustomerAccountAsync_WhenMailAddressAlreadyExists_ThrowsExactlyExistsExceptionAndRollback()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer();

        _customerRepositoryMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    ValidUsername))
            .ReturnsAsync(false);

        _customerRepositoryMock
            .Setup(
                x => x.ExistsByMailAddressAsync(
                    ValidMailAddress))
            .ReturnsAsync(true);

        // Act
        var exception =
            await Assert.ThrowsExactlyAsync<ExistsException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            customer);
                });

        // Assert
        Assert.AreEqual(
            "このメールアドレスは既に使用されています",
            exception.Message);

        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.ExistsByUsernameAsync(
                ValidUsername),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.ExistsByMailAddressAsync(
                ValidMailAddress),
            Times.Once);

        _passwordHashingServiceMock.Verify(
            x => x.Hash(
                It.IsAny<string>()),
            Times.Never);

        _customerRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// パスワードハッシュ化で例外が発生した場合、
    /// 例外を再スローしてロールバックすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_パスワードハッシュ化で例外が発生した場合は再スローしてロールバックする")]
    public async Task
        RegisterCustomerAccountAsync_WhenHashThrowsException_ThrowsExactlyExceptionAndRollback()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer();

        var expected =
            new InvalidOperationException(
                "ハッシュ化に失敗しました。");

        SetupNoDuplicates();

        _passwordHashingServiceMock
            .Setup(
                x => x.Hash(
                    ValidRawPassword))
            .Throws(expected);

        // Act
        var actual =
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            customer);
                });

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Once);

        _passwordHashingServiceMock.Verify(
            x => x.Hash(
                ValidRawPassword),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// 顧客ドメインオブジェクト生成時に
    /// 例外が発生した場合、
    /// ロールバックすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_ハッシュ値が不正でCustomer生成に失敗した場合はロールバックする")]
    public async Task
        RegisterCustomerAccountAsync_WhenCustomerCreationThrowsException_RollsBack()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer();

        SetupNoDuplicates();

        /*
         * Customerが空文字のパスワードを
         * 許可しない実装であることを前提とする。
         */
        _passwordHashingServiceMock
            .Setup(
                x => x.Hash(
                    ValidRawPassword))
            .Returns(string.Empty);

        // Act
        await Assert.ThrowsAsync<Exception>(
            async () =>
            {
                await _usecase
                    .RegisterCustomerAccountAsync(
                        customer);
            });

        // Assert
        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// 顧客登録で例外が発生した場合、
    /// 例外を再スローしてロールバックすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_顧客登録で例外が発生した場合は再スローしてロールバックする")]
    public async Task
        RegisterCustomerAccountAsync_WhenCreateThrowsException_ThrowsExactlyExceptionAndRollback()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer();

        var expected =
            new InvalidOperationException(
                "顧客登録に失敗しました。");

        SetupNoDuplicates();

        _passwordHashingServiceMock
            .Setup(
                x => x.Hash(
                    ValidRawPassword))
            .Returns(HashedPassword);

        _customerRepositoryMock
            .Setup(
                x => x.CreateAsync(
                    It.IsAny<Customer>()))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            customer);
                });

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Customer>()),
            Times.Once);
    }

    /// <summary>
    /// コミットで例外が発生した場合、
    /// 例外を再スローしてロールバックすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_コミットで例外が発生した場合は再スローしてロールバックする")]
    public async Task
        RegisterCustomerAccountAsync_WhenCommitThrowsException_ThrowsExactlyExceptionAndRollback()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer();

        var expected =
            new InvalidOperationException(
                "コミットに失敗しました。");

        SetupNoDuplicates();

        _passwordHashingServiceMock
            .Setup(
                x => x.Hash(
                    ValidRawPassword))
            .Returns(HashedPassword);

        _customerRepositoryMock
            .Setup(
                x => x.CreateAsync(
                    It.IsAny<Customer>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock
            .Setup(
                x => x.CommitAsync())
            .ThrowsAsync(expected);

        // Act
        var actual =
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            customer);
                });

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Customer>()),
            Times.Once);
    }

    /// <summary>
    /// トランザクション開始で例外が発生した場合、
    /// ロールバックを実行しないこと
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_トランザクション開始で例外が発生した場合はロールバックしない")]
    public async Task
        RegisterCustomerAccountAsync_WhenBeginThrowsException_DoesNotRollback()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer();

        var expected =
            new InvalidOperationException(
                "トランザクションを開始できませんでした。");

        _unitOfWorkMock
            .Setup(
                x => x.BeginAsync())
            .ThrowsAsync(expected);

        // Act
        var actual =
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            customer);
                });

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Never);

        _customerRepositoryMock.Verify(
            x => x.ExistsByUsernameAsync(
                It.IsAny<string>()),
            Times.Never);

        _passwordHashingServiceMock.Verify(
            x => x.Hash(
                It.IsAny<string>()),
            Times.Never);

        _customerRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// アカウント名の存在確認で例外が発生した場合、
    /// ロールバックすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_アカウント名確認で例外が発生した場合は再スローしてロールバックする")]
    public async Task
        RegisterCustomerAccountAsync_WhenUsernameCheckThrowsException_ThrowsExactlyExceptionAndRollback()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer();

        var expected =
            new InvalidOperationException(
                "アカウント名の確認に失敗しました。");

        _customerRepositoryMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    ValidUsername))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            customer);
                });

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Once);

        _customerRepositoryMock.Verify(
            x => x.ExistsByMailAddressAsync(
                It.IsAny<string>()),
            Times.Never);

        _passwordHashingServiceMock.Verify(
            x => x.Hash(
                It.IsAny<string>()),
            Times.Never);
    }

    /// <summary>
    /// メールアドレスの存在確認で例外が発生した場合、
    /// ロールバックすること
    /// </summary>
    [TestMethod(
        DisplayName =
            "RegisterCustomerAccountAsync_メールアドレス確認で例外が発生した場合は再スローしてロールバックする")]
    public async Task
        RegisterCustomerAccountAsync_WhenMailAddressCheckThrowsException_ThrowsExactlyExceptionAndRollback()
    {
        // Arrange
        var customer =
            CreateDefaultCustomer();

        var expected =
            new InvalidOperationException(
                "メールアドレスの確認に失敗しました。");

        _customerRepositoryMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    ValidUsername))
            .ReturnsAsync(false);

        _customerRepositoryMock
            .Setup(
                x => x.ExistsByMailAddressAsync(
                    ValidMailAddress))
            .ThrowsAsync(expected);

        // Act
        var actual =
            await Assert.ThrowsExactlyAsync<InvalidOperationException>(
                async () =>
                {
                    await _usecase
                        .RegisterCustomerAccountAsync(
                            customer);
                });

        // Assert
        Assert.AreSame(
            expected,
            actual);

        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Once);

        _passwordHashingServiceMock.Verify(
            x => x.Hash(
                It.IsAny<string>()),
            Times.Never);

        _customerRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// アカウント名とメールアドレスが
    /// 未登録になるよう設定する
    /// </summary>
    private void SetupNoDuplicates()
    {
        _customerRepositoryMock
            .Setup(
                x => x.ExistsByUsernameAsync(
                    ValidUsername))
            .ReturnsAsync(false);

        _customerRepositoryMock
            .Setup(
                x => x.ExistsByMailAddressAsync(
                    ValidMailAddress))
            .ReturnsAsync(false);
    }

    /// <summary>
    /// トランザクションが開始されていないことを
    /// 検証する
    /// </summary>
    private void VerifyTransactionWasNotStarted()
    {
        _unitOfWorkMock.Verify(
            x => x.BeginAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.CommitAsync(),
            Times.Never);

        _unitOfWorkMock.Verify(
            x => x.RollbackAsync(),
            Times.Never);
    }

    /// <summary>
    /// 重複確認、ハッシュ化、登録が
    /// 実行されていないことを検証する
    /// </summary>
    private void VerifyRegistrationWasNotExecuted()
    {
        _customerRepositoryMock.Verify(
            x => x.ExistsByUsernameAsync(
                It.IsAny<string>()),
            Times.Never);

        _customerRepositoryMock.Verify(
            x => x.ExistsByMailAddressAsync(
                It.IsAny<string>()),
            Times.Never);

        _passwordHashingServiceMock.Verify(
            x => x.Hash(
                It.IsAny<string>()),
            Times.Never);

        _customerRepositoryMock.Verify(
            x => x.CreateAsync(
                It.IsAny<Customer>()),
            Times.Never);
    }

    /// <summary>
    /// 標準的なテスト用Customerを生成する
    /// </summary>
    private static Customer CreateDefaultCustomer(
        string password = ValidRawPassword)
    {
        return CreateCustomer(
            customerUuid: Guid.NewGuid(),
            name: "山田太郎",
            kana: "ヤマダタロウ",
            address1: "東京都千代田区1-1",
            address2: "テストマンション101",
            phoneNumber: "090-1234-5678",
            mailAddress: ValidMailAddress,
            username: ValidUsername,
            password: password,
            createdAt: DateTime.UtcNow);
    }

    /// <summary>
    /// テスト用Customerを生成する
    /// </summary>
    private static Customer CreateCustomer(
        Guid customerUuid,
        string name,
        string kana,
        string address1,
        string address2,
        string phoneNumber,
        string mailAddress,
        string username,
        string password,
        DateTime createdAt)
    {
        /*
         * 不正な平文パスワードを持つCustomerも
         * ユースケースへ渡す必要があるため、
         * コンストラクタを経由せず生成する。
         */
        var customer =
            (Customer)RuntimeHelpers
                .GetUninitializedObject(
                    typeof(Customer));

        SetPrivateProperty(
            customer,
            "CustomerUuid",
            customerUuid);

        SetPrivateProperty(
            customer,
            "Name",
            name);

        SetPrivateProperty(
            customer,
            "Kana",
            kana);

        SetPrivateProperty(
            customer,
            "Address1",
            address1);

        SetPrivateProperty(
            customer,
            "Address2",
            address2);

        SetPrivateProperty(
            customer,
            "PhoneNumber",
            phoneNumber);

        SetPrivateProperty(
            customer,
            "MailAddress",
            mailAddress);

        SetPrivateProperty(
            customer,
            "Username",
            username);

        SetPrivateProperty(
            customer,
            "Password",
            password);

        SetPrivateProperty(
            customer,
            "CreatedAt",
            createdAt);

        return customer;
    }

    /// <summary>
    /// private setのプロパティへ
    /// テスト用の値を設定する
    /// </summary>
    private static void SetPrivateProperty<T>(
        T target,
        string propertyName,
        object? value)
    {
        var field =
            typeof(T).GetField(
                $"<{propertyName}>k__BackingField",
                BindingFlags.Instance |
                BindingFlags.NonPublic);

        if (field is null)
        {
            throw new InvalidOperationException(
                $"{propertyName}のバッキングフィールドが見つかりません。");
        }

        field.SetValue(
            target,
            value);
    }
}