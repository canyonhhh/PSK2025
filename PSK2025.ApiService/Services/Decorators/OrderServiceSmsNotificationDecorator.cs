using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;

namespace PSK2025.ApiService.Services.Decorators
{
    public class OrderServiceSmsNotificationDecorator(
        IOrderService decoratedOrderService,
        ISmsService smsService,
        IUserService userService,
        ILogger<OrderServiceSmsNotificationDecorator> logger) : IOrderService
    {
        private readonly IOrderService decoratedOrderService = decoratedOrderService;
        private readonly ISmsService smsService = smsService;
        private readonly IUserService userService = userService;
        private readonly ILogger<OrderServiceSmsNotificationDecorator> logger = logger;

        public async Task<PaginatedResult<OrderDto>> GetOrdersAsync(
            string? userId = null,
            OrderStatus? status = null,
            OrderSortBy sortBy = OrderSortBy.CreatedAt,
            bool ascending = false,
            int page = 1,
            int pageSize = 10)
        {
            return await decoratedOrderService.GetOrdersAsync(userId, status, sortBy, ascending, page, pageSize);
        }

        public async Task<(OrderDto? Order, ServiceError Error)> GetOrderByIdAsync(string id)
        {
            return await decoratedOrderService.GetOrderByIdAsync(id);
        }

        public async Task<(OrderDto? Order, ServiceError Error)> CreateOrderAsync(string userId, CreateOrderDto model)
        {
            return await decoratedOrderService.CreateOrderAsync(userId, model);
        }

        public async Task<(OrderDto? Order, ServiceError Error)> UpdateOrderStatusAsync(string id, UpdateOrderStatusDto model)
        {
            var (Order, _) = await decoratedOrderService.GetOrderByIdAsync(id);
            var previousStatus = Order?.Status;

            var result = await decoratedOrderService.UpdateOrderStatusAsync(id, model);

            if (result.Error == ServiceError.None &&
                result.Order != null &&
                previousStatus != OrderStatus.Completed &&
                model.Status == OrderStatus.Completed)
            {
                await SendOrderCompletedSmsAsync(result.Order);
            }

            return result;
        }

        public async Task<ServiceError> DeleteOrderAsync(string id)
        {
            return await decoratedOrderService.DeleteOrderAsync(id);
        }

        private async Task SendOrderCompletedSmsAsync(OrderDto order)
        {
            try
            {
                var (user, error) = await userService.GetUserByIdAsync(order.UserId);

                if (error != ServiceError.None || user == null)
                {
                    logger.LogWarning("Cannot send SMS notification: User not found for order {OrderId}. Error: {Error}",
                        order.Id, error);
                    return;
                }

                string phoneNumber = user.PhoneNumber;

                if (string.IsNullOrEmpty(phoneNumber))
                {
                    logger.LogWarning("Cannot send SMS notification: No phone number available for user {UserId}", user.Id);
                    return;
                }

                string message = $"Good news! Your order #{order.Id} is ready for pickup. Thank you for your business!";

                bool sent = await smsService.SendSmsAsync(phoneNumber, message);

                if (sent)
                {
                    logger.LogInformation("SMS notification sent to {PhoneNumber} for completed order {OrderId}",
                        MaskPhoneNumber(phoneNumber), order.Id);
                }
                else
                {
                    logger.LogWarning("Failed to send SMS notification to {PhoneNumber} for completed order {OrderId}",
                        MaskPhoneNumber(phoneNumber), order.Id);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sending SMS notification for completed order {OrderId}", order.Id);
            }
        }

        private static string MaskPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length <= 4)
                return phoneNumber;

            return string.Concat("...", phoneNumber.AsSpan(phoneNumber.Length - 4));
        }
    }
}