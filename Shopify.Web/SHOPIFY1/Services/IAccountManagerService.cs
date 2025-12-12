using SHOPIFY1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SHOPIFY1.Services
{
    public interface IAccountManagerService
    {
        // Lấy danh sách Tài khoản Khách hàng (Role 5)
        Task<List<TaiKhoan>> GetAllCustomersAsync();

        // Thêm: Lấy danh sách Tài khoản Nhân viên (Role 2, 3, 4)
        Task<List<TaiKhoan>> GetAllStaffsAsync();

        // Thêm: Lấy danh sách tất cả các Vai trò
        Task<List<VaiTro>> GetAllRolesAsync();

        // Lấy Tài khoản theo ID
        Task<TaiKhoan?> GetAccountByIdAsync(int id);

        // Cập nhật trạng thái (Kích hoạt/Vô hiệu hóa)
        Task UpdateAccountStatusAsync(int id, bool newStatus);

        // Cập nhật thông tin cơ bản của tài khoản (dành cho quản trị viên)
        Task UpdateAccountInfoAsync(TaiKhoan account);

        // Thêm: Thêm Tài khoản Nhân viên mới (Giảm thiểu trường)
        Task AddStaffAccountAsync(TaiKhoan account);
    }
}