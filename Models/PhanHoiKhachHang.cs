using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class PhanHoiKhachHang
{
    public int MaPh { get; set; }

    public int? MaKh { get; set; }

    public string? NoiDung { get; set; }

    public DateTime? NgayPhanHoi { get; set; }

    public string? TrangThai { get; set; }

    public virtual TaiKhoan? MaKhNavigation { get; set; }
}
