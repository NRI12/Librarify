using System;
using System.ComponentModel.DataAnnotations;

namespace libraryproject.ViewModels.NguoiDung
{
    public class NguoiDungViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Mã bạn đọc")]
        [StringLength(20, ErrorMessage = "Mã bạn đọc không được vượt quá 20 ký tự")]
        [Display(Name = "Mã bạn đọc")]
        public string? MaBanDoc { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ tên")]
        public string? HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự.", MinimumLength = 6)]
        public string ?MatKhau { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("MatKhau", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
        public string? XacNhanMatKhau { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Loại bạn đọc")]
        [Display(Name = "Loại bạn đọc")]
        public int LoaiBanDocID { get; set; }

        [Display(Name = "Tên loại bạn đọc")]
        public string? TenLoaiBanDoc { get; set; }

        [Display(Name = "Khoa")]
        public string? Khoa { get; set; }

        [Display(Name = "Lớp")]
        public string? Lop { get; set; }

        [Display(Name = "Khóa học")]
        public string? KhoaHoc { get; set; }

        [Display(Name = "Ngành")]
        public string? Nganh { get; set; }

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [Display(Name = "Mã phụ")]
        public string? MaPhu { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Display(Name = "Ngày đăng ký")]
        [DataType(DataType.Date)]
        public DateTime NgayThanhVien { get; set; }

        [Display(Name = "Ngày hết hạn")]
        [DataType(DataType.Date)]
        public DateTime NgayHetHan { get; set; }

        [Display(Name = "Hệ chương trình")]
        public string? HeChuongTrinh { get; set; }

        [Display(Name = "Số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        public string? SDT { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        [Display(Name = "Token")]
        public string? AuthToken { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        [Display(Name = "Vai trò")]
        public string? Role { get; set; }

        // Method to map from entity to view model
        public static NguoiDungViewModel FromEntity(Models.NguoiDung entity)
        {
            return new NguoiDungViewModel
            {
                ID = entity.ID,
                MaBanDoc = entity.MaBanDoc,
                HoTen = entity.HoTen,
                Email = entity.Email,
                LoaiBanDocID = entity.LoaiBanDocID,
                TenLoaiBanDoc = entity.LoaiBanDoc?.TenLoai,
                Khoa = entity.Khoa,
                Lop = entity.Lop,
                KhoaHoc = entity.KhoaHoc,
                Nganh = entity.Nganh,
                DiaChi = entity.DiaChi,
                MaPhu = entity.MaPhu,
                NgaySinh = entity.NgaySinh,
                GioiTinh = entity.GioiTinh,
                NgayThanhVien = entity.NgayThanhVien,
                NgayHetHan = entity.NgayHetHan,
                HeChuongTrinh = entity.HeChuongTrinh,
                SDT = entity.SDT,
                GhiChu = entity.GhiChu,
                AuthToken = entity.AuthToken,
                Role = entity.Role
            };
        }

        // Method to map from view model to entity
        public Models.NguoiDung ToEntity()
        {
            return new Models.NguoiDung
            {
                ID = this.ID,
                MaBanDoc = this.MaBanDoc,
                HoTen = this.HoTen,
                Email = this.Email,
                MatKhau = this.MatKhau,  // Note: This should be hashed before saving
                LoaiBanDocID = this.LoaiBanDocID,
                Khoa = this.Khoa ?? "",
                Lop = this.Lop ?? "",
                KhoaHoc = this.KhoaHoc ?? "",
                Nganh = this.Nganh ?? "",
                DiaChi = this.DiaChi ?? "",
                MaPhu = this.MaPhu ?? "",
                NgaySinh = this.NgaySinh,
                GioiTinh = this.GioiTinh ?? "",
                NgayThanhVien = this.NgayThanhVien,
                NgayHetHan = this.NgayHetHan,
                HeChuongTrinh = this.HeChuongTrinh ?? "",
                SDT = this.SDT ?? "",
                GhiChu = this.GhiChu ?? "",
                AuthToken = this.AuthToken ?? "",
                Role = this.Role
            };
        }
    }
}