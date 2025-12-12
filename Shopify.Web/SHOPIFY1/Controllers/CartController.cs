using Microsoft.AspNetCore.Mvc;
using SHOPIFY1.Models;
using SHOPIFY1.Services;
using SHOPIFY1.Extensions;
namespace SHOPIFY1.Controllers
{
    public class CartController : Controller
    {
        private readonly ShopifyContext _context;

        public CartController(ShopifyContext context)
        {
            _context = context;
        }

        // GET: /Cart/Index
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            ViewBag.Total = cart.Sum(c => c.ThanhTien);
            return View(cart);
        }

        // GET: /Cart/Add/5?soLuong=2
        public async Task<IActionResult> Add(int id, int soLuong = 1)
        {
            if (soLuong <= 0) soLuong = 1;

            var product = await _context.SanPhams.FindAsync(id);
            if (product == null)
                return NotFound("Sản phẩm không tồn tại.");

            if (product.SoLuongTon < soLuong)
            {
                TempData["Error"] = $"Chỉ còn {product.SoLuongTon} sản phẩm trong kho!";
                return RedirectToAction("Index");
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(c => c.MaSp == id);
            if (existingItem != null)
            {
                int newQuantity = existingItem.SoLuong + soLuong;
                if (newQuantity > product.SoLuongTon)
                {
                    TempData["Error"] = $"Không thể thêm! Chỉ còn {product.SoLuongTon} sản phẩm.";
                    return RedirectToAction("Index");
                }
                existingItem.SoLuong = newQuantity;
            }
            else
            {
                cart.Add(new CartItem
                {
                    MaSp = product.MaSp,
                    TenSp = product.TenSp,
                    Gia = product.Gia,
                    SoLuong = soLuong,
                    HinhAnh = product.HinhAnh ?? "/images/no-image.jpg"
                });
            }

            HttpContext.Session.SetObjectAsJson("Cart", cart);
            TempData["Success"] = $"Đã thêm {soLuong} {product.TenSp} vào giỏ hàng!";
            return RedirectToAction("Index");
        }

        // POST: /Cart/Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(List<CartItem> items)
        {
            if (items == null || !items.Any())
            {
                TempData["Error"] = "Dữ liệu giỏ hàng không hợp lệ.";
                return RedirectToAction("Index");
            }

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            bool hasChanges = false;

            foreach (var item in items)
            {
                var existing = cart.FirstOrDefault(c => c.MaSp == item.MaSp);
                if (existing == null) continue;

                var product = _context.SanPhams.Find(item.MaSp);
                if (product == null) continue;

                int newQty = item.SoLuong > 0 ? item.SoLuong : 1;
                newQty = Math.Min(newQty, (int)product.SoLuongTon);

                if (existing.SoLuong != newQty)
                {
                    existing.SoLuong = newQty;
                    hasChanges = true;
                }
            }

            // Xóa sản phẩm có số lượng <= 0
            int removedCount = cart.RemoveAll(c => c.SoLuong <= 0);
            if (removedCount > 0) hasChanges = true;

            if (hasChanges)
            {
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                TempData["Success"] = "Cập nhật giỏ hàng thành công!";
            }
            else
            {
                TempData["Info"] = "Không có thay đổi nào.";
            }

            return RedirectToAction("Index");
        }

        // GET: /Cart/Remove/5
        public IActionResult Remove(int id)
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            var removed = cart.RemoveAll(c => c.MaSp == id);
            if (removed > 0)
            {
                HttpContext.Session.SetObjectAsJson("Cart", cart);
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng.";
            }
            return RedirectToAction("Index");
        }

        // GET: /Cart/Clear
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            TempData["Success"] = "Đã làm trống giỏ hàng.";
            return RedirectToAction("Index");
        }
    }
}

