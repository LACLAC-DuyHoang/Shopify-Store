namespace SHOPIFY1.Models
{
    public class CartItem
    {
        public int MaSp { get; set; }
        public string TenSp { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }

        // Tính tổng tiền cho 1 sản phẩm trong giỏ
        public decimal ThanhTien => Gia * SoLuong;
    }
}
