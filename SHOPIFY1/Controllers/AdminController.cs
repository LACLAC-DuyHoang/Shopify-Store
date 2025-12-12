using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using SHOPIFY1.Filters; // ← SẼ TẠO SAU

namespace SHOPIFY1.Controllers
{
    [Authorize] // Phải đăng nhập
    [AdminRole] // ← CHỈ ROLE 2 MỚI VÀO ĐƯỢC
    public class AdminController : Controller
    {
        private readonly ShopifyContext _context;

        public AdminController(ShopifyContext context)
        {
            _context = context;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.TotalProducts = await _context.SanPhams.CountAsync();
            ViewBag.TotalOrders = await _context.DonHangs.CountAsync();
            ViewBag.TotalRevenue = await _context.ChiTietDonHangs
                .SumAsync(c => c.SoLuong * c.DonGia);
            ViewBag.PendingOrders = await _context.DonHangs
                .CountAsync(d => d.TrangThai == "Chờ xử lý");

            return View();
        }

        // === QUẢN LÝ SẢN PHẨM ===
        public async Task<IActionResult> Products(string search = null)
        {
            var query = _context.SanPhams
                .Include(p => p.MaDmNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.TenSp.Contains(search));
                ViewBag.Search = search;
            }

            var products = await query.ToListAsync();
            return View(products);
        }

        // Tạo sản phẩm
        public async Task<IActionResult> CreateProduct()
        {
            ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(SanPham model, IFormFile? hinhAnh)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
                return View(model);
            }

            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(hinhAnh.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await hinhAnh.CopyToAsync(stream);
                }
                model.HinhAnh = "/images/" + fileName;
            }
            else
            {
                model.HinhAnh = "/images/no-image.jpg";
            }

            _context.SanPhams.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction(nameof(Products));
        }

        // Sửa sản phẩm
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.SanPhams.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(SanPham model, IFormFile? hinhAnh)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.DanhMucs = await _context.DanhMucs.ToListAsync();
                return View(model);
            }

            var existing = await _context.SanPhams.AsNoTracking().FirstOrDefaultAsync(p => p.MaSp == model.MaSp);
            if (existing == null) return NotFound();

            if (hinhAnh != null && hinhAnh.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(hinhAnh.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await hinhAnh.CopyToAsync(stream);
                }
                model.HinhAnh = "/images/" + fileName;
            }
            else
            {
                model.HinhAnh = existing.HinhAnh;
            }

            _context.SanPhams.Update(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction(nameof(Products));
        }

        // Xóa sản phẩm
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.SanPhams.FindAsync(id);
            if (product == null) return Json(new { success = false });

            _context.SanPhams.Remove(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}