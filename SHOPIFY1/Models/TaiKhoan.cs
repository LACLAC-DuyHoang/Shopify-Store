using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class TaiKhoan
{
    public int MaTk { get; set; }

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string MatKhau { get; set; } = null!;

    public int? MaVaiTro { get; set; }

    public bool? TrangThai { get; set; }

    public string? DiaChi { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<DanhGium> DanhGia { get; set; } = new List<DanhGium>();

    public virtual ICollection<DonHang> DonHangMaKhNavigations { get; set; } = new List<DonHang>();

    public virtual ICollection<DonHang> DonHangMaShipperNavigations { get; set; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual ICollection<LichSuGiaoHang> LichSuGiaoHangs { get; set; } = new List<LichSuGiaoHang>();

    public virtual VaiTro? MaVaiTroNavigation { get; set; }

    public virtual ICollection<PhanHoiKhachHang> PhanHoiKhachHangs { get; set; } = new List<PhanHoiKhachHang>();

    public virtual ICollection<SuDungMaGiamGium> SuDungMaGiamGia { get; set; } = new List<SuDungMaGiamGium>();
}
