using Daraz101_Services;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Daraz101.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        // This is the Checkout page (GET)
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            //try
            //{
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var cartItems = await _cartService.GetCartItemsAsync(userId);

                if (!cartItems.Any())
                {
                    TempData["Error"] = "Your cart is empty. Please add items before checkout.";
                    return RedirectToAction("Index", "Cart");
                }

                return View(cartItems); // Should match Views/Order/Checkout.cshtml
            //}
            //catch (Exception ex)
            //{
            //    TempData["Error"] = $"Error loading checkout: {ex.Message}";
            //    return RedirectToAction("Index", "Cart");
            //}
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(string fullAddress)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Json(new { redirectUrl = Url.Action("Login", "Account") });

                if (string.IsNullOrWhiteSpace(fullAddress))
                {
                    return Json(new { error = "Please provide a valid shipping address." });
                }

                var orderId = await _orderService.PlaceOrderAsync(userId, fullAddress);
                await _cartService.ClearCartAsync(userId);

                var redirectUrl = Url.Action("Confirmation", new { id = orderId });
                return Json(new { redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { error = $"Order failed: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.UserId != userId)
            {
                TempData["Error"] = "Order not found or you are not authorized.";
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }


        public async Task<IActionResult> OrderHistory()
        {
            //try
            //{
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var orders = await _orderService.GetOrdersByUserAsync(userId);
                return View(orders);
            //}
            //catch (Exception ex)
            //{
            //    TempData["Error"] = $"Could not load order history: {ex.Message}";
            //    return View(new List<OrderDTO>());
            //}
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            //try
            //{
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return RedirectToAction("Login", "Account");

                var order = await _orderService.GetOrderByIdAsync(id);

                if (order == null)
                    return NotFound();

                return View(order);
            //}
            //catch (Exception ex)
            //{
            //    TempData["Error"] = $"Could not load order details: {ex.Message}";
            //    return RedirectToAction("OrderHistory");
            //}
        }
    }
}
