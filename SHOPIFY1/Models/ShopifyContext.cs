using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SHOPIFY1.Models;

public partial class ShopifyContext : DbContext
{
    public ShopifyContext()
    {
    }

    public ShopifyContext(DbContextOptions<ShopifyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietGioHang> ChiTietGioHangs { get; set; }

    public virtual DbSet<DanhGium> DanhGia { get; set; }

    public virtual DbSet<DanhMuc> DanhMucs { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<GioHang> GioHangs { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<LichSuGiaoHang> LichSuGiaoHangs { get; set; }

    public virtual DbSet<MaGiamGium> MaGiamGia { get; set; }

    public virtual DbSet<PhanHoiKhachHang> PhanHoiKhachHangs { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<SuDungMaGiamGium> SuDungMaGiamGia { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<ThongKe> ThongKes { get; set; }

    public virtual DbSet<VaiTro> VaiTros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DUY-HOANG\\LACLAC;Initial Catalog=SHOPIFY;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.MaCtdh).HasName("PK__ChiTietD__1E4E40F06A43FAD4");

            entity.ToTable("ChiTietDonHang");

            entity.HasIndex(e => e.MaDh, "IX_ChiTietDonHang_MaDH");

            entity.HasIndex(e => e.MaSp, "IX_ChiTietDonHang_MaSP");

            entity.Property(e => e.MaCtdh).HasColumnName("MaCTDH");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDh)
                .HasConstraintName("FK__ChiTietDon__MaDH__52593CB8");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaSp)
                .HasConstraintName("FK__ChiTietDon__MaSP__534D60F1");
        });

        modelBuilder.Entity<ChiTietGioHang>(entity =>
        {
            entity.HasKey(e => e.MaCtgh).HasName("PK__ChiTietG__1E4FAF547D5EDD3F");

            entity.ToTable("ChiTietGioHang");

            entity.Property(e => e.MaCtgh).HasColumnName("MaCTGH");
            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaGh).HasColumnName("MaGH");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");

            entity.HasOne(d => d.MaGhNavigation).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.MaGh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietGio__MaGH__6D0D32F4");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.ChiTietGioHangs)
                .HasForeignKey(d => d.MaSp)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ChiTietGio__MaSP__6E01572D");
        });

        modelBuilder.Entity<DanhGium>(entity =>
        {
            entity.HasKey(e => e.MaDg).HasName("PK__DanhGia__272586603653F2A8");

            entity.HasIndex(e => e.MaSp, "IX_DanhGia_MaSP");

            entity.Property(e => e.MaDg).HasColumnName("MaDG");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.NgayDg)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("NgayDG");
            entity.Property(e => e.NoiDung).HasMaxLength(1000);

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK__DanhGia__MaKH__571DF1D5");

            entity.HasOne(d => d.MaSpNavigation).WithMany(p => p.DanhGia)
                .HasForeignKey(d => d.MaSp)
                .HasConstraintName("FK__DanhGia__MaSP__5629CD9C");
        });

        modelBuilder.Entity<DanhMuc>(entity =>
        {
            entity.HasKey(e => e.MaDm).HasName("PK__DanhMuc__2725866E6680C429");

            entity.ToTable("DanhMuc");

            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenDm)
                .HasMaxLength(150)
                .HasColumnName("TenDM");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDh).HasName("PK__DonHang__272586612D3A902B");

            entity.ToTable("DonHang");

            entity.HasIndex(e => e.MaKh, "IX_DonHang_MaKH");

            entity.HasIndex(e => e.NgayDat, "IX_DonHang_NgayDat");

            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.DiaChiGiao).HasMaxLength(255);
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ xác nhận");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.DonHangMaKhNavigations)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK__DonHang__MaKH__4CA06362");

            entity.HasOne(d => d.MaShipperNavigation).WithMany(p => p.DonHangMaShipperNavigations)
                .HasForeignKey(d => d.MaShipper)
                .HasConstraintName("FK__DonHang__MaShipp__4F7CD00D");
        });

        modelBuilder.Entity<GioHang>(entity =>
        {
            entity.HasKey(e => e.MaGh).HasName("PK__GioHang__2725AE8564FCC27C");

            entity.ToTable("GioHang");

            entity.HasIndex(e => e.MaKh, "IX_GioHang_MaKH");

            entity.Property(e => e.MaGh).HasColumnName("MaGH");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Đang hoạt động");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.GioHangs)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GioHang__MaKH__68487DD7");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKm).HasName("PK__KhuyenMa__2725CF15A72CA1AA");

            entity.ToTable("KhuyenMai");

            entity.Property(e => e.MaKm).HasColumnName("MaKM");
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TenKm)
                .HasMaxLength(200)
                .HasColumnName("TenKM");
        });

        modelBuilder.Entity<LichSuGiaoHang>(entity =>
        {
            entity.HasKey(e => e.MaLs).HasName("PK__LichSuGi__2725C772DE696C18");

            entity.ToTable("LichSuGiaoHang");

            entity.Property(e => e.MaLs).HasColumnName("MaLS");
            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThaiGiao).HasMaxLength(50);

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.LichSuGiaoHangs)
                .HasForeignKey(d => d.MaDh)
                .HasConstraintName("FK__LichSuGiao__MaDH__5EBF139D");

            entity.HasOne(d => d.MaShipperNavigation).WithMany(p => p.LichSuGiaoHangs)
                .HasForeignKey(d => d.MaShipper)
                .HasConstraintName("FK__LichSuGia__MaShi__5FB337D6");
        });

        modelBuilder.Entity<MaGiamGium>(entity =>
        {
            entity.HasKey(e => e.MaMgg).HasName("PK__MaGiamGi__3AA19447CF5C8701");

            entity.HasIndex(e => e.MaCode, "UQ__MaGiamGi__152C7C5C4C7323E2").IsUnique();

            entity.Property(e => e.MaMgg).HasColumnName("MaMGG");
            entity.Property(e => e.GiaTriToiDa)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MaCode).HasMaxLength(50);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.SoLanSuDungToiDa).HasDefaultValue(0);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Hoạt động");
        });

        modelBuilder.Entity<PhanHoiKhachHang>(entity =>
        {
            entity.HasKey(e => e.MaPh).HasName("PK__PhanHoiK__2725E7FA4A7CCB2B");

            entity.ToTable("PhanHoiKhachHang");

            entity.Property(e => e.MaPh).HasColumnName("MaPH");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.NgayPhanHoi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasMaxLength(1000);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Chưa xử lý");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.PhanHoiKhachHangs)
                .HasForeignKey(d => d.MaKh)
                .HasConstraintName("FK__PhanHoiKha__MaKH__6383C8BA");
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSp).HasName("PK__SanPham__2725081CF48A526C");

            entity.ToTable("SanPham");

            entity.HasIndex(e => e.MaDm, "IX_SanPham_MaDM");

            entity.HasIndex(e => e.TenSp, "IX_SanPham_TenSP");

            entity.Property(e => e.MaSp).HasColumnName("MaSP");
            entity.Property(e => e.Gia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HangSx)
                .HasMaxLength(150)
                .HasColumnName("HangSX");
            entity.Property(e => e.HinhAnh).HasMaxLength(500);
            entity.Property(e => e.MaDm).HasColumnName("MaDM");
            entity.Property(e => e.NgayCapNhat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoLuongTon).HasDefaultValue(0);
            entity.Property(e => e.TenSp)
                .HasMaxLength(250)
                .HasColumnName("TenSP");
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaDmNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaDm)
                .HasConstraintName("FK__SanPham__MaDM__440B1D61");
        });

        modelBuilder.Entity<SuDungMaGiamGium>(entity =>
        {
            entity.HasKey(e => e.MaSdmgg).HasName("PK__SuDungMa__2CECBE2F03B02253");

            entity.Property(e => e.MaSdmgg).HasColumnName("MaSDMGG");
            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.MaKh).HasColumnName("MaKH");
            entity.Property(e => e.MaMgg).HasColumnName("MaMGG");
            entity.Property(e => e.NgaySuDung)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoTienGiam)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.SuDungMaGiamGia)
                .HasForeignKey(d => d.MaDh)
                .HasConstraintName("FK__SuDungMaGi__MaDH__7F2BE32F");

            entity.HasOne(d => d.MaKhNavigation).WithMany(p => p.SuDungMaGiamGia)
                .HasForeignKey(d => d.MaKh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SuDungMaGi__MaKH__7E37BEF6");

            entity.HasOne(d => d.MaMggNavigation).WithMany(p => p.SuDungMaGiamGia)
                .HasForeignKey(d => d.MaMgg)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SuDungMaG__MaMGG__7D439ABD");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTk).HasName("PK__TaiKhoan__272500707C662061");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.Email, "IX_TaiKhoan_Email");

            entity.HasIndex(e => e.MaVaiTro, "IX_TaiKhoan_MaVaiTro");

            entity.HasIndex(e => e.Email, "UQ__TaiKhoan__A9D105346AA4EEB8").IsUnique();

            entity.Property(e => e.MaTk).HasColumnName("MaTK");
            entity.Property(e => e.DiaChi).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(300);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TrangThai).HasDefaultValue(true);

            entity.HasOne(d => d.MaVaiTroNavigation).WithMany(p => p.TaiKhoans)
                .HasForeignKey(d => d.MaVaiTro)
                .HasConstraintName("FK__TaiKhoan__MaVaiT__3B75D760");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaTt).HasName("PK__ThanhToa__27250079F6ECEE7B");

            entity.ToTable("ThanhToan");

            entity.Property(e => e.MaTt).HasColumnName("MaTT");
            entity.Property(e => e.MaDh).HasColumnName("MaDH");
            entity.Property(e => e.MaGiaoDich).HasMaxLength(100);
            entity.Property(e => e.NgayThanhToan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PhuongThuc).HasMaxLength(50);
            entity.Property(e => e.SoTien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Chờ xử lý");

            entity.HasOne(d => d.MaDhNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaDh)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ThanhToan__MaDH__71D1E811");
        });

        modelBuilder.Entity<ThongKe>(entity =>
        {
            entity.HasKey(e => e.MaThongKe).HasName("PK__ThongKe__60E521F483DB2774");

            entity.ToTable("ThongKe");

            entity.Property(e => e.DoanhThu).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HieuSuatNhanVien).HasMaxLength(500);
            entity.Property(e => e.SanPhamBanChay).HasMaxLength(500);
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<VaiTro>(entity =>
        {
            entity.HasKey(e => e.MaVaiTro).HasName("PK__VaiTro__C24C41CF6E100FFF");

            entity.ToTable("VaiTro");

            entity.HasIndex(e => e.TenVaiTro, "UQ__VaiTro__1DA55814EA4DB9C8").IsUnique();

            entity.Property(e => e.TenVaiTro).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
