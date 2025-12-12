using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class GioHang
{
    public int MaGh { get; set; }

    public int MaKh { get; set; }

    public DateTime? NgayTao { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; set; } = new List<ChiTietGioHang>();

    public virtual TaiKhoan MaKhNavigation { get; set; } = null!;
}
