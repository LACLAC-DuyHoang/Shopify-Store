using System;
using System.Collections.Generic;

namespace SHOPIFY1.Models;

public partial class MaGiamGium
{
    public int MaMgg { get; set; }

    public string MaCode { get; set; } = null!;

    public string? MoTa { get; set; }

    public int? PhanTramGiam { get; set; }

    public decimal? GiaTriToiDa { get; set; }

    public DateTime NgayBatDau { get; set; }

    public DateTime NgayKetThuc { get; set; }

    public int? SoLanSuDungToiDa { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<SuDungMaGiamGium> SuDungMaGiamGia { get; set; } = new List<SuDungMaGiamGium>();
}
