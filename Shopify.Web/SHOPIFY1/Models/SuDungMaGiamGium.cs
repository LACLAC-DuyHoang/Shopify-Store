using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class SuDungMaGiamGium
{
    public int MaSdmgg { get; set; }

    public int MaMgg { get; set; }

    public int MaKh { get; set; }

    public int? MaDh { get; set; }

    public DateTime? NgaySuDung { get; set; }

    public decimal? SoTienGiam { get; set; }

    public virtual DonHang? MaDhNavigation { get; set; }

    public virtual TaiKhoan MaKhNavigation { get; set; } = null!;

    public virtual MaGiamGium MaMggNavigation { get; set; } = null!;
}
