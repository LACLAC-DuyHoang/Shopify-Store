using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;

namespace SHOPIFY1.Controllers;

public class HomeController : Controller
{
    private readonly ShopifyContext _context;
    public HomeController(ShopifyContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // L?y danh m?c
        var categories = await _context.DanhMucs
            .OrderBy(c => c.TenDm)
            .ToListAsync();

        // L?y s?n ph?m n?i b?t (m?i nh?t)
        var products = await _context.SanPhams
            .Where(p => p.TrangThai == true)
            .OrderByDescending(p => p.NgayTao)
            .Take(8)
            .ToListAsync();

        ViewBag.Categories = categories;
        return View(products);
    }
    public IActionResult AccessDenied()
    {
        return View();
    }
}

