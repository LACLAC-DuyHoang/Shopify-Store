using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class ThongKe
{
    public int MaThongKe { get; set; }

    public DateTime? ThoiGian { get; set; }

    public decimal? DoanhThu { get; set; }

    public string? SanPhamBanChay { get; set; }

    public int? TonKho { get; set; }

    public string? HieuSuatNhanVien { get; set; }
}
