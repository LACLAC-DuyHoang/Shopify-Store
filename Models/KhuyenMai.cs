using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class KhuyenMai
{
    public int MaKm { get; set; }

    public string TenKm { get; set; } = null!;

    public int? PhanTramGiam { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public DateTime? NgayKetThuc { get; set; }

    public string? MoTa { get; set; }
}
