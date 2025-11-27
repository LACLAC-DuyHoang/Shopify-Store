using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class LichSuGiaoHang
{
    public int MaLs { get; set; }

    public int? MaDh { get; set; }

    public int? MaShipper { get; set; }

    public string? TrangThaiGiao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    public virtual DonHang? MaDhNavigation { get; set; }

    public virtual TaiKhoan? MaShipperNavigation { get; set; }
}
