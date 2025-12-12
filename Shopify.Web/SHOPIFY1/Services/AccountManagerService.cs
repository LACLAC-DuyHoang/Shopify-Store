using Microsoft.EntityFrameworkCore;
using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using BCrypt.Net; // ✅ Thêm thư viện mã hóa mật khẩu

namespace SHOPIFY1.Services
{
    // ✅ Đảm bảo lớp này triển khai IAccountManagerService
    public class AccountManagerService : IAccountManagerService
    {
        private readonly ShopifyContext _context;
        private const int CustomerRole = 5;

        public AccountManagerService(ShopifyContext context) => _context = context;

        // --------------------------------------------------------------------
        // Lấy danh sách Vai trò Nội bộ (Role 2, 3, 4)
        // --------------------------------------------------------------------
        public async Task<List<VaiTro>> GetAllRolesAsync()
        {
            // Chỉ lấy các vai trò nội bộ (Manager, Employee, Shipper)
            return await _context.VaiTros
                .Where(v => v.MaVaiTro >= 2 && v.MaVaiTro <= 4)
                .OrderBy(v => v.MaVaiTro)
                .ToListAsync();
        }

        // --------------------------------------------------------------------
        // Lấy danh sách Staffs (Role 2, 3, 4)
        // --------------------------------------------------------------------
        public async Task<List<TaiKhoan>> GetAllStaffsAsync()
        {
            // Chỉ lấy các tài khoản có MaVaiTro 2, 3, 4
            return await _context.TaiKhoans
                .Where(tk => tk.MaVaiTro >= 2 && tk.MaVaiTro <= 4)
                .Include(tk => tk.MaVaiTroNavigation)
                .OrderBy(tk => tk.MaVaiTro)
                .ThenBy(tk => tk.HoTen)
                .ToListAsync();
        }

        // --------------------------------------------------------------------
        // Thêm Tài khoản Staff mới
        // --------------------------------------------------------------------
        public async Task AddStaffAccountAsync(TaiKhoan account)
        {
            if (await _context.TaiKhoans.AnyAsync(u => u.Email == account.Email))
                throw new Exception("Email này đã được sử dụng.");

            if (account.MaVaiTro < 2 || account.MaVaiTro > 4)
                throw new Exception("Vai trò không hợp lệ. Chỉ có thể thêm Manager, Employee, Shipper.");

            if (string.IsNullOrWhiteSpace(account.MatKhau))
                throw new Exception("Mật khẩu không được để trống.");

            // Mã hóa mật khẩu
            account.MatKhau = BCrypt.Net.BCrypt.HashPassword(account.MatKhau);
            account.NgayTao = DateTime.Now;
            account.TrangThai = true; // Mặc định kích hoạt

            _context.TaiKhoans.Add(account);
            await _context.SaveChangesAsync();
        }

        // --------------------------------------------------------------------
        // Lấy danh sách Khách hàng (Giữ nguyên)
        // --------------------------------------------------------------------
        public async Task<List<TaiKhoan>> GetAllCustomersAsync()
        {
            // Chỉ lấy các tài khoản có MaVaiTro = 5 (Customer)
            return await _context.TaiKhoans
                .Where(tk => tk.MaVaiTro == CustomerRole)
                .OrderByDescending(tk => tk.NgayTao)
                .ToListAsync();
        }

        public async Task<TaiKhoan?> GetAccountByIdAsync(int id)
        {
            return await _context.TaiKhoans
                .Include(tk => tk.MaVaiTroNavigation)
                .FirstOrDefaultAsync(tk => tk.MaTk == id);
        }

        // --------------------------------------------------------------------
        // Cập nhật trạng thái (Giữ nguyên logic chính)
        // --------------------------------------------------------------------
        public async Task UpdateAccountStatusAsync(int id, bool newStatus)
        {
            var account = await _context.TaiKhoans.FindAsync(id);
            if (account == null) throw new Exception("Tài khoản không tồn tại.");

            // Thay đổi logic kiểm tra: Cho phép thay đổi trạng thái của cả Staff và Customer
            // Tuy nhiên, không cho phép Manager vô hiệu hóa chính mình (cần check ở Controller)
            // if (account.MaVaiTro != CustomerRole) throw new Exception("Không thể thay đổi trạng thái của tài khoản quản trị.");

            account.TrangThai = newStatus;
            _context.TaiKhoans.Update(account);
            await _context.SaveChangesAsync();
        }

        // --------------------------------------------------------------------
        // Cập nhật thông tin cơ bản (Đã thêm logic Role)
        // --------------------------------------------------------------------
        public async Task UpdateAccountInfoAsync(TaiKhoan account)
        {
            var existing = await _context.TaiKhoans.FindAsync(account.MaTk);
            if (existing == null) throw new Exception("Tài khoản không tồn tại.");

            // Kiểm tra trùng Email (trừ chính nó)
            if (await _context.TaiKhoans.AnyAsync(tk => tk.Email == account.Email && tk.MaTk != account.MaTk))
                throw new Exception($"Email '{account.Email}' đã được sử dụng bởi tài khoản khác.");

            // Cập nhật các trường được phép
            existing.HoTen = account.HoTen;
            existing.Email = account.Email;
            existing.SoDienThoai = account.SoDienThoai;
            existing.DiaChi = account.DiaChi;
            existing.TrangThai = account.TrangThai; // Có thể cập nhật trạng thái

            // Nếu là tài khoản nội bộ (Staff: Role 2, 3, 4), cho phép cập nhật Vai trò
            if (existing.MaVaiTro >= 2 && existing.MaVaiTro <= 4)
            {
                if (account.MaVaiTro < 2 || account.MaVaiTro > 4)
                    throw new Exception("Vai trò mới không hợp lệ cho tài khoản nội bộ.");
                existing.MaVaiTro = account.MaVaiTro;
            }
            else if (existing.MaVaiTro == CustomerRole)
            {
                // Đảm bảo không thể thay đổi vai trò của Khách hàng thành Staff
                existing.MaVaiTro = CustomerRole;
            }

            _context.TaiKhoans.Update(existing);
            await _context.SaveChangesAsync();
        }
    }
}