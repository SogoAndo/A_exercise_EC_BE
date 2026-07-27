using A_exercise_EC_BE.Domains.Exceptions;
using A_exercise_EC_BE.Domains.Models;
using A_exercise_EC_BE.Domains.Repositories;

namespace A_exercise_EC_BE.Applications.Usecases.Purchases;

/// <summary>
/// UC005 購入確定UseCase。
/// </summary>
public sealed class ConfirmPurchaseUsecase
    : IConfirmPurchaseUsecase
{
    private const string AvailablePaymentMethodName =
        "銀行振込";

    private const string InitialOrderStatusName =
        "受付";

    private readonly ICustomerRepository
        _customerRepository;

    private readonly IProductRepository
        _productRepository;

    private readonly IProductStockRepository
        _productStockRepository;

    private readonly IOrderRepository
        _orderRepository;

    private readonly IOrderDetailRepository
        _orderDetailRepository;

    private readonly IPaymentMethodRepository
        _paymentMethodRepository;

    private readonly IOrderStatusRepository
        _orderStatusRepository;

    private readonly IPurchaseAmountCalculator
        _purchaseAmountCalculator;

    private readonly IUnitOfWork
        _unitOfWork;

    /// <summary>
    /// コンストラクタ。
    /// </summary>
    public ConfirmPurchaseUsecase(
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IProductStockRepository productStockRepository,
        IOrderRepository orderRepository,
        IOrderDetailRepository orderDetailRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IOrderStatusRepository orderStatusRepository,
        IPurchaseAmountCalculator purchaseAmountCalculator,
        IUnitOfWork unitOfWork)
    {
        _customerRepository =
            customerRepository;

        _productRepository =
            productRepository;

        _productStockRepository =
            productStockRepository;

        _orderRepository =
            orderRepository;

        _orderDetailRepository =
            orderDetailRepository;

        _paymentMethodRepository =
            paymentMethodRepository;

        _orderStatusRepository =
            orderStatusRepository;

        _purchaseAmountCalculator =
            purchaseAmountCalculator;

        _unitOfWork =
            unitOfWork;
    }

    /// <inheritdoc />
    public async Task<ConfirmPurchaseResult>
        ConfirmAsync(
            ConfirmPurchaseRequest request)
    {
        ValidateRequest(request);

        var customer =
            await FindCustomerAsync(
                request.CustomerUuid);

        var paymentMethod =
            await FindPaymentMethodAsync(
                request.PaymentMethodId);

        var orderStatus =
            await FindInitialOrderStatusAsync();

        var orderDetails =
            await CreateOrderDetailsAsync(
                request.Items);

        var amountTotal =
            _purchaseAmountCalculator.Calculate(
                orderDetails);

        var orderDate =
            DateTime.Now;

        var order =
            new Orders(
                orderDate,
                amountTotal,
                customer,
                orderStatus,
                paymentMethod,
                orderDetails);

        await _unitOfWork.BeginAsync();

        var isCommitted = false;

        try
        {
            foreach (var orderDetail
                in orderDetails)
            {
                var isDecreased =
                    await _productStockRepository
                        .TryDecreaseAsync(
                            orderDetail.Product
                                .ProductUuid,
                            orderDetail.Count);

                if (!isDecreased)
                {
                    throw new DomainException(
                        "申し訳ありませんが、"
                        + $"商品「{orderDetail.Product.Name}」"
                        + "の在庫が不足しています");
                }
            }

            var orderId =
                await _orderRepository
                    .CreateAsync(order);

            await _orderDetailRepository
                .CreateRangeAsync(
                    orderId,
                    orderDetails);

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

        return new ConfirmPurchaseResult(
            order.OrderUuid,
            order.OrderDate,
            order.AmountTotal);
    }

    private static void ValidateRequest(
        ConfirmPurchaseRequest request)
    {
        _ = request
            ?? throw new DomainException(
                "購入情報を入力してください");

        if (request.CustomerUuid
            == Guid.Empty)
        {
            throw new DomainException(
                "顧客識別IDが不正です");
        }

        if (request.PaymentMethodId <= 0)
        {
            throw new DomainException(
                "支払い方法を選択してください");
        }

        if (request.Items is null
            || request.Items.Count == 0)
        {
            throw new DomainException(
                "カートに商品がありません");
        }

        if (request.Items.Any(
            item =>
                item.ProductUuid
                    == Guid.Empty))
        {
            throw new DomainException(
                "商品識別IDが不正です");
        }

        if (request.Items.Any(
            item =>
                item.Quantity <= 0))
        {
            throw new DomainException(
                "購入数量は1以上で入力してください。");
        }
    }

    private async Task<Customer>
        FindCustomerAsync(
            Guid customerUuid)
    {
        var customer =
            await _customerRepository
                .FindByCustomerUuidAsync(
                    customerUuid);

        return customer
            ?? throw new NotFoundException(
                "顧客アカウントが"
                + "見つかりません");
    }

    private async Task<PaymentMethod>
        FindPaymentMethodAsync(
            int paymentMethodId)
    {
        var paymentMethod =
            await _paymentMethodRepository
                .FindByIdAsync(
                    paymentMethodId);

        if (paymentMethod is null)
        {
            throw new NotFoundException(
                "支払い方法が"
                + "見つかりません");
        }

        if (paymentMethod.Name
            != AvailablePaymentMethodName)
        {
            throw new DomainException(
                "支払い方法は銀行振込のみ"
                + "選択できます");
        }

        return paymentMethod;
    }

    private async Task<OrderStatus>
        FindInitialOrderStatusAsync()
    {
        var orderStatus =
            await _orderStatusRepository
                .FindByNameAsync(
                    InitialOrderStatusName);

        return orderStatus
            ?? throw new InternalException(
                "初期注文ステータスが"
                + "登録されていません");
    }

    private async Task<List<OrdersDetail>>
        CreateOrderDetailsAsync(
            IReadOnlyCollection<
                ConfirmPurchaseItemRequest> items)
    {
        var orderDetails =
            new List<OrdersDetail>();

        foreach (var item in items)
        {
            var product =
                await _productRepository
                    .FindByIdAsync(
                        item.ProductUuid);

            if (product is null)
            {
                throw new NotFoundException(
                    $"商品ID:{item.ProductUuid}"
                    + "の商品が見つかりません");
            }

            orderDetails.Add(
                new OrdersDetail(
                    product,
                    item.Quantity));
        }

        return orderDetails;
    }
}
