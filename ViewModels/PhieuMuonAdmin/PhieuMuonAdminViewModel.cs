using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace libraryproject.ViewModels.PhieuMuonAdmin
{
    public class PhieuMuonAdminViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã phiếu mượn")]
        [StringLength(50, ErrorMessage = "Mã phiếu mượn không được vượt quá 50 ký tự")]
        [Display(Name = "Mã phiếu mượn")]
        public string MaPhieuMuon { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn người mượn")]
        [Display(Name = "Người mượn")]
        public int NguoiDungID { get; set; }

        [Display(Name = "Tên người mượn")]
        public string? TenNguoiMuon { get; set; }

        [Display(Name = "Mã bạn đọc")]
        public string? MaBanDoc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhân viên")]
        [Display(Name = "Nhân viên")]
        public int NhanVienID { get; set; }

        [Display(Name = "Tên nhân viên")]
        public string? TenNhanVien { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày mượn")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày mượn")]
        public DateTime NgayMuon { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày hẹn trả")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày hẹn trả")]
        public DateTime NgayHenTra { get; set; }

        [Display(Name = "Trạng thái")]
        public string? TrangThai { get; set; }

        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        // Danh sách tài liệu mượn
        public List<ChiTietPhieuMuonAdminViewModel>? ChiTietPhieuMuons { get; set; }

        // Method to map from entity to view model
        public static PhieuMuonAdminViewModel FromEntity(Models.PhieuMuon entity)
        {
            var viewModel = new PhieuMuonAdminViewModel
            {
                ID = entity.ID,
                MaPhieuMuon = entity.MaPhieuMuon,
                NguoiDungID = entity.NguoiDungID,
                TenNguoiMuon = entity.NguoiDung?.HoTen,
                MaBanDoc = entity.NguoiDung?.MaBanDoc,
                NhanVienID = entity.NhanVienID,
                TenNhanVien = entity.NhanVien?.HoTen,
                NgayMuon = entity.NgayMuon,
                NgayHenTra = entity.NgayHenTra,
                TrangThai = entity.TrangThai,
                GhiChu = entity.GhiChu,
                ChiTietPhieuMuons = new List<ChiTietPhieuMuonAdminViewModel>()
            };

            if (entity.ChiTietPhieuMuons != null)
            {
                foreach (var chiTiet in entity.ChiTietPhieuMuons)
                {
                    viewModel.ChiTietPhieuMuons.Add(ChiTietPhieuMuonAdminViewModel.FromEntity(chiTiet));
                }
            }

            return viewModel;
        }

        public Models.PhieuMuon ToEntity()
        {
            return new Models.PhieuMuon
            {
                ID = this.ID,
                MaPhieuMuon = this.MaPhieuMuon,
                NguoiDungID = this.NguoiDungID,
                NhanVienID = this.NhanVienID,
                NgayMuon = this.NgayMuon,
                NgayHenTra = this.NgayHenTra,
                TrangThai = this.TrangThai ?? "Đang mượn",
                GhiChu = this.GhiChu ?? ""
            };
        }
    }
}
