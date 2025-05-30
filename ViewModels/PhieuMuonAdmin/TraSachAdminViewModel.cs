using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace libraryproject.ViewModels.PhieuMuonAdmin
{
    public class TraSachAdminViewModel
    {
        public int PhieuMuonID { get; set; }

        public string MaPhieuMuon { get; set; } = null!;

        public string? TenNguoiMuon { get; set; }

        public string? MaBanDoc { get; set; }

        public DateTime NgayMuon { get; set; }

        public DateTime NgayHenTra { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày trả")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày trả")]
        public DateTime NgayTra { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public List<ChiTietTraSachAdminViewModel>? ChiTietTraSach { get; set; }
    }

    public class ChiTietTraSachAdminViewModel
    {
        public int ChiTietPhieuMuonID { get; set; }

        public int TaiLieuID { get; set; }

        public string? TenTaiLieu { get; set; }

        public string? MaSach { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = null!;

        [Display(Name = "Tiền phạt")]
        [DataType(DataType.Currency)]
        public decimal? TienPhat { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }
    }
}
