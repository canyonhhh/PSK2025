using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSK2025.ApiService.Services.Interfaces;
using PSK2025.Models.DTOs;
using PSK2025.Models.Enums;
using PSK2025.Models.Extensions;
using Stripe;
using PSK2025.Data.Repositories.Interfaces;
using PSK2025.Models.Entities;

namespace PSK2025.ApiService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IGetUserIdService _getUserIdService;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            IGetUserIdService getUserIdService,
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            ILogger<PaymentController> logger)
        {
            _getUserIdService = getUserIdService;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        [HttpPost("create-intent")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = _getUserIdService.GetUserIdFromToken();
                
                // Get user's cart to calculate total
                var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                if (cart == null || cart.Items.Count == 0)
                {
                    return BadRequest(new { message = "Cart is empty" });
                }

                var totalAmount = cart.Items.Sum(item => item.Product!.Price * item.Quantity);
                var amountInCents = (long)(totalAmount * 100); // Convert to cents for Stripe

                _logger.LogInformation("Creating payment intent for user {UserId} with amount {Amount}", 
                    userId, totalAmount);

                var paymentIntentService = new PaymentIntentService();
                var options = new PaymentIntentCreateOptions
                {
                    Amount = amountInCents,
                    Currency = request.Currency.ToLower(),
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        ["userId"] = userId,
                        ["expectedCompletionTime"] = request.ExpectedCompletionTime.ToString("O")
                    }
                };

                var paymentIntent = await paymentIntentService.CreateAsync(options);

                _logger.LogInformation("Payment intent created successfully for user {UserId} with client secret", userId);

                return Ok(new
                {
                    paymentIntentId = paymentIntent.Id,
                    clientSecret = paymentIntent.ClientSecret,
                    amount = totalAmount,
                    currency = request.Currency
                });
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating payment intent");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent");
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

        [HttpPost("confirm")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = _getUserIdService.GetUserIdFromToken();
                
                _logger.LogInformation("Confirming payment for user {UserId}, payment intent {PaymentIntentId}", 
                    userId, request.PaymentIntentId);

                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(request.PaymentIntentId);

                // Verify the payment intent belongs to this user
                if (!paymentIntent.Metadata.TryGetValue("userId", out var paymentUserId) || paymentUserId != userId)
                {
                    return Forbid("Payment intent does not belong to current user");
                }

                if (paymentIntent.Status == "succeeded")
                {
                    // Create order from cart
                    var cart = await _cartRepository.GetCartByUserIdAsync(userId);
                    if (cart == null || cart.Items.Count == 0)
                    {
                        return BadRequest(new { message = "Cart is empty" });
                    }

                    var orderItems = cart.Items.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product!.Title,
                        ProductPrice = item.Product.Price,
                        Quantity = item.Quantity
                    }).ToList();

                    // Parse expected completion time from metadata
                    DateTime.TryParse(paymentIntent.Metadata["expectedCompletionTime"], out var expectedCompletionTime);

                    var order = new Order
                    {
                        UserId = userId,
                        StripePaymentIntentId = paymentIntent.Id, // Store Stripe payment intent ID
                        CreatedAt = DateTime.UtcNow,
                        ExpectedCompletionTime = expectedCompletionTime,
                        Status = OrderStatus.Pending,
                        Items = orderItems
                    };

                    await _orderRepository.CreateAsync(order);
                    await _cartRepository.ClearCartAsync(userId);

                    _logger.LogInformation("Payment confirmed and order created for user {UserId}, order {OrderId}", 
                        userId, order.Id);

                    return Ok(new
                    {
                        success = true,
                        orderId = order.Id,
                        message = "Payment successful and order created!"
                    });
                }
                else if (paymentIntent.Status == "requires_payment_method")
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Payment failed. Please try a different payment method."
                    });
                }
                else
                {
                    return Ok(new
                    {
                        success = false,
                        message = "Payment is still processing. Please wait."
                    });
                }
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error confirming payment");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming payment");
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }
    }
}