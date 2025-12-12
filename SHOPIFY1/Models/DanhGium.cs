using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class DanhGium
{
    public int MaDg { get; set; }

    public int? MaSp { get; set; }

    public int? MaKh { get; set; }

    public string? NoiDung { get; set; }

    public int? SoSao { get; set; }

    public DateTime? NgayDg { get; set; }

    public virtual TaiKhoan? MaKhNavigation { get; set; }

    public virtual SanPham? MaSpNavigation { get; set; }
}
