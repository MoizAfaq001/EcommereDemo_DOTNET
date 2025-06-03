using AutoMapper;
using Daraz101_Data;
using Daraz101_Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Daraz101.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _cartService = cartService;
            _userManager = userManager;
            _mapper = mapper;
        }

        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID not found.");
            return userId;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetUserId();
                var items = await _cartService.GetCartItemsAsync(userId);
                return View(items);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Could not load cart items: " + ex.Message;
                return View(new List<CartItemDTO>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity)
        {
            try
            {
                var userId = GetUserId();
                await _cartService.AddToCartAsync(userId, productId, quantity);
                TempData["Message"] = "Item added to cart successfully.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            try
            {
                var userId = GetUserId();
                await _cartService.UpdateQuantityAsync(userId, productId, quantity);
                TempData["Message"] = "Quantity updated.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            try
            {
                var userId = GetUserId();
                await _cartService.RemoveFromCartAsync(userId, productId);
                TempData["Message"] = "Item removed.";
            }
            catch (KeyNotFoundException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            var userId = GetUserId();
            await _cartService.ClearCartAsync(userId);
            TempData["Message"] = "Cart cleared.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = GetUserId();
            var cartItems = await _cartService.GetCartItemsAsync(userId);
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }
            
            return RedirectToAction("Checkout", "Order");
        }
    }
}
