using AWSomeShop.API.Infrastructure.DbContext;
using AWSomeShop.API.Models.DTOs;
using AWSomeShop.API.Models.Entities;
using AWSomeShop.API.Repositories.Interfaces;
using AWSomeShop.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AWSomeShop.API.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IPointsService _pointsService;
    private readonly INotificationService _notificationService;
    private readonly AWSomeShopDbContext _context;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IAddressRepository addressRepository,
        IPointsService pointsService,
        INotificationService notificationService,
        AWSomeShopDbContext context,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _addressRepository = addressRepository;
        _pointsService = pointsService;
        _notificationService = notificationService;
        _context = context;
        _logger = logger;
    }

    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto, string employeeId)
    {
        // 验证验证码（简化实现，实际应该验证session中的验证码）
        if (string.IsNullOrEmpty(dto.Captcha))
        {
            throw new ArgumentException("验证码不能为空");
        }

        // 验证地址
        var address = await _addressRepository.GetByIdAsync(dto.AddressId);
        if (address == null || address.EmployeeId != employeeId)
        {
            throw new ArgumentException("收货地址无效");
        }

        // 验证每月兑换次数限制（每月最多5次）
        var now = DateTime.UtcNow;
        var monthlyCount = await _orderRepository.GetMonthlyOrderCountAsync(employeeId, now.Year, now.Month);
        if (monthlyCount >= 5)
        {
            throw new InvalidOperationException("本月兑换次数已达上限（5次）");
        }

        // 开始事务
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var orderItems = new List<OrderItem>();
            int totalPoints = 0;

            // 验证产品并计算总积分
            foreach (var item in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"产品 {item.ProductId} 不存在");
                }

                if (product.Status != ProductStatus.上架)
                {
                    throw new ArgumentException($"产品 {product.Name} 已下架");
                }

                if (product.Stock < item.Quantity)
                {
                    throw new ArgumentException($"产品 {product.Name} 库存不足");
                }

                // 验证重复兑换限制（同一产品每人限兑1次）
                var hasOrdered = await _orderRepository.HasOrderedProductAsync(employeeId, item.ProductId);
                if (hasOrdered)
                {
                    throw new InvalidOperationException($"产品 {product.Name} 已兑换过，不能重复兑换");
                }

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ProductImage = product.Images.FirstOrDefault()?.ImageUrl,
                    Points = product.Points,
                    Quantity = item.Quantity
                });

                totalPoints += product.Points * item.Quantity;
            }

            // 验证积分余额
            var balance = await _pointsService.GetBalanceAsync(employeeId);
            if (balance < totalPoints)
            {
                throw new InvalidOperationException($"积分余额不足。需要 {totalPoints} 积分，当前余额 {balance} 积分");
            }

            // 扣除积分
            await _pointsService.DeductPointsAsync(employeeId, totalPoints, "兑换商品");

            // 减少库存（带并发控制）
            foreach (var item in dto.Items)
            {
                await DeductStockWithRetryAsync(item.ProductId, item.Quantity);
            }

            // 创建订单
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                EmployeeId = employeeId,
                TotalPoints = totalPoints,
                Status = OrderStatus.待确认,
                AddressId = dto.AddressId,
                Items = orderItems,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdOrder = await _orderRepository.CreateAsync(order);

            // 提交事务
            await transaction.CommitAsync();

            _logger.LogInformation("Order created: {OrderId} for employee {EmployeeId}, total points: {TotalPoints}",
                createdOrder.Id, employeeId, totalPoints);

            // 创建通知
            await _notificationService.CreateNotificationAsync(
                employeeId,
                "订单创建成功",
                $"您的订单 {order.OrderNumber} 已创建成功，消耗 {totalPoints} 积分",
                NotificationType.订单状态,
                createdOrder.Id);

            // 重新加载订单以获取完整信息
            var fullOrder = await _orderRepository.GetByIdAsync(createdOrder.Id, includeItems: true, includeAddress: true);
            return MapToDto(fullOrder!);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to create order for employee {EmployeeId}", employeeId);
            throw;
        }
    }

    public async Task<OrderDto?> GetOrderByIdAsync(string orderId, string employeeId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, includeItems: true, includeAddress: true);
        if (order == null || order.EmployeeId != employeeId)
        {
            return null;
        }
        return MapToDto(order);
    }

    public async Task<(List<OrderDto> Orders, int TotalCount)> GetOrdersByEmployeeIdAsync(
        string employeeId,
        int pageNumber = 1,
        int pageSize = 10,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var (orders, totalCount) = await _orderRepository.GetByEmployeeIdAsync(
            employeeId, pageNumber, pageSize, status, startDate, endDate);

        return (orders.Select(MapToDto).ToList(), totalCount);
    }

    public async Task<OrderDto> CancelOrderAsync(string orderId, string employeeId, string? reason = null)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, includeItems: true, includeAddress: true);
        if (order == null || order.EmployeeId != employeeId)
        {
            throw new InvalidOperationException("订单不存在");
        }

        // 验证取消条件：仅在"待发货"状态可取消，且24小时内
        if (order.Status != OrderStatus.待发货)
        {
            throw new InvalidOperationException("只有待发货状态的订单可以取消");
        }

        var hoursSinceCreated = (DateTime.UtcNow - order.CreatedAt).TotalHours;
        if (hoursSinceCreated > 24)
        {
            throw new InvalidOperationException("订单创建超过24小时，无法取消");
        }

        // 开始事务
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // 退回积分
            await _pointsService.GrantPointsAsync(
                employeeId,
                order.TotalPoints,
                $"取消订单 {order.OrderNumber}",
                "System");

            // 恢复库存（带并发控制）
            foreach (var item in order.Items)
            {
                await RestoreStockWithRetryAsync(item.ProductId, item.Quantity);
            }

            // 更新订单状态
            order.Status = OrderStatus.已取消;
            order.CancelReason = reason ?? "用户取消";
            var updatedOrder = await _orderRepository.UpdateAsync(order);

            await transaction.CommitAsync();

            _logger.LogInformation("Order cancelled: {OrderId}, points refunded: {TotalPoints}",
                orderId, order.TotalPoints);

            // 创建通知
            await _notificationService.CreateNotificationAsync(
                employeeId,
                "订单已取消",
                $"您的订单 {order.OrderNumber} 已取消，{order.TotalPoints} 积分已退回",
                NotificationType.订单状态,
                orderId);

            return MapToDto(updatedOrder);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Failed to cancel order {OrderId}", orderId);
            throw;
        }
    }

    public async Task<(List<OrderDto> Orders, int TotalCount)> GetAllOrdersAsync(
        int pageNumber = 1,
        int pageSize = 10,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? employeeId = null)
    {
        var (orders, totalCount) = await _orderRepository.GetAllAsync(
            pageNumber, pageSize, status, startDate, endDate, employeeId);

        return (orders.Select(MapToDto).ToList(), totalCount);
    }

    public async Task<OrderDto> UpdateOrderStatusAsync(string orderId, OrderStatus newStatus, string? trackingNumber = null)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, includeItems: true, includeAddress: true);
        if (order == null)
        {
            throw new InvalidOperationException("订单不存在");
        }

        // 验证状态转换合法性
        ValidateStatusTransition(order.Status, newStatus);

        order.Status = newStatus;
        if (!string.IsNullOrEmpty(trackingNumber))
        {
            order.TrackingNumber = trackingNumber;
        }

        var updatedOrder = await _orderRepository.UpdateAsync(order);

        _logger.LogInformation("Order status updated: {OrderId}, from {OldStatus} to {NewStatus}",
            orderId, order.Status, newStatus);

        // 创建通知
        string notificationContent = newStatus switch
        {
            OrderStatus.待发货 => $"您的订单 {order.OrderNumber} 已确认，等待发货",
            OrderStatus.已发货 => $"您的订单 {order.OrderNumber} 已发货" + (string.IsNullOrEmpty(trackingNumber) ? "" : $"，物流单号：{trackingNumber}"),
            OrderStatus.已完成 => $"您的订单 {order.OrderNumber} 已完成",
            _ => $"您的订单 {order.OrderNumber} 状态已更新为 {newStatus}"
        };

        await _notificationService.CreateNotificationAsync(
            order.EmployeeId,
            "订单状态更新",
            notificationContent,
            NotificationType.订单状态,
            orderId);

        return MapToDto(updatedOrder);
    }

    private void ValidateStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        // 已取消和已完成状态不能转换
        if (currentStatus == OrderStatus.已取消 || currentStatus == OrderStatus.已完成)
        {
            throw new InvalidOperationException($"订单状态为 {currentStatus}，无法修改");
        }

        // 定义合法的状态转换
        var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
        {
            { OrderStatus.待确认, new List<OrderStatus> { OrderStatus.待发货, OrderStatus.已取消 } },
            { OrderStatus.待发货, new List<OrderStatus> { OrderStatus.已发货, OrderStatus.已取消 } },
            { OrderStatus.已发货, new List<OrderStatus> { OrderStatus.已完成 } }
        };

        if (!validTransitions.ContainsKey(currentStatus) || !validTransitions[currentStatus].Contains(newStatus))
        {
            throw new InvalidOperationException($"不能从 {currentStatus} 转换到 {newStatus}");
        }
    }

    private string GenerateOrderNumber()
    {
        // 格式: ORD + 年月日 + 6位随机数
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = new Random().Next(100000, 999999);
        return $"ORD{datePart}{randomPart}";
    }

    /// <summary>
    /// 带重试的库存扣减（乐观锁并发控制）
    /// </summary>
    private async Task DeductStockWithRetryAsync(string productId, int quantity, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    throw new InvalidOperationException($"产品 {productId} 不存在");
                }

                if (product.Stock < quantity)
                {
                    throw new InvalidOperationException($"产品 {product.Name} 库存不足");
                }

                product.Stock -= quantity;
                await _productRepository.UpdateAsync(product);
                return; // 成功，退出
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict when deducting stock for product {ProductId}, retry {Retry}/{MaxRetries}",
                    productId, i + 1, maxRetries);

                if (i == maxRetries - 1)
                {
                    throw new InvalidOperationException("库存更新失败，请稍后重试", ex);
                }

                // 等待一小段时间后重试
                await Task.Delay(100 * (i + 1));
            }
        }
    }

    /// <summary>
    /// 带重试的库存恢复（乐观锁并发控制）
    /// </summary>
    private async Task RestoreStockWithRetryAsync(string productId, int quantity, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    _logger.LogWarning("Product {ProductId} not found when restoring stock", productId);
                    return;
                }

                product.Stock += quantity;
                await _productRepository.UpdateAsync(product);
                return; // 成功，退出
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency conflict when restoring stock for product {ProductId}, retry {Retry}/{MaxRetries}",
                    productId, i + 1, maxRetries);

                if (i == maxRetries - 1)
                {
                    throw new InvalidOperationException("库存恢复失败，请稍后重试", ex);
                }

                // 等待一小段时间后重试
                await Task.Delay(100 * (i + 1));
            }
        }
    }

    private OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Items = order.Items.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                ProductImage = item.ProductImage,
                Points = item.Points,
                Quantity = item.Quantity
            }).ToList(),
            TotalPoints = order.TotalPoints,
            Status = order.Status.ToString(),
            TrackingNumber = order.TrackingNumber,
            Address = order.Address != null ? new AddressDto
            {
                Id = order.Address.Id,
                ReceiverName = order.Address.ReceiverName,
                ReceiverPhone = order.Address.ReceiverPhone,
                Province = order.Address.Province,
                City = order.Address.City,
                District = order.Address.District,
                DetailAddress = order.Address.DetailAddress,
                IsDefault = order.Address.IsDefault
            } : null,
            CreatedAt = order.CreatedAt
        };
    }
}
