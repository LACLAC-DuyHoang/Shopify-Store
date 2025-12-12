using Microsoft.AspNetCore.Mvc;
using SHOPIFY1.Services;

namespace SHOPIFY1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService) => _productService = productService;


        public async Task<IActionResult> Index()
        {
            var list = await _productService.GetLatestAsync(50);
            return View(list);
        }


        public async Task<IActionResult> Details(int id)
        {
            var model = await _productService.GetByIdAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }
    }
}
