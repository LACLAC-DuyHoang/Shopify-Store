
IF DB_ID('SHOPIFY') IS NULL
    CREATE DATABASE SHOPIFY;
GO

USE SHOPIFY;
GO

-- Bảng Vai trò
CREATE TABLE VaiTro (
    MaVaiTro INT IDENTITY(1,1) PRIMARY KEY,
    TenVaiTro NVARCHAR(50) NOT NULL UNIQUE
);
GO

-- Bảng Tài khoản
CREATE TABLE TaiKhoan (
    MaTK INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    SoDienThoai NVARCHAR(20),
    MatKhau NVARCHAR(300) NOT NULL,
    MaVaiTro INT FOREIGN KEY REFERENCES VaiTro(MaVaiTro),
    TrangThai BIT DEFAULT 1,
    DiaChi NVARCHAR(500),
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

-- Bảng Danh mục
CREATE TABLE DanhMuc (
    MaDM INT IDENTITY(1,1) PRIMARY KEY,
    TenDM NVARCHAR(150) NOT NULL,
    MoTa NVARCHAR(500),
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

-- Bảng Sản phẩm
CREATE TABLE SanPham (
    MaSP INT IDENTITY(1,1) PRIMARY KEY,
    TenSP NVARCHAR(250) NOT NULL,
    HangSX NVARCHAR(150),
    Gia DECIMAL(18,2) NOT NULL,
    MoTa NVARCHAR(MAX),
    HinhAnh NVARCHAR(500),
    SoLuongTon INT DEFAULT 0,
    MaDM INT FOREIGN KEY REFERENCES DanhMuc(MaDM),
    TrangThai BIT DEFAULT 1,
    NgayTao DATETIME DEFAULT GETDATE(),
    NgayCapNhat DATETIME DEFAULT GETDATE()
);
GO

-- Bảng Khuyến mãi
CREATE TABLE KhuyenMai (
    MaKM INT IDENTITY(1,1) PRIMARY KEY,
    TenKM NVARCHAR(200) NOT NULL,
    PhanTramGiam INT CHECK (PhanTramGiam BETWEEN 0 AND 100),
    NgayBatDau DATETIME,
    NgayKetThuc DATETIME,
    MoTa NVARCHAR(500)
);
GO

-- Bảng Đơn hàng
CREATE TABLE DonHang (
    MaDH INT IDENTITY(1,1) PRIMARY KEY,
    MaKH INT FOREIGN KEY REFERENCES TaiKhoan(MaTK),
    NgayDat DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(18,2),
    TrangThai NVARCHAR(50) DEFAULT N'Chờ xác nhận',
    DiaChiGiao NVARCHAR(255),
    PhuongThucThanhToan NVARCHAR(50),
    MaShipper INT FOREIGN KEY REFERENCES TaiKhoan(MaTK)
);
GO

-- Bảng Chi tiết đơn hàng
CREATE TABLE ChiTietDonHang (
    MaCTDH INT IDENTITY(1,1) PRIMARY KEY,
    MaDH INT FOREIGN KEY REFERENCES DonHang(MaDH),
    MaSP INT FOREIGN KEY REFERENCES SanPham(MaSP),
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL
);
GO

-- Bảng Đánh giá
CREATE TABLE DanhGia (
    MaDG INT IDENTITY(1,1) PRIMARY KEY,
    MaSP INT FOREIGN KEY REFERENCES SanPham(MaSP),
    MaKH INT FOREIGN KEY REFERENCES TaiKhoan(MaTK),
    NoiDung NVARCHAR(1000),
    SoSao INT CHECK (SoSao BETWEEN 1 AND 5),
    NgayDG DATETIME DEFAULT GETDATE()
);
GO

-- Bảng Thống kê
CREATE TABLE ThongKe (
    MaThongKe INT IDENTITY(1,1) PRIMARY KEY,
    ThoiGian DATETIME DEFAULT GETDATE(),
    DoanhThu DECIMAL(18,2),
    SanPhamBanChay NVARCHAR(500),
    TonKho INT,
    HieuSuatNhanVien NVARCHAR(500)
);
GO

-- Bảng Lịch sử giao hàng
CREATE TABLE LichSuGiaoHang (
    MaLS INT IDENTITY(1,1) PRIMARY KEY,
    MaDH INT FOREIGN KEY REFERENCES DonHang(MaDH),
    MaShipper INT FOREIGN KEY REFERENCES TaiKhoan(MaTK),
    TrangThaiGiao NVARCHAR(50),
    NgayCapNhat DATETIME DEFAULT GETDATE()
);
GO

-- Bảng Phản hồi khách hàng
CREATE TABLE PhanHoiKhachHang (
    MaPH INT IDENTITY(1,1) PRIMARY KEY,
    MaKH INT FOREIGN KEY REFERENCES TaiKhoan(MaTK),
    NoiDung NVARCHAR(1000),
    NgayPhanHoi DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) DEFAULT N'Chưa xử lý'
);
GO
CREATE TABLE GioHang (
    MaGH INT IDENTITY(1,1) PRIMARY KEY,
    MaKH INT NOT NULL FOREIGN KEY REFERENCES TaiKhoan(MaTK),
    NgayTao DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) DEFAULT N'Đang hoạt động'  -- có thể là 'Đang hoạt động', 'Đã đặt hàng'
);
GO

------------------------------------------------------------
-- 2️⃣ BẢNG CHI TIẾT GIỎ HÀNG (CART ITEMS)
------------------------------------------------------------
CREATE TABLE ChiTietGioHang (
    MaCTGH INT IDENTITY(1,1) PRIMARY KEY,
    MaGH INT NOT NULL FOREIGN KEY REFERENCES GioHang(MaGH),
    MaSP INT NOT NULL FOREIGN KEY REFERENCES SanPham(MaSP),
    SoLuong INT CHECK (SoLuong > 0),
    DonGia DECIMAL(18,2) NOT NULL
);
GO

------------------------------------------------------------
-- 3️⃣ BẢNG THANH TOÁN (PAYMENTS)
------------------------------------------------------------
CREATE TABLE ThanhToan (
    MaTT INT IDENTITY(1,1) PRIMARY KEY,
    MaDH INT NOT NULL FOREIGN KEY REFERENCES DonHang(MaDH),
    SoTien DECIMAL(18,2) NOT NULL,
    PhuongThuc NVARCHAR(50) NOT NULL,     -- Ví dụ: 'COD', 'Momo', 'ZaloPay', 'VNPay', 'PayPal'
    TrangThai NVARCHAR(50) DEFAULT N'Chờ xử lý',  -- Chờ xử lý / Thành công / Thất bại / Hoàn tiền
    MaGiaoDich NVARCHAR(100),             -- Mã trả về từ cổng thanh toán
    NgayThanhToan DATETIME DEFAULT GETDATE()
);
GO

------------------------------------------------------------
-- 4️⃣ BẢNG MÃ GIẢM GIÁ (COUPON)
------------------------------------------------------------
CREATE TABLE MaGiamGia (
    MaMGG INT IDENTITY(1,1) PRIMARY KEY,
    MaCode NVARCHAR(50) UNIQUE NOT NULL,    -- Ví dụ: SALE10, FREESHIP
    MoTa NVARCHAR(255),
    PhanTramGiam INT CHECK (PhanTramGiam BETWEEN 0 AND 100),
    GiaTriToiDa DECIMAL(18,2) DEFAULT 0,
    NgayBatDau DATETIME NOT NULL,
    NgayKetThuc DATETIME NOT NULL,
    SoLanSuDungToiDa INT DEFAULT 0,        -- 0 = không giới hạn
    TrangThai NVARCHAR(50) DEFAULT N'Hoạt động'
);
GO

------------------------------------------------------------
-- 5️⃣ BẢNG LƯU LỊCH SỬ SỬ DỤNG MÃ GIẢM GIÁ (COUPON REDEMPTION)
------------------------------------------------------------
CREATE TABLE SuDungMaGiamGia (
    MaSDMGG INT IDENTITY(1,1) PRIMARY KEY,
    MaMGG INT NOT NULL FOREIGN KEY REFERENCES MaGiamGia(MaMGG),
    MaKH INT NOT NULL FOREIGN KEY REFERENCES TaiKhoan(MaTK),
    MaDH INT FOREIGN KEY REFERENCES DonHang(MaDH),
    NgaySuDung DATETIME DEFAULT GETDATE(),
    SoTienGiam DECIMAL(18,2) DEFAULT 0
);
GO

------------------------------------------------------------
-- 6️⃣ INDEXES BỔ SUNG TỐI ƯU HIỆU NĂNG
------------------------------------------------------------

CREATE INDEX IX_TaiKhoan_Email ON TaiKhoan(Email);
CREATE INDEX IX_TaiKhoan_MaVaiTro ON TaiKhoan(MaVaiTro);


CREATE INDEX IX_SanPham_MaDM ON SanPham(MaDM);
CREATE INDEX IX_SanPham_TenSP ON SanPham(TenSP);

-- Đơn hàng
CREATE INDEX IX_DonHang_MaKH ON DonHang(MaKH);
CREATE INDEX IX_DonHang_NgayDat ON DonHang(NgayDat);

-- Chi tiết đơn hàng
CREATE INDEX IX_ChiTietDonHang_MaDH ON ChiTietDonHang(MaDH);
CREATE INDEX IX_ChiTietDonHang_MaSP ON ChiTietDonHang(MaSP);

-- Đánh giá
CREATE INDEX IX_DanhGia_MaSP ON DanhGia(MaSP);

-- Giỏ hàng
CREATE INDEX IX_GioHang_MaKH ON GioHang(MaKH);


-- Dữ liệu mẫu
INSERT INTO VaiTro (TenVaiTro) VALUES (N'Admin'), (N'Manager'), (N'Employee'), (N'Shipper'), (N'Customer');
GO

INSERT INTO TaiKhoan (HoTen, Email, MatKhau, MaVaiTro, TrangThai, DiaChi)
VALUES
(N'Nguyễn Văn Admin', 'admin@shopify.vn', '123456', 1, 1, N'Hà Nội'),
(N'Trần Thị Quản Lý', 'manager@shopify.vn', '123456', 2, 1, N'Hà Nội'),
(N'Lê Văn Nhân Viên', 'employee@shopify.vn', '123456', 3, 1, N'Hà Nội'),
(N'Phạm Văn Shipper', 'shipper@shopify.vn', '123456', 4, 1, N'Hà Nội'),
(N'Hoàng Minh Khách', 'customer@shopify.vn', '123456', 5, 1, N'Hà Nội');
GO

------------------------------------------------------------
-- 🌐 THÊM DANH MỤC SẢN PHẨM
------------------------------------------------------------
INSERT INTO DanhMuc (TenDM, MoTa) VALUES
(N'Điện thoại', N'Sản phẩm điện thoại di động và smartphone'),
(N'Laptop', N'Máy tính xách tay cho học tập và làm việc'),
(N'Tai nghe', N'Tai nghe có dây, không dây, chống ồn'),
(N'Tủ lạnh', N'Tủ lạnh các hãng phổ biến'),
(N'Máy giặt', N'Máy giặt cửa trước và cửa trên'),
(N'Tivi', N'Tivi thông minh, OLED, QLED'),
(N'Phụ kiện', N'Phụ kiện điện tử: chuột, bàn phím, cáp sạc');
GO

------------------------------------------------------------
-- 💻 THÊM SẢN PHẨM
------------------------------------------------------------
INSERT INTO SanPham (TenSP, HangSX, Gia, SoLuongTon, MaDM, MoTa, HinhAnh)
VALUES
-- 📱 Điện thoại
(N'iPhone 15 Pro Max', N'Apple', 38990000, 15, 1, N'Điện thoại flagship của Apple, chip A17 Pro, camera 48MP', N'/images/iphone15promax.jpg'),
(N'Samsung Galaxy S24 Ultra', N'Samsung', 33990000, 20, 1, N'Màn hình AMOLED 120Hz, camera zoom 10x, bút S-Pen', N'/images/s24ultra.jpg'),
(N'Xiaomi 14', N'Xiaomi', 17990000, 25, 1, N'Chip Snapdragon 8 Gen 3, màn hình 1.5K, sạc nhanh 90W', N'/images/xiaomi14.jpg'),
(N'OPPO Find X7 Pro', N'OPPO', 25990000, 18, 1, N'Thiết kế sang trọng, camera hợp tác cùng Hasselblad', N'/images/findx7pro.jpg'),

-- 💻 Laptop
(N'MacBook Air M3 2024', N'Apple', 32990000, 12, 2, N'Laptop mỏng nhẹ, chip Apple M3, pin 18 giờ', N'/images/macbookairm3.jpg'),
(N'ASUS ZenBook 14 OLED', N'ASUS', 24990000, 10, 2, N'Màn hình OLED 2.8K, chip Intel Core Ultra 7', N'/images/zenbook14.jpg'),
(N'HP Pavilion 15', N'HP', 15990000, 20, 2, N'Laptop học tập và văn phòng, màn hình 15.6 inch FullHD', N'/images/hppavilion15.jpg'),
(N'Lenovo Legion 5 Pro', N'Lenovo', 35990000, 8, 2, N'Laptop gaming RTX 4070, tản nhiệt tốt, bàn phím RGB', N'/images/legion5pro.jpg'),

-- 🎧 Tai nghe
(N'AirPods Pro 2', N'Apple', 5990000, 40, 3, N'Tai nghe chống ồn chủ động, sạc MagSafe', N'/images/airpodspro2.jpg'),
(N'Sony WH-1000XM5', N'Sony', 7990000, 30, 3, N'Tai nghe over-ear chống ồn tốt nhất thị trường', N'/images/sonywh1000xm5.jpg'),
(N'JBL Tune 230NC', N'JBL', 2490000, 50, 3, N'Tai nghe không dây giá rẻ, pin 40 giờ', N'/images/jbltune230.jpg'),
(N'Beats Studio Buds', N'Beats', 4990000, 25, 3, N'Thiết kế thời trang, kết nối nhanh với iPhone', N'/images/beatsstudiobuds.jpg'),

-- 🧊 Tủ lạnh
(N'Tủ lạnh LG Inverter 420L', N'LG', 13990000, 10, 4, N'Tủ lạnh tiết kiệm điện, ngăn đông lớn', N'/images/lg420l.jpg'),
(N'Tủ lạnh Samsung 500L Side by Side', N'Samsung', 18990000, 6, 4, N'Cảm biến nhiệt độ thông minh, sang trọng', N'/images/samsung500l.jpg'),
(N'Tủ lạnh Aqua 186L', N'Aqua', 5990000, 15, 4, N'Phù hợp gia đình nhỏ, làm lạnh nhanh', N'/images/aqua186l.jpg'),

-- 🧺 Máy giặt
(N'Máy giặt Electrolux 10kg', N'Electrolux', 8990000, 8, 5, N'Máy giặt cửa trước, tiết kiệm nước', N'/images/electrolux10kg.jpg'),
(N'Máy giặt Samsung AI 12kg', N'Samsung', 13990000, 10, 5, N'Công nghệ AI Wash tự động điều chỉnh lượng nước', N'/images/samsungai12kg.jpg'),
(N'Máy giặt Toshiba Inverter 9kg', N'Toshiba', 7490000, 15, 5, N'Hoạt động êm ái, tiết kiệm điện', N'/images/toshiba9kg.jpg'),

-- 📺 Tivi
(N'Tivi LG OLED C4 55 inch', N'LG', 27990000, 5, 6, N'Tivi OLED 4K, Dolby Vision, AI ThinQ', N'/images/lgoledc4.jpg'),
(N'Samsung QLED Q80C 65 inch', N'Samsung', 32990000, 7, 6, N'Tivi QLED 120Hz, HDR10+, Smart Hub', N'/images/samsungq80c.jpg'),
(N'Sony Bravia XR 55 inch', N'Sony', 30990000, 6, 6, N'Hình ảnh sắc nét, âm thanh vòm 360 Reality Audio', N'/images/sonybraviaxr.jpg'),

-- ⚙️ Phụ kiện
(N'Chuột Logitech G Pro X Superlight', N'Logitech', 3590000, 40, 7, N'Chuột gaming không dây, siêu nhẹ chỉ 63g', N'/images/logitechgprox.jpg'),
(N'Bàn phím cơ Keychron K6', N'Keychron', 2790000, 25, 7, N'Bàn phím cơ Bluetooth, hot-swappable switch', N'/images/keychronk6.jpg'),
(N'Sạc nhanh Anker 65W', N'Anker', 1290000, 50, 7, N'Sạc nhanh PD 3.0, cổng USB-C', N'/images/anker65w.jpg'),
(N'Cáp sạc Baseus Type-C', N'Baseus', 199000, 80, 7, N'Cáp sạc bọc dù, sạc nhanh 100W', N'/images/baseuscable.jpg'),
(N'Thẻ nhớ SanDisk 128GB', N'SanDisk', 399000, 60, 7, N'Tốc độ cao, phù hợp điện thoại và máy ảnh', N'/images/sandisk128.jpg');
GO

------------------------------------------------------------
-- 💰 THÊM MÃ GIẢM GIÁ
------------------------------------------------------------
INSERT INTO MaGiamGia (MaCode, MoTa, PhanTramGiam, GiaTriToiDa, NgayBatDau, NgayKetThuc)
VALUES
(N'SALE10', N'Giảm 10% cho đơn hàng đầu tiên', 10, 2000000, GETDATE(), DATEADD(MONTH, 3, GETDATE())),
(N'FREESHIP', N'Miễn phí vận chuyển toàn quốc', 5, 100000, GETDATE(), DATEADD(MONTH, 6, GETDATE())),
(N'SUPER20', N'Giảm 20% cho sản phẩm trên 20 triệu', 20, 5000000, GETDATE(), DATEADD(MONTH, 1, GETDATE())),
(N'VIPCUSTOMER', N'Dành cho khách hàng thân thiết', 15, 3000000, GETDATE(), DATEADD(MONTH, 12, GETDATE()));
GO