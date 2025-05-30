using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace libraryproject.ViewModels.TaiLieu
{
    public class TaiLieuViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã sách")]
        [StringLength(50)]
        [Display(Name = "Mã sách")]
        public string MaSach { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã DDC")]
        public string? DDC { get; set; }

        [StringLength(50)]
        [Display(Name = "Mã Cutter")]
        public string? Cutter { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tác giả")]
        [StringLength(100)]
        [Display(Name = "Tác giả")]
        public string TacGia { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nhan đề")]
        [StringLength(200)]
        [Display(Name = "Nhan đề")]
        public string NhanDe { get; set; }

        [StringLength(200)]
        [Display(Name = "Phần còn lại của nhan đề")]
        public string? PhanConLaiNhanDe { get; set; }

        [StringLength(200)]
        [Display(Name = "Thông tin trách nhiệm")]
        public string? ThongTinTrachNhiem { get; set; }

        [StringLength(200)]
        [Display(Name = "Nhan đề song song")]
        public string? NhanDeSongSong { get; set; }

        [StringLength(100)]
        [Display(Name = "Nơi xuất bản")]
        public string? NoiXuatBan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nhà xuất bản")]
        [StringLength(100)]
        [Display(Name = "Nhà xuất bản")]
        public string NhaXuatBan { get; set; }

        [Range(1800, 2100)]
        [Display(Name = "Năm xuất bản")]
        public int? NamXuatBan { get; set; }

        [Range(1, 10000)]
        [Display(Name = "Tổng số trang")]
        public int? TongSoTrang { get; set; }

        [StringLength(50)]
        [Display(Name = "Khổ giấy")]
        public string? KhoGiay { get; set; }

        [Display(Name = "Phụ chú")]
        public string? PhuChu { get; set; }

        [Display(Name = "Thuật ngữ chủ đề")]
        public string? ThuatNguChuDe { get; set; }

        [Display(Name = "Khoa")]
        public string? Khoa { get; set; }

        [Display(Name = "Nơi lưu trữ")]
        public string? NoiLuuTru { get; set; }

        [Display(Name = "Ký hiệu kho")]
        public string? KyHieuKho { get; set; }

        [Display(Name = "Hình ảnh hiện tại")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Hình ảnh mới")]
        public IFormFile? HinhAnhFile { get; set; }

        [Required]
        [Range(0, 1000)]
        [Display(Name = "Số lượng nhập về")]
        public int SoLuongNhapVe { get; set; }

        [Range(0, 1000)]
        [Display(Name = "Số lượng hiện có")]
        public int? SoLuongHienCo { get; set; }

        [Required]
        [Display(Name = "Bộ sưu tập")]
        public int BoSuuTapID { get; set; }

        [Display(Name = "Tên bộ sưu tập")]
        public string? TenBoSuuTap { get; set; }

        [Display(Name = "File PDF hiện tại")]
        public string? FilePDF { get; set; }

        [Display(Name = "File PDF mới")]
        public IFormFile? FilePDFFile { get; set; }

        [Display(Name = "Ngày nhập")]
        [DataType(DataType.Date)]
        public DateTime? NgayNhap { get; set; }

        public static TaiLieuViewModel FromEntity(Models.TaiLieu entity)
        {
            return new TaiLieuViewModel
            {
                ID = entity.ID,
                MaSach = entity.MaSach,
                DDC = entity.DDC,
                Cutter = entity.Cutter,
                TacGia = entity.TacGia,
                NhanDe = entity.NhanDe,
                PhanConLaiNhanDe = entity.PhanConLaiNhanDe,
                ThongTinTrachNhiem = entity.ThongTinTrachNhiem,
                NhanDeSongSong = entity.NhanDeSongSong,
                NoiXuatBan = entity.NoiXuatBan,
                NhaXuatBan = entity.NhaXuatBan,
                NamXuatBan = entity.NamXuatBan,
                TongSoTrang = entity.TongSoTrang,
                KhoGiay = entity.KhoGiay,
                PhuChu = entity.PhuChu,
                ThuatNguChuDe = entity.ThuatNguChuDe,
                Khoa = entity.Khoa,
                NoiLuuTru = entity.NoiLuuTru,
                KyHieuKho = entity.KyHieuKho,
                HinhAnh = entity.HinhAnh,
                SoLuongNhapVe = entity.SoLuongNhapVe,
                SoLuongHienCo = entity.SoLuongHienCo,
                BoSuuTapID = entity.BoSuuTapID,
                TenBoSuuTap = entity.BoSuuTap?.TenBoSuuTap,
                FilePDF = entity.FilePDF,
                NgayNhap = entity.NgayNhap
            };
        }

        public Models.TaiLieu ToEntity()
        {
            return new Models.TaiLieu
            {
                ID = this.ID,
                MaSach = this.MaSach,
                DDC = this.DDC ?? "",
                Cutter = this.Cutter ?? "",
                TacGia = this.TacGia,
                NhanDe = this.NhanDe,
                PhanConLaiNhanDe = this.PhanConLaiNhanDe ?? "",
                ThongTinTrachNhiem = this.ThongTinTrachNhiem ?? "",
                NhanDeSongSong = this.NhanDeSongSong ?? "",
                NoiXuatBan = this.NoiXuatBan ?? "",
                NhaXuatBan = this.NhaXuatBan,
                NamXuatBan = this.NamXuatBan ?? DateTime.Now.Year,
                TongSoTrang = this.TongSoTrang ?? 0,
                KhoGiay = this.KhoGiay ?? "",
                PhuChu = this.PhuChu ?? "",
                ThuatNguChuDe = this.ThuatNguChuDe ?? "",
                Khoa = this.Khoa ?? "",
                NoiLuuTru = this.NoiLuuTru ?? "",
                KyHieuKho = this.KyHieuKho ?? "",
                HinhAnh = this.HinhAnh ?? "",
                SoLuongNhapVe = this.SoLuongNhapVe,
                SoLuongHienCo = this.SoLuongHienCo ?? 0,
                BoSuuTapID = this.BoSuuTapID,
                FilePDF = this.FilePDF ?? "",
                NgayNhap = this.NgayNhap ?? DateTime.Now
            };
        }
    }
}
