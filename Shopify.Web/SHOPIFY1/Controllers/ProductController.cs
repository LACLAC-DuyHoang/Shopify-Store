using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using SHOPIFY1.Services;

namespace SHOPIFY1.Controllers
{
    public class ProductController : Controller
    {
        private readonly ShopifyContext _context;
        private readonly IProductService _productService;

        public ProductController(ShopifyContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        // GET: /Product/ByCategory?id=1&priceRange=10-20&search=iphone
        public async Task<IActionResult> ByCategory(int? id, string priceRange = null, string search = null)
        {
            var categories = await _context.DanhMucs.ToListAsync();
            ViewBag.Categories = categories;
            ViewBag.SelectedCategoryId = id;
            ViewBag.SelectedPriceRange = priceRange;
            ViewBag.Search = search;

            var products = await _productService.GetByCategoryAsync(id, priceRange, search, 50);

            ViewBag.Search = search;

            if (!products.Any() && !string.IsNullOrEmpty(search))
            {
                TempData["Info"] = $"Không tìm thấy sản phẩm nào với từ khóa: <strong>\"{search}\"</strong>";
            }

            return View(products);

            
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue) return NotFound();

            var product = await _productService.GetByIdAsync(id.Value);
            if (product == null) return NotFound();

            return View(product);
        }
        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return Json(new List<object>());

            var results = await _context.SanPhams
                .Where(p => EF.Functions.Like(p.TenSp, $"%{query}%"))
                .Take(6)
                .Select(p => new
                {
                    maSp = p.MaSp,
                    tenSp = p.TenSp,
                    gia = p.Gia
                })
                .ToListAsync();

            return Json(results);
        }
    }
}
