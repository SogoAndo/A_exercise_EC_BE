using A_exercise_EC_BE.Application.Security;
using A_exercise_EC_BE.Domain.Exceptions;
using A_exercise_EC_BE.Domain.Models;
using A_exercise_EC_BE.Domain.Repositories;

namespace A_exercise_EC_BE.Application.Usecases.Customers;

/// <summary>
/// 顧客アカウント登録ユースケース
/// </summary>
public class RegisterCustomerAccountUsecase
    : IRegisterCustomerAccountUsecase
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="customerRepository">
    /// 顧客リポジトリ
    /// </param>
    /// <param name="passwordHashingService">
    /// パスワードハッシュ化サービス
    /// </param>
    /// <param name="unitOfWork">
    /// トランザクション制御機能
    /// </param>
    public RegisterCustomerAccountUsecase(
        ICustomerRepository customerRepository,
        IPasswordHashingService passwordHashingService,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _passwordHashingService = passwordHashingService;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// アカウント名が既に存在するかを検証する
    /// </summary>
    /// <param name="username">アカウント名</param>
    /// <returns>なし</returns>
    public async Task ExistsByUsernameAsync(
        string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new DomainException(
                "アカウント名を入力してください");
        }

        var exists =
            await _customerRepository
                .ExistsByUsernameAsync(username);

        if (exists)
        {
            throw new ExistsException(
                "このアカウント名は既に使用されています");
        }
    }

    /// <summary>
    /// メールアドレスが既に存在するかを検証する
    /// </summary>
    /// <param name="mailAddress">
    /// メールアドレス
    /// </param>
    /// <returns>なし</returns>
    public async Task ExistsByMailAddressAsync(
        string mailAddress)
    {
        if (string.IsNullOrWhiteSpace(mailAddress))
        {
            throw new DomainException(
                "メールアドレスを入力してください");
        }

        var exists =
            await _customerRepository
                .ExistsByMailAddressAsync(mailAddress);

        if (exists)
        {
            throw new ExistsException(
                "このメールアドレスは既に使用されています");
        }
    }

    /// <summary>
    /// 顧客アカウントを登録する
    /// </summary>
    /// <param name="customer">
    /// 登録対象の顧客
    /// </param>
    /// <returns>なし</returns>
    public async Task RegisterCustomerAccountAsync(
        Customer customer)
    {
        _ = customer
            ?? throw new InternalException(
                "引数customerがnullです。");

        // 登録前の平文パスワードを検証する
        ValidateRawPassword(customer.Password);

        await _unitOfWork.BeginAsync();
        var isCommitted = false;

        try
        {
            // アカウント名の重複を確認する
            await ExistsByUsernameAsync(
                customer.Username);

            // メールアドレスの重複を確認する
            await ExistsByMailAddressAsync(
                customer.MailAddress);

            // パスワードをハッシュ化する
            var hashedPassword =
                _passwordHashingService.Hash(
                    customer.Password);

            // ハッシュ化済みパスワードを持つ
            // 顧客ドメインオブジェクトを作成する
            var registeredCustomer =
                new Customer(
                    customer.CustomerUuid,
                    customer.Name,
                    customer.Kana,
                    customer.Address1,
                    customer.Address2,
                    customer.PhoneNumber,
                    customer.MailAddress,
                    customer.Username,
                    hashedPassword,
                    customer.CreatedAt
                );

            // 顧客アカウントを永続化する
            await _customerRepository
                .CreateAsync(registeredCustomer);

            await _unitOfWork.CommitAsync();
            isCommitted = true;
        }
        finally
        {
            if (!isCommitted)
            {
                await _unitOfWork.RollbackAsync();
            }
        }
    }

    /// <summary>
    /// 登録画面から入力された平文パスワードを検証する
    /// </summary>
    /// <param name="password">
    /// 平文パスワード
    /// </param>
    private static void ValidateRawPassword(
        string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new DomainException(
                "パスワードを入力してください");
        }

        if (password.Length < 5 ||
            password.Length > 20)
        {
            throw new DomainException(
                "パスワードは5文字以上20文字以内で入力してください");
        }
    }
}