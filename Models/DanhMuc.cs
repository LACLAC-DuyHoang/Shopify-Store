using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class DanhMuc
{
    public int MaDm { get; set; }

    public string TenDm { get; set; } = null!;

    public string? MoTa { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
