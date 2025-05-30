using System;
using System.ComponentModel.DataAnnotations;

namespace libraryproject.ViewModels.PhieuMuonAdmin
{
    public class ChiTietPhieuMuonAdminViewModel
    {
        public int ID { get; set; }

        public int PhieuMuonID { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn tài liệu")]
        [Display(Name = "Tài liệu")]
        public int TaiLieuID { get; set; }

        [Display(Name = "Tên tài liệu")]
        public string? TenTaiLieu { get; set; }

        [Display(Name = "Mã sách")]
        public string? MaSach { get; set; }

        [Display(Name = "Tác giả")]
        public string? TacGia { get; set; }

        [Display(Name = "Ngày trả")]
        [DataType(DataType.Date)]
        public DateTime? NgayTra { get; set; }

        [Display(Name = "Tiền phạt")]
        [DataType(DataType.Currency)]
        public decimal? TienPhat { get; set; }

        [Display(Name = "Trạng thái")]
        public string? TrangThai { get; set; }

        // Method to map from entity to view model
        public static ChiTietPhieuMuonAdminViewModel FromEntity(Models.ChiTietPhieuMuon entity)
        {
            return new ChiTietPhieuMuonAdminViewModel
            {
                ID = entity.ID,
                PhieuMuonID = entity.PhieuMuonID,
                TaiLieuID = entity.TaiLieuID,
                TenTaiLieu = entity.TaiLieu?.NhanDe,
                MaSach = entity.TaiLieu?.MaSach,
                TacGia = entity.TaiLieu?.TacGia,
                NgayTra = entity.NgayTra,
                TienPhat = entity.TienPhat,
                TrangThai = entity.TrangThai
            };
        }

        // Method to map from view model to entity
        public Models.ChiTietPhieuMuon ToEntity()
        {
            return new Models.ChiTietPhieuMuon
            {
                ID = this.ID,
                PhieuMuonID = this.PhieuMuonID,
                TaiLieuID = this.TaiLieuID,
                NgayTra = this.NgayTra,
                TienPhat = this.TienPhat,
                TrangThai = this.TrangThai ?? "Đang mượn"
            };
        }
    }
}
