using System;
using System.ComponentModel.DataAnnotations;

namespace libraryproject.ViewModels.NguoiDung
{
    public class GiaHanTheViewModel
    {
        public int UserID { get; set; }

        [Display(Name = "Họ tên bạn đọc")]
        public string? HoTen { get; set; }

        [Display(Name = "Mã bạn đọc")]
        public string? MaBanDoc { get; set; }

        [Display(Name = "Ngày hết hạn hiện tại")]
        [DataType(DataType.Date)]
        public DateTime NgayHetHanHienTai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tháng gia hạn")]
        [Range(1, 36, ErrorMessage = "Số tháng gia hạn phải từ 1 đến 36 tháng")]
        [Display(Name = "Số tháng gia hạn")]
        public int SoThangGiaHan { get; set; }
    }
}