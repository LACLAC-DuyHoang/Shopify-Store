using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class DonHang
{
    public int MaDh { get; set; }

    public int? MaKh { get; set; }

    public DateTime? NgayDat { get; set; }

    public decimal? TongTien { get; set; }

    public string? TrangThai { get; set; }

    public string? DiaChiGiao { get; set; }

    public string? PhuongThucThanhToan { get; set; }

    public int? MaShipper { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; set; } = new List<ChiTietDonHang>();

    public virtual ICollection<LichSuGiaoHang> LichSuGiaoHangs { get; set; } = new List<LichSuGiaoHang>();

    public virtual TaiKhoan? MaKhNavigation { get; set; }

    public virtual TaiKhoan? MaShipperNavigation { get; set; }

    public virtual ICollection<SuDungMaGiamGium> SuDungMaGiamGia { get; set; } = new List<SuDungMaGiamGium>();

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
