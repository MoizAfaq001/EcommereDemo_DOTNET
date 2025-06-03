using Daraz101_Data;
using Daraz101_Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Daraz101.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string? searchTerm) // Mark method as async
        {
            // Fetch products asynchronously
            var products = string.IsNullOrWhiteSpace(searchTerm)
                ? await _productService.GetAllProductsAsync() // Await the Task
                : await _productService.SearchProductsAsync(searchTerm); // Await the Task

            // Separate featured and all
            var featuredProducts = products.Where(p => p.IsFeatured).Take(5).ToList();

            var viewModel = new HomeViewModel
            {
                FeaturedProducts = featuredProducts,
                AllProducts = products.ToList(), // Show all products (including featured)
                SearchTerm = searchTerm ?? string.Empty // ensuring SearchTerm is not null
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
