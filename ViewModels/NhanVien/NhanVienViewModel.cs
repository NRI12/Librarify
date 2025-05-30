using System;
using System.ComponentModel.DataAnnotations;

namespace libraryproject.ViewModels.NhanVien
{
    public class NhanVienViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã nhân viên")]
        [StringLength(20, ErrorMessage = "Mã nhân viên không được vượt quá 20 ký tự")]
        [Display(Name = "Mã nhân viên")]
        public string MaNhanVien { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ tên")]
        public string ?HoTen { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [Display(Name = "Email")]
        public string ?Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [Display(Name = "Số điện thoại")]
        public string ?SDT { get; set; }

        // Method to map from entity to view model
        public static NhanVienViewModel FromEntity(Models.NhanVien entity)
        {
            return new NhanVienViewModel
            {
                ID = entity.ID,
                MaNhanVien = entity.MaNhanVien,
                HoTen = entity.HoTen,
                NgaySinh = entity.NgaySinh,
                Email = entity.Email,
                SDT = entity.SDT
            };
        }

        // Method to map from view model to entity
        public Models.NhanVien ToEntity()
        {
            return new Models.NhanVien
            {
                ID = this.ID,
                MaNhanVien = this.MaNhanVien,
                HoTen = this.HoTen,
                NgaySinh = this.NgaySinh,
                Email = this.Email,
                SDT = this.SDT ?? ""
            };
        }
    }
}