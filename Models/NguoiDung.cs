// ~/Models/NguoiDung.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace libraryproject.Models
{
    public class NguoiDung
    {
        public int ID { get; set; }

        [Required]
        public string? MaBanDoc { get; set; }

        [Required]
        public string? HoTen { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? MatKhau { get; set; }

        [Required]
        public int LoaiBanDocID { get; set; }

        // Các trường không bắt buộc
        public string? Khoa { get; set; }
        public string? Lop { get; set; }
        public string? KhoaHoc { get; set; }
        public string? Nganh { get; set; }
        public string? DiaChi { get; set; }
        public string? MaPhu { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string? GioiTinh { get; set; }
        public string? AuthToken { get; set; }


        [Required]
        public DateTime NgayThanhVien { get; set; }

        [Required]
        public DateTime NgayHetHan { get; set; }

        public string? HeChuongTrinh { get; set; }
        public string SDT { get; set; }
        public string? GhiChu { get; set; }

        [Required]
        public string? Role { get; set; }

        // Navigation properties
        public virtual LoaiBanDoc LoaiBanDoc { get; set; }
        public virtual ICollection<PhieuMuon> PhieuMuons { get; set; }
        public virtual ICollection<ThongBao> ThongBaos { get; set; }
    }
}