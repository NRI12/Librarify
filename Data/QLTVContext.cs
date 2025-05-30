// Data/QLTVContext.cs
using libraryproject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace libraryproject.Data
{
    public class QLTVContext : DbContext
    {
        public QLTVContext(DbContextOptions<QLTVContext> options)
            : base(options)
        {
        }

        // Constructor không tham số cho migration
        public QLTVContext()
        {
        }

        // Ghi đè phương thức này để cấu hình khi sử dụng constructor không tham số
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("QLTVConnection"));
            }
        }

        public DbSet<NguoiDung> NguoiDungs { get; set; }
        public DbSet<LoaiBanDoc> LoaiBanDocs { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }
        public DbSet<BoSuuTap> BoSuuTaps { get; set; }
        public DbSet<TaiLieu> TaiLieus { get; set; }
        public DbSet<PhieuMuon> PhieuMuons { get; set; }
        public DbSet<ChiTietPhieuMuon> ChiTietPhieuMuons { get; set; }
        public DbSet<MaVach> MaVachs { get; set; }
        public DbSet<ThongBao> ThongBaos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // NguoiDung - LoaiBanDoc (Many-to-One)
            modelBuilder.Entity<NguoiDung>()
                .HasOne(n => n.LoaiBanDoc)
                .WithMany(l => l.NguoiDungs)
                .HasForeignKey(n => n.LoaiBanDocID);

            // PhieuMuon - NguoiDung (Many-to-One)
            modelBuilder.Entity<PhieuMuon>()
                .HasOne(p => p.NguoiDung)
                .WithMany(n => n.PhieuMuons)
                .HasForeignKey(p => p.NguoiDungID);

            // PhieuMuon - NhanVien (Many-to-One)
            modelBuilder.Entity<PhieuMuon>()
                .HasOne(p => p.NhanVien)
                .WithMany(n => n.PhieuMuons)
                .HasForeignKey(p => p.NhanVienID);

            // TaiLieu - BoSuuTap (Many-to-One)
            modelBuilder.Entity<TaiLieu>()
                .HasOne(t => t.BoSuuTap)
                .WithMany(b => b.TaiLieus)
                .HasForeignKey(t => t.BoSuuTapID);

            // ChiTietPhieuMuon - PhieuMuon (Many-to-One)
            modelBuilder.Entity<ChiTietPhieuMuon>()
                .HasOne(c => c.PhieuMuon)
                .WithMany(p => p.ChiTietPhieuMuons)
                .HasForeignKey(c => c.PhieuMuonID);

            // ChiTietPhieuMuon - TaiLieu (Many-to-One)
            modelBuilder.Entity<ChiTietPhieuMuon>()
                .HasOne(c => c.TaiLieu)
                .WithMany(t => t.ChiTietPhieuMuons)
                .HasForeignKey(c => c.TaiLieuID);

            // MaVach - TaiLieu (Many-to-One)
            modelBuilder.Entity<MaVach>()
                .HasOne(m => m.TaiLieu)
                .WithMany(t => t.MaVachs)
                .HasForeignKey(m => m.TaiLieuID);

            // ThongBao - NguoiDung (Many-to-One)
            modelBuilder.Entity<ThongBao>()
                .HasOne(t => t.NguoiDung)
                .WithMany(n => n.ThongBaos)
                .HasForeignKey(t => t.NguoiDungID);
        }
    }
}