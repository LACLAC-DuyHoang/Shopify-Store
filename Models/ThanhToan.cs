using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class ThanhToan
{
    public int MaTt { get; set; }

    public int MaDh { get; set; }

    public decimal SoTien { get; set; }

    public string PhuongThuc { get; set; } = null!;

    public string? TrangThai { get; set; }

    public string? MaGiaoDich { get; set; }

    public DateTime? NgayThanhToan { get; set; }

    public virtual DonHang MaDhNavigation { get; set; } = null!;
}
