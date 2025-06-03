using Daraz101_Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Daraz101.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            List<ProductDTO> products;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                products = await _productService.GetAllProductsAsync();
            }
            else
            {
                products = await _productService.SearchProductsAsync(searchTerm);
            }

            if (products == null || !products.Any())
            {
                return NotFound("No products match your search.");
            }

            return View(products);
        }

        [AllowAnonymous]  // Also allow non-logged-in users to view product details 
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductDTO product)
        {
            if (ModelState.IsValid)
            {
                _productService.CreateProductAsync(product);
                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: Product/Edit/
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ProductDTO model)
        {
            if (!ModelState.IsValid) return View(model);

            await _productService.UpdateProductAsync(model); // no variable assignment

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var product = _productService.GetProductByIdAsync(id);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _productService.DeleteProductAsync(id);
            return RedirectToAction("Index");
        }
    }
}
